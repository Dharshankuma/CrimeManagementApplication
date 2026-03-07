using CrimeManagement.DTO;
using CrimeManagement.Helper;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _service;
        public DashBoardController(IDashBoardService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("DoGetDashBoardDetails")]
        public async Task<IActionResult> DoGetCrimeDashBoardDetails()
        {
            try
            {
                var data = await _service.DoGetCrimeDashBoardContent();
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "Dashboard details retrived successfully.", responseStatus = Helper.CustomHelper._success, data = data });
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
