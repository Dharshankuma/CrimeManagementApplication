using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _service;

        public ConfigurationController(IConfigurationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetConfiguration")]
        public async Task<IActionResult> DoGetConfigurationDetails()
        {
            try
            {
                var data = await _service.DoGetConfigurationDetails();
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseStatus = Helper.CustomHelper._success ,data = data});
            }
            catch(CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseMessage = ex.Message });
            }
            catch(Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseStatus = Helper.CustomHelper._failure, responseDatetime = DateTime.Now, responseMessage = ex.Message });
            }
        }
    }
}
