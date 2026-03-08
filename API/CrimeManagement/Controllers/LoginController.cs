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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using CrimeManagement.Helper;


namespace CrimeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
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
                if (string.IsNullOrWhiteSpace(objdto.userName) || string.IsNullOrWhiteSpace(objdto.password))
                    throw new CustomException("Username or Password is missing");

                var userDetails = await DoGetUserData(objdto.userName);
                if (userDetails == null)
                    throw new CustomException("User does not exist");

                
                if (!CustomHelper.VerifyPassword(objdto.password,userDetails.HashPassword))
                    throw new CustomException("Incorrect Password");


                response = await _loginService.DoGenerateToken(userDetails);
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
                response.data = ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle(LoginGoogleDTO dto)
        {
            var response = new CommonResponseDTO
            {
                responseDatetime = DateTime.UtcNow
            };

            try
            {
                if (string.IsNullOrWhiteSpace(dto.googleToken))
                    throw new CustomException("Google token is missing");

                // 1️⃣ Verify Google token
                var payload = await Google.Apis.Auth.GoogleJsonWebSignature
                    .ValidateAsync(dto.googleToken);

                var email = payload.Email;
                var name = payload.Name;
                var googleId = payload.Subject;

                // 2️⃣ Find user by email
                var userDetails = await _userService.DoGetLoginUserDetails(email);

                // 3️⃣ Auto-register if new user
                if (userDetails == null)
                {
                    var registerDto = new RegisterUserDTO
                    {
                        userName = name,
                        emailId = email,
                        authProvider = "Google",
                        externalProviderId = googleId
                    };

                    await _loginService.DoRegisterUser(registerDto);

                    userDetails = await _userService.DoGetLoginUserDetails(email);
                }

                // 4️⃣ Issue JWT (same as username/password)
                response = await _loginService.DoGenerateToken(userDetails);
                return Ok(response);
            }
            catch (CustomException ex)
            {
                response.responseCode = 400;
                response.responseStatus = CustomHelper._failure;
                response.responseMessage = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.responseCode = 500;
                response.responseStatus = CustomHelper._failure;
                response.responseMessage = "An unexpected error occurred.";
                response.data = ex.Message;
                return StatusCode(500, response);
            }
        }

        //[HttpGet]
        //[Route("LoginWithGoogle")]
        //public IActionResult DoLoginUserWithGoogle()
        //{
        //    var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };

        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        //[HttpGet]
        //[Route("GoogleResponse")]
        //public async Task<IActionResult> DoGoogleCallBackResponse()
        //{
        //    var response = new CommonResponseDTO();
        //    response.responseDatetime = CustomHelper.DoGetDateTime();
        //    try
        //    {
        //        var result = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

        //        if (!result.Succeeded)
        //        {
        //            throw new CustomException("Google Authentication failed");
        //        }

        //        var claims = result.Principal.Claims;

        //        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        //        var user = claims.First(c => c.Type == ClaimTypes.Name)?.Value;
        //        var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //        if (string.IsNullOrWhiteSpace(email))
        //        {
        //            throw new CustomException("Email not received from Google");
        //        }

        //        var userDetails = await _userService.DoGetLoginUserDetails(email);

        //        if(userDetails == null)
        //        {
        //            var userData = new RegisterUserDTO
        //            {
        //                userName = user,
        //                emailId = email,
        //                authProvider = "Google",
        //                externalProviderId = googleId
        //            };
        //            await _loginService.DoRegisterUser(userData);
        //            response.responseStatus = CustomHelper._success;
        //            response.responseCode = 200;
        //            response.responseMessage = "User registered successfully with Google";
        //        }

        //        return Ok(response);

        //    }
        //    catch (CustomException ex)
        //    {
        //        response.responseCode = 400;
        //        response.responseStatus = CustomHelper._failure;
        //        response.responseMessage = ex.Message;
        //        return BadRequest(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.responseCode = 500;
        //        response.responseStatus = CustomHelper._failure;
        //        response.responseMessage = "An unexpected error occurred.";
        //        response.data = ex.Message; // optional: remove in production
        //        return StatusCode(500, response);
        //    }
        //}

        [HttpGet]
        [Route("GenerateKey")]
        public async Task<IActionResult> DoGenereateKey()
        {
            try
            {
                var key = Helper.CustomHelper.GenerateRandomKey(31);
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
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request" ,responseStatus = Helper.CustomHelper._failure});
                }

                await _loginService.DoRegisterUser(objdto);

                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "User Registered Successfully",responseStatus = Helper.CustomHelper._success});
            }
            catch(CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message,responseStatus = Helper.CustomHelper._failure });

            }
            catch(Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message,responseStatus = Helper.CustomHelper._failure });
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> DoResetPassword(RegisterUserDTO objdto)
        {
            try
            {
                if (objdto == null)
                {
                    return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = "Invalid Request", responseStatus = Helper.CustomHelper._failure });
                }

                await _loginService.DoResetPassword(objdto);

                return Ok(new CommonResponseDTO { responseCode = 200, responseDatetime = DateTime.Now, responseMessage = "Resetted password", responseStatus = Helper.CustomHelper._success });

            }
            catch (CustomException ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
            catch (Exception ex)
            {
                return BadRequest(new CommonResponseDTO { responseCode = 500, responseDatetime = DateTime.Now, responseMessage = ex.Message, responseStatus = Helper.CustomHelper._failure });
            }
        }
        private async Task<UserLoginDetails> DoGetUserData(string userName)
        {
            var userDetails = await _userService.DoGetLoginUserDetails(userName);
            return userDetails;
        }

        [HttpGet]
        public IActionResult Throw()
        {
            throw new Exception("Test exception in crime module");
        }
    }
}
