using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Helper;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface IEvidenceService
    {
        Task<List<EvidenceGridData>> DoGetEvidenceGridDetails(string identifier);
    }

    public class EvidenceService : IEvidenceService
    {
        private readonly CrimeDbContext _db;

        public EvidenceService(CrimeDbContext db)
        {
            _db = db;
        }


        public async Task<List<EvidenceGridData>> DoGetEvidenceGridDetails(string identifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                    throw new CustomException("Identifier is required");

                var complaintId = await _db.ComplaintRequests
                    .Where(x => x.Identifier == identifier)
                    .Select(x => x.ComplaintRequestId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (complaintId == 0)
                    throw new CustomException("No Complaint found");

                var evidenceList = await _db.EvidenceAttachments
                    .AsNoTracking()
                    .Where(er => er.ComplaintId == complaintId)
                    .Select(er => new EvidenceGridData
                    {
                        identifier = er.Identifier,
                        evidenceName = er.Filename,
                        createdDate = er.CreatedOn.HasValue
                            ? er.CreatedOn.Value.ToString("dd/MM/yyyy")
                            : string.Empty
                    })
                    .ToListAsync()
                    .ConfigureAwait(false);

                return evidenceList;
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}