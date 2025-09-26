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
    [EnableCors("CrimeManagement")]
    public class CrimeReportController : ControllerBase
    {
        private readonly ICrimeReportService _crimeService;

        public CrimeReportController(ICrimeReportService crimeService)
        {
            _crimeService = crimeService;
        }


        [HttpGet]
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
    }
}
