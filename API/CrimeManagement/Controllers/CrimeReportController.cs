using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrimeReportController : ControllerBase
    {
        private readonly ICrimeReportService _crimeService;

        public CrimeReportController(ICrimeReportService crimeService)
        {
            _crimeService = crimeService;
        }


        [HttpPost]
        [Route("GetCrimeReports")]
        public async Task<IActionResult> DoGetCrimeReports(CrimeRequestviewDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._failure });
                }

                var data = await _crimeService.DoGetCrimeReportDetails(objdto);
                return Ok(new CommonResponseDTO { data = data, responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "Crime Report Details", responseStatus = Helper.CustomHelper._success });
            }
            catch (CustomException cex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = cex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
        }

        [HttpPost]
        [Route("RaiseCrimeReport")]
        public async Task<IActionResult> DoRaiseCrimeReport(CrimeReportDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._failure });
                }
                await _crimeService.DoRaiseCrimeReport(objdto);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "Crime Report Raised Successfully", responseStatus = Helper.CustomHelper._success });
            }
            catch (CustomException cex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = cex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
        }


        [HttpGet]
        [Route("FetchComplaintDetails")]
        public async Task<IActionResult> DoGetParticularCrimeReportDetails(string identifier)
        {
            try
            {
                if (string.IsNullOrEmpty(identifier))
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseMessage = "Invalid Request" });
                }
                var data = await _crimeService.DoGetCrimeReportDetailsById(identifier);
                return Ok(new CommonResponseDTO { data = data, responseCode = 200, responseMessage = "success", responseDatetime = DateTime.Now, responseStatus = Helper.CustomHelper._success });
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
