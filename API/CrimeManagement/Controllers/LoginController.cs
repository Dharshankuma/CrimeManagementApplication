using CrimeManagement.DTO;
using CrimeManagement.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        public LoginController(IConfiguration config, Microsoft.AspNetCore.Hosting.IHostingEnvironment environement,IUserService userService)
        {
            _config = config;
            _environement = environement;
            _userService = userService;
        }


        [HttpPost]
        [Route("LoginUser")]
        public async Task<CrimeResponseDTO<LoginDetails>> DoCheckLoginUser(LoginDTO objdto)
        {
            var response = new CrimeResponseDTO<LoginDetails>
            {
                responseDatetime = DateTime.UtcNow
            };

            try
            {
                if (string.IsNullOrWhiteSpace(objdto.emailId) || string.IsNullOrWhiteSpace(objdto.password))
                {
                    response.responseStatus = "failure";
                    response.responseDescription = "Email Id or Password is missing";
                    return response;
                }

                var userDetails = await DoGetUserData(objdto.emailId);
                if (userDetails == null)
                {
                    response.responseStatus = "failure";
                    response.responseDescription = "User does not exist";
                    return response;
                }

                // Replace this with proper hash comparison in production

                string decryptPassword = CustomHelper.CustomHelper.Decrypt(userDetails.HashPassword);
                if (!objdto.password.Trim().Equals(decryptPassword, StringComparison.Ordinal))
                {
                    response.responseStatus = "failure";
                    response.responseDescription = "Incorrect Password";
                    return response;
                }

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
                response.responseDescription = "Login successful";
                response.data = loginDetails;

                return response;
            }
            catch (Exception ex)
            {
                response.responseCode = 500;
                response.responseStatus = "failure";
                response.responseDescription = ex.Message;
                return response;
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





        private async Task<UserLoginDetails> DoGetUserData(string emailId)
        {
            var userDetails = await _userService.DoGetUserDetails(emailId);
            return userDetails;
        }
    }
}
