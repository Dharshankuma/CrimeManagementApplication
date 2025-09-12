using CrimeManagement.Context;
using CrimeManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface IUserService
    {
        Task<UserLoginDetails> DoGetUserDetails(string emailId);
    }
    public class UserService : IUserService
    {
        private readonly CrimeDbContext _db;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IConfiguration _config;

        public UserService(CrimeDbContext db,Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment,IConfiguration config)
        {
            _db = db;
            _environment = environment;
            _config = config;
        }
        public async Task<UserLoginDetails> DoGetUserDetails(string emailId)
        {
            if (string.IsNullOrWhiteSpace(emailId))
                return null;

            try
            {
                var userData = await (from user in _db.UserMasters
                                      join roledata in _db.Roles on user.RoleId equals roledata.RoleId into roledatatemp
                                      from role in roledatatemp
                                      join desgdata in _db.DesignationMasters on user.DesignationId equals desgdata.DesignationId into desgdatatemp
                                      from desg in desgdatatemp
                                      where user.EmailId.ToLower() == emailId.ToLower() && user.Status == "Active"
                                      select new UserLoginDetails
                                      {
                                          UserName = user.UserName,
                                          Firstname = user.Firstname,
                                          Lastname = user.Lastname,
                                          MiddleName = user.MiddleName,
                                          Designation = desg.DesignationName,
                                          Role = role.RoleName,
                                          EmailId = user.EmailId,
                                          Gender = user.Gender,
                                          ProfilePhotoPath = user.ProfilePhotoPath,
                                          Jurisdiction = user.Jurisdiction,
                                          RoleId = user.RoleId,
                                          DesignationId = user.DesignationId
                                      }).FirstOrDefaultAsync();

                return userData;
            }
            catch
            {
                throw;
            }
        }
    }
}
