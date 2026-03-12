using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IConfiguration _config;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environement;
        private readonly IAdminService _adminService;

        public AdminController(IConfiguration config, Microsoft.AspNetCore.Hosting.IHostingEnvironment environement, IAdminService adminService)
        {
            _config = config;
            _environement = environement;
            _adminService = adminService;
        }




        [HttpGet]
        [Route("GetMasterDetails")]
        public async Task<IActionResult> DoGetMasterDetails(AdminDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._failure });
                }

                await _adminService.DoGetMasterDetails(objdto);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "success", responseStatus = Helper.CustomHelper._success });
            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 501, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 501, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
        }

        [HttpPost]
        [Route("UpdateMasterDetails")]
        public async Task<IActionResult> DoUpdateMasterDetails(AdminUpdateDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._failure });
                }
                await _adminService.DoUpdateMasterDetails(objdto);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "success", responseStatus = Helper.CustomHelper._success });
            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 501, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 501, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
        }

        [HttpDelete]
        [Route("DeleteMasterDetails")]
        public async Task<IActionResult> DoDeleteMasterDetails(AdminUpdateDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._failure });
                }
                await _adminService.DoDeleteMasterDetails(objdto);
                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "success", responseStatus = Helper.CustomHelper._success });
            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 501, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 501, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
        }
    }
}