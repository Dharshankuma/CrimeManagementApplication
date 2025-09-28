using CrimeManagement.DTO;
using CrimeManagement.Models;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestigationController : ControllerBase
    {
        private readonly IInvestigationService _service;

        public InvestigationController(IInvestigationService service)
        {
            _service = service;
        }


        [HttpPost]
        [Route("InvestigationGridDetails")]
        public async Task<IActionResult> DoGetInvestigatonsOverviewDetails(InvestigationRequestviewDTO objdto)
        {
            try
            {
                if(objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._success });
                }

                var result = await _service.DoGetInvestigationOverviewDetails(objdto);

                return Ok(new CommonResponseDTO { responseCode = 200,responseDatetime = DateTime.Now, responseMessage = "Success", responseStatus = Helper.CustomHelper._success, data = result });
            }
            catch(CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch(Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "An unexpected error occurred.", responseStatus = Helper.CustomHelper._failure, data = ex.Message });

            }
        }

        [HttpGet]
        [Route("InvestigationDetails")]
        public async Task<IActionResult> DoGetParticularInvestigationDetails(string identifier)
        {
            try
            {
                if (string.IsNullOrEmpty(identifier))
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500,responseStatus =  Helper.CustomHelper._failure ,responseDatetime = DateTime.Now,responseMessage = "Invalid Request"});
                }

                var data = await _service.DoGetInvestigationDetailsById(identifier);
                return Ok(new CommonResponseDTO { data = data, responseCode = 200, responseMessage = "success", responseDatetime = DateTime.Now, responseStatus = Helper.CustomHelper._success });
            }
            catch(CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseCode = 500, responseMessage = ex.Message });
            }
            catch(Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseCode = 500, responseMessage = ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdateInvestigation")]
        public async Task<IActionResult> DoUpdateInvestigationDetails(InvestigationDTO objdto)
        {
            try
            {
                if(objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseCode = 500, responseMessage = "Invalid Request" });
                }

                await _service.DoUpdateInvestigation(objdto);
                return Ok(new CommonResponseDTO { responseCode = 200,responseDatetime = DateTime.Now,responseStatus = Helper.CustomHelper._success ,responseMessage = "Updated Successfully"});
            }
            catch(CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseCode = 500, responseMessage = ex.Message });
            }
            catch(Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseCode = 500, responseMessage = ex.Message });
            }
        }
    }
}
