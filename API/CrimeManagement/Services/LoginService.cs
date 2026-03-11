using Azure;
using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Helper;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Services
{
    public interface ILoginService
    {
        Task DoRegisterUser(RegisterUserDTO objdto);
        Task DoResetPassword(RegisterUserDTO objdto);
        Task<CommonResponseDTO> DoGenerateToken(UserLoginDetails userDetails);
    }
    public class LoginService : ILoginService
    {
        private readonly CrimeDbContext _db;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IConfiguration _config;

        public LoginService(CrimeDbContext db, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IConfiguration config)
        {
            _db = db;
            _environment = environment;
            _config = config;
        }


        public async Task DoRegisterUser(RegisterUserDTO objdto)
        {
            try
            {
                if(objdto != null)
                {
                    bool emailExists = await _db.UserMasters
                               .AnyAsync(u => u.EmailId.ToLower() == objdto.emailId.ToLower());

                    if (emailExists)
                        throw new CustomException("Email Id already exists");

                    var user = new UserMaster
                    {
                        Firstname = objdto.firstName,
                        UserName = objdto.userName,
                        Lastname = objdto.lastName,
                        Aadhaar = objdto.aadhaar,
                        EmailId = objdto.emailId,
                        PhoneNo = objdto.phoneNo,
                        Identifier = Helper.CustomHelper.DoGenerateGuid(),
                        HashPassword = objdto.password != null ? Helper.CustomHelper.HashPassword(objdto.password) : "",
                        Status = "Active",
                        RoleId = 3  //by default the role will get assign as citizen 
                    };

                    await _db.UserMasters.AddAsync(user);
                    await _db.SaveChangesAsync(); 
                }
            }
            catch(CustomException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task DoResetPassword(RegisterUserDTO objdto)
        {
            try
            {
                var userDetails = await _db.UserMasters
                                  .FirstOrDefaultAsync(u => u.UserName.ToLower() == objdto.userName.ToLower());
                if(userDetails == null)
                {
                    throw new CustomException("Email Id does not exist");
                }

                userDetails.HashPassword = CustomHelper.HashPassword(objdto.password);
                _db.SaveChanges();
            }
            catch(CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CommonResponseDTO> DoGenerateToken(UserLoginDetails userDetails)
        {
            var response = new CommonResponseDTO();
            try
            {

                if (userDetails == null)
                    throw new CustomException("User details are required to generate token.");

                // Validate configuration values required for token creation
                var jwtKey = _config?["JWT:Key"];
                var jwtIssuer = _config?["JWT:Issuer"];
                var jwtAudience = _config?["JWT:Audience"];
                var jwtSubject = _config?["JWT:Subject"];
                var jwtExpireMinutesString = _config?["JWT:ExpireMinutes"];

                if (string.IsNullOrWhiteSpace(jwtKey))
                    throw new CustomException("JWT key is not configured.");
                if (string.IsNullOrWhiteSpace(jwtIssuer))
                    throw new CustomException("JWT issuer is not configured.");
                if (string.IsNullOrWhiteSpace(jwtAudience))
                    throw new CustomException("JWT audience is not configured.");

                if (!int.TryParse(jwtExpireMinutesString, out var expiryMinutes))
                    expiryMinutes = 60; // reasonable default

                // Build claims safely (avoid NullReferenceException when properties are null)
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, jwtSubject ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                // Add user specific claims only if values are present (or use empty string)
                claims.Add(new Claim("UserId", userDetails.userId.ToString() ?? string.Empty));
                claims.Add(new Claim("UserIdentifier", userDetails.userIdentifier ?? string.Empty));
                claims.Add(new Claim("UserName", userDetails.UserName ?? string.Empty));
                claims.Add(new Claim("Email", userDetails.EmailId ?? string.Empty));
                claims.Add(new Claim("Roles", userDetails.Role ?? string.Empty));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
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

            }
            catch (CustomException ex)
            {
                response.responseCode = 400;
                response.responseStatus = "failure";
                response.responseMessage = ex.Message;
                
            }
            catch (Exception ex)
            {
                response.responseCode = 500;
                response.responseStatus = "failure";
                response.responseMessage = "An unexpected error occurred.";
                response.data = ex.Message; 
            }

            return response;
        }
    }
}
