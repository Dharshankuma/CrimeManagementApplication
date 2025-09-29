using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CrimeManagement")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("GetUser")]
        public async Task<IActionResult> DoGetUserDetails(UserGridDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseStatus = Helper.CustomHelper._failure,responseMessage = "Identifier is required", responseCode = 400, responseDatetime = DateTime.Now });
                }

                var data = await _userService.DoGetUserDetails(objdto);

                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now,responseStatus = Helper.CustomHelper._success,data = data});
            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._success });
            }
        }
        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> DoUpdateDetails(UserDTO objdto)
        {
            try
            {
                if(objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 400, responseDatetime = DateTime.Now, responseMessage = "Invalid request", responseStatus = "failure" });
                }

                await _userService.DoUpdateUserDetails(objdto);

                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "updated successfully", responseStatus = "success" });
            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = "failure" });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = "failure" });
            }
        }
    }
}
