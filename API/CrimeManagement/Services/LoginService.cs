using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface ILoginService
    {
        Task DoRegisterUser(RegisterUserDTO objdto);
        Task DoResetPassword(RegisterUserDTO objdto);
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
                        HashPassword = Helper.CustomHelper.Encrypt(objdto.password)
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
                                  .FirstOrDefaultAsync(u => u.EmailId.ToLower() == objdto.emailId.ToLower());
                if(userDetails == null)
                {
                    throw new CustomException("Email Id does not exist");
                }

                userDetails.HashPassword = Helper.CustomHelper.Encrypt(objdto.password);
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
    }
}
