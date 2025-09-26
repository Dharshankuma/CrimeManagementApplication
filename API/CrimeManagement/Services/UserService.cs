using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Services
{
    public interface IUserService
    {
        Task<UserLoginDetails> DoGetLoginUserDetails(string emailId);
        Task<Data<List<UserDTO>>> DoGetUserDetails(string identifier);
        Task DoUpdateUserDetails(UserDTO objdto);
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
        public async Task<UserLoginDetails> DoGetLoginUserDetails(string emailId)
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

        public async Task DoUpdateUserDetails(UserDTO objdto)
        {
            try
            {
                var userDetails = await _db.UserMasters
                    .FirstOrDefaultAsync(x => x.Identifier == objdto.identifier);

                if (userDetails == null)
                    throw new CustomException("User details not found.");

                var jurisdictionTask = _db.JurisdictionMasters
                    .Where(x => x.Identifier == objdto.jurisdictionIdentifier)
                    .Select(x => (int?)x.JurisdictionId)
                    .FirstOrDefaultAsync();

                var designationTask = _db.DesignationMasters
                    .Where(x => x.Identifier == objdto.designationIdentifier)
                    .Select(x => (int?)x.DesignationId)
                    .FirstOrDefaultAsync();

                await Task.WhenAll(jurisdictionTask, designationTask);

                if (jurisdictionTask.Result == null)
                    throw new CustomException("Jurisdiction not found.");
                if (designationTask.Result == null)
                    throw new CustomException("Designation not found.");

                userDetails.Firstname = objdto.firstName;
                userDetails.Lastname = objdto.lastName;
                userDetails.MiddleName = objdto.middleName;
                userDetails.Address = objdto.address;
                userDetails.PhoneNo = objdto.phoneNo;
                userDetails.EmailId = objdto.emailId;
                userDetails.Gender = objdto.gender;
                userDetails.Aadhaar = objdto.aadhaar;
                userDetails.Dob = objdto.dob;
                userDetails.UserName = objdto.userName;
                userDetails.ModifyBy = userDetails.UserId;
                userDetails.ModifyOn = DateTime.Now;
                userDetails.Jurisdiction = jurisdictionTask.Result.Value;
                userDetails.DesignationId = designationTask.Result.Value;
                userDetails.Pan = objdto.pan;
                userDetails.EmergencyContact = objdto.emergencyContact;

                await _db.SaveChangesAsync();
            }
            catch (CustomException) // rethrow known errors untouched
            {
                throw;
            }
            catch (Exception ex) // wrap unknowns
            {
                throw ex;
            }
        }

        public async Task<Data<List<UserDTO>>> DoGetUserDetails(string Identifier)
        {
            try
            {
                var userDetails = await (from user in _db.UserMasters
                                         join roledata in _db.Roles on user.RoleId equals roledata.RoleId into roledataTemp
                                         from role in roledataTemp.DefaultIfEmpty()
                                         join desdata in _db.DesignationMasters on user.DesignationId equals desdata.DesignationId into desdatatemp
                                         from des in desdatatemp.DefaultIfEmpty()
                                         join jursdata in _db.JurisdictionMasters on user.Jurisdiction equals jursdata.JurisdictionId into jurstemmp
                                         from jurs in jurstemmp.DefaultIfEmpty()
                                         join locdata in _db.LocationMasters on jurs.LocationId equals locdata.LocationId into loctemp
                                         from loc in loctemp.DefaultIfEmpty()
                                         join stdata in _db.StateMasters on loc.StateId equals stdata.StateId into sttemp
                                         from st in sttemp.DefaultIfEmpty()
                                         join cntrydata in _db.CountryMasters on st.CountryId equals cntrydata.CountryId into cntrydtemp
                                         from cntry in cntrydtemp.DefaultIfEmpty()
                                         where user.Identifier == Identifier && user.Status == "Active"
                                         select new UserDTO
                                         {
                                             identifier = user.Identifier,
                                             userName = user.UserName,
                                             firstName = user.Firstname,
                                             lastName = user.Lastname,
                                             middleName = user.MiddleName,
                                             aadhaar = user.Aadhaar,
                                             pan = user.Pan,
                                             emailId = user.EmailId,
                                             phoneNo = user.PhoneNo,
                                             emergencyContact = user.EmergencyContact,
                                             status = user.Status,
                                             gender = user.Gender,
                                             roleName = role.RoleName,
                                             jurisdictionIdentifier = jurs.Identifier,
                                             designationIdentifier = des.Identifier,
                                             stateIdentifer = st.Identifier,
                                             countryIdentifier = cntry.Identifier,
                                             locationIdentifier = loc.Identifier,
                                             dob = user.Dob
                                         }).ToListAsync();

                var totalCount = userDetails.Count();



                return new Data<List<UserDTO>>
                {
                    data = userDetails,
                    totalCount = totalCount,
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
