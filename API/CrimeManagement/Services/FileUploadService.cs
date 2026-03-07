using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Helper;
using CrimeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace CrimeManagement.Services
{
    public interface IFileUploadService
    {
        Task<bool> DoUploadEvidenceFiles(EvidenceUploadRequestDTO request);
    }
    public class FileUploadService : IFileUploadService
    {
        private readonly CrimeDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".pdf" };
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png", "application/pdf" };
        private const long MasFileSize = 10 * 1024 * 1024; // 10 MB
        private const int FileSignatureCheckBytes = 8;

        public FileUploadService(CrimeDbContext db, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }


        public async Task<bool> DoUploadEvidenceFiles(EvidenceUploadRequestDTO request)
        {
            if (request != null)
                throw new CustomException("Request cannot be null");

            if (string.IsNullOrWhiteSpace(request.complaintIdentifier))
                throw new CustomException("Complaint identifier is required");

            var files = request.files ?? new List<IFormFile>();
            if (!files.Any())
            {
                throw new CustomException("No file uploaded");
            }

            int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);
            var complaintId = await _db.ComplaintRequests.Where(x => x.Identifier == request.complaintIdentifier).Select(x => x.ComplaintRequestId).FirstOrDefaultAsync();

            if (complaintId == 0)
            {
                throw new CustomException("No complaint found");
            }


            var relativeDirectory = Path.Combine("Uploads", "Evidence");
            var uploadPath = Path.Combine(_env.ContentRootPath, relativeDirectory);

            Directory.CreateDirectory(uploadPath);

            var cancellationToken = _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

            var attachmentsToAdd = new List<EvidenceAttachment>(files.Count);

            foreach (var file in request.files)
            {
                if (file == null || file.Length == 0)
                    continue;

                if (file.Length > MasFileSize)
                    throw new CustomException($"File {file.FileName} exceeds the maximum allowed size of 10 MB");

                var orignialExt = Path.GetExtension(file.FileName);

                if (string.IsNullOrEmpty(orignialExt) || !AllowedExtensions.Contains(orignialExt))
                    throw new CustomException($"File {file.FileName} has an invalid extension. Allowed extensions are: {string.Join(", ", AllowedExtensions)}");

                if (!string.IsNullOrEmpty(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
                    throw new CustomException($"File {file.FileName} has an invalid content type. Allowed content types are: {string.Join(", ", AllowedContentTypes)}");

                if(!await HasValidFileSignatureAsync(file, cancellationToken))
                    throw new CustomException($"File {file.FileName} failed the file signature validation and may not be a valid {orignialExt} file.");

                var fileName = $"{CustomHelper.DoGenerateGuid():N}{orignialExt.ToLowerInvariant()}";
                var filePath = Path.Combine(uploadPath, fileName);

                const int bufferSize = 81920; // 80 KB

                await using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
                }

                var storedPath = Path.Combine(relativeDirectory,fileName).Replace("\\", "/");

                var evidence = new EvidenceAttachment
                {
                    ComplaintId = complaintId,
                    EvidenceAttachmentPath = storedPath,
                    Identifier = CustomHelper.DoGenerateGuid(),
                    CreatedOn = CustomHelper.DoGetDateTime(),
                    CreatedBy = userId
                };

                attachmentsToAdd.Add(evidence);
            }

            if(attachmentsToAdd.Count == 0)
            {
                return false;
            }

            await _db.EvidenceAttachments.AddRangeAsync(attachmentsToAdd).ConfigureAwait(false);
            await _db.SaveChangesAsync().ConfigureAwait(false);

            return true;


        }


        private static async Task<bool> HasValidFileSignatureAsync(IFormFile file, CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();
            var header = new byte[FileSignatureCheckBytes];
            var read = await stream.ReadAsync(header.AsMemory(0, FileSignatureCheckBytes), cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                return false;
            }

            // JPEG: FF D8 FF
            if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            {
                return true;
            }

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (read >= 8 &&
                header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
            {
                return true;
            }

            // PDF: %PDF (25 50 44 46)
            if (read >= 4 && header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46)
            {
                return true;
            }

            return false;
        }
    }
}
