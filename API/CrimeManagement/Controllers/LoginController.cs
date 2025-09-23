using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CrimeManagement")]
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environement;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        public LoginController(IConfiguration config, Microsoft.AspNetCore.Hosting.IHostingEnvironment environement,IUserService userService, ILoginService loginService)
        {
            _config = config;
            _environement = environement;
            _userService = userService;
            _loginService = loginService;
        }


        [HttpPost]
        [Route("LoginUser")]
        public async Task<IActionResult> DoCheckLoginUser(LoginDTO objdto)
        {
            var response = new CommonResponseDTO
            {
                responseDatetime = DateTime.UtcNow
            };

            try
            {
                if (string.IsNullOrWhiteSpace(objdto.emailId) || string.IsNullOrWhiteSpace(objdto.password))
                    throw new CustomException("Email Id or Password is missing");

                var userDetails = await DoGetUserData(objdto.emailId);
                if (userDetails == null)
                    throw new CustomException("User does not exist");

                string decryptPassword = CustomHelper.CustomHelper.Decrypt(userDetails.HashPassword);
                if (!objdto.password.Trim().Equals(decryptPassword, StringComparison.Ordinal))
                    throw new CustomException("Incorrect Password");

                // ----- Create JWT -----
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, _config["JWT:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("Id", userDetails.userIdentifier.ToString()),
            new Claim("Name", $"{userDetails.Firstname} {userDetails.Lastname}"),
            new Claim("Email", userDetails.EmailId)
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiryMinutes = int.Parse(_config["JWT:ExpireMinutes"]);
                var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:Issuer"],
                    audience: _config["JWT:Audience"],
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: creds
                );

                var authToken = new JwtSecurityTokenHandler().WriteToken(token);

                var loginDetails = new LoginDetails
                {
                    userDetails = userDetails,
                    token = authToken,
                    expiryTime = expiryMinutes
                };

                response.responseCode = 200;
                response.responseStatus = "success";
                response.responseMessage = "Login successful";
                response.data = loginDetails;

                return Ok(response);
            }
            catch (CustomException ex)
            {
                response.responseCode = 400;
                response.responseStatus = "failure";
                response.responseMessage = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.responseCode = 500;
                response.responseStatus = "failure";
                response.responseMessage = "An unexpected error occurred.";
                response.data = ex.Message; // optional: remove in production
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Route("GenerateKey")]
        public async Task<IActionResult> DoGenereateKey()
        {
            try
            {
                var key = CustomHelper.CustomHelper.GenerateRandomKey(31);
                return Ok(new { status = "success" ,key = Convert.ToBase64String(key)});
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> DoRegisterUser(RegisterUserDTO objdto)
        {
            try
            {
                if(objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request" });
                }

                await _loginService.DoRegisterUser(objdto);

                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "User Registered Successfully" });
            }
            catch(CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message });

            }
            catch(Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message });
            }
        }




        private async Task<UserLoginDetails> DoGetUserData(string emailId)
        {
            var userDetails = await _userService.DoGetLoginUserDetails(emailId);
            return userDetails;
        }
    }
}
