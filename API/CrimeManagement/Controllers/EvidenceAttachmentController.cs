using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class EvidenceAttachmentController : ControllerBase
    {
        private readonly IFileUploadService _fileService;
        private readonly IEvidenceService _evidenceService;

        public EvidenceAttachmentController(IFileUploadService fileservice, IEvidenceService evidenceService)
        {
            _fileService = fileservice;
            _evidenceService = evidenceService;
        }


        [HttpPost]
        [Route("DoUploadEvidence")]
        public async Task<IActionResult> DoUploadEvidenceAttachment(EvidenceUploadRequestDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseMessage = "No data found", responseCode = 400, responseDatetime = DateTime.Now });
                }

                await _fileService.DoUploadEvidenceFiles(objdto);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseStatus = Helper.CustomHelper._success });

            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "An unexpected error occurred.", responseStatus = Helper.CustomHelper._failure, data = ex.Message });
            }
        }

        [HttpPost]
        [Route("DoDownloadEvidence/${identifier}")]
        public async Task<IActionResult> DoDownloadEvidenceFile(string identifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseMessage = "Identifier is required", responseCode = 400, responseDatetime = DateTime.Now });
                }

                var data = await _fileService.DoDownloadEvidenceFile(identifier);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseStatus = Helper.CustomHelper._success, data = data });

            }

            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "An unexpected error occurred.", responseStatus = Helper.CustomHelper._failure, data = ex.Message });
            }
        }

        [HttpGet]
        [Route("DoGetEvidenceFiles/")]
        public async Task<IActionResult> DoGetEvidenceFiles(string complaintIdentifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(complaintIdentifier))
                {
                    return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseMessage = "Complaint identifier is required", responseCode = 400, responseDatetime = DateTime.Now });
                }
                var data = await _evidenceService.DoGetEvidenceGridDetails(complaintIdentifier);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseStatus = Helper.CustomHelper._success, data = data });
            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "An unexpected error occurred.", responseStatus = Helper.CustomHelper._failure, data = ex.Message });
            }
        }
    }
}
