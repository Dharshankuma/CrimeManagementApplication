using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Helper;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Services
{
    public interface IUserService
    {
        Task<UserLoginDetails> DoGetLoginUserDetails(string userName);
        Task<Data<List<UserDTO>>> DoGetUserDetails(UserGridDTO objdto);
        Task DoUpdateUserDetails(UserDTO objdto);
        Task DoUpdateUserProfile(UserDTO objdto);
    }
    public class UserService : IUserService
    {
        private readonly CrimeDbContext _db;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(CrimeDbContext db,Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment,IConfiguration config,IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _environment = environment;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserLoginDetails> DoGetLoginUserDetails(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            try
            {
                var userData = await (from user in _db.UserMasters
                                      join roledata in _db.Roles on user.RoleId equals roledata.RoleId into roledatatemp
                                      from role in roledatatemp.DefaultIfEmpty()
                                      join desgdata in _db.DesignationMasters on user.DesignationId equals desgdata.DesignationId into desgdatatemp
                                      from desg in desgdatatemp.DefaultIfEmpty()
                                      where user.UserName.ToLower() == userName.ToLower() || user.EmailId.ToLower() == userName.ToLower() && user.Status == "Active"
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
                                          DesignationId = user.DesignationId,
                                          HashPassword = user.HashPassword,
                                          userIdentifier = user.Identifier,
                                          userId = user.UserId,
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
                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);

                var userDetails = await _db.UserMasters
                    .FirstOrDefaultAsync(x => x.Identifier == objdto.identifier);

                if (userDetails == null)
                    throw new CustomException("User details not found.");

                var jurisdictionTask =await  _db.JurisdictionMasters
                    .Where(x => x.Identifier == objdto.jurisdictionIdentifier)
                    .Select(x => (int?)x.JurisdictionId)
                    .FirstOrDefaultAsync();

                var designationTask = await _db.Roles
                    .Where(x => x.Identifier == objdto.roleIdentifier)
                    .Select(x => (int?)x.RoleId)
                    .FirstOrDefaultAsync();

                //await Task.WhenAll(jurisdictionTask, designationTask);

                if (jurisdictionTask == 0)
                    throw new CustomException("Jurisdiction not found.");
                if (designationTask == 0)
                    throw new CustomException("Designation not found.");

                //userDetails.Firstname = objdto.firstName;
                //userDetails.Lastname = objdto.lastName;
                //userDetails.MiddleName = objdto.middleName;
                //userDetails.Address = objdto.address;
                //userDetails.PhoneNo = objdto.phoneNo;
                //userDetails.EmailId = objdto.emailId;
                //userDetails.Gender = objdto.gender;
                //userDetails.Aadhaar = objdto.aadhaar;
                //userDetails.Dob = objdto.dob;
                //userDetails.UserName = objdto.userName;
                userDetails.ModifyBy = userDetails.UserId;
                userDetails.ModifyOn = DateTime.Now;
                userDetails.Jurisdiction = jurisdictionTask;
                userDetails.RoleId = designationTask;
                //userDetails.Pan = objdto.pan;
                //userDetails.EmergencyContact = objdto.emergencyContact;


                var jurisdictiondetails = await _db.IojurisdictionAssigns.Where(x => x.UserId == userDetails.UserId).FirstOrDefaultAsync();

                if(jurisdictiondetails == null)
                {
                    var jurisdictionData = new IojurisdictionAssign
                    {
                        Identifier = CustomHelper.DoGenerateGuid(),
                        UserId = userDetails.UserId,
                        JurisdictionId = jurisdictionTask,
                        CreatedBy = userId,
                        CreatedOn = CustomHelper.DoGetDateTime()
                    };

                    await _db.AddAsync(jurisdictionData);

                }
                else
                {
                    jurisdictiondetails.JurisdictionId = jurisdictionTask;
                }

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

        public async Task DoUpdateUserProfile(UserDTO objdto)
        {
            if (objdto == null)
                throw new ArgumentNullException(nameof(objdto));

            try
            {
                var userDetails = await _db.UserMasters
                    .FirstOrDefaultAsync(x => x.Identifier == objdto.identifier);

                if (userDetails == null)
                    throw new CustomException("User details not found.");

                // Only mark as modified if values actually change to avoid unnecessary database updates
                var isModified = false;

                if (userDetails.Firstname != objdto.firstName)
                {
                    userDetails.Firstname = objdto.firstName;
                    isModified = true;
                }

                if (userDetails.Lastname != objdto.lastName)
                {
                    userDetails.Lastname = objdto.lastName;
                    isModified = true;
                }

                if (userDetails.MiddleName != objdto.middleName)
                {
                    userDetails.MiddleName = objdto.middleName;
                    isModified = true;
                }

                if (userDetails.Address != objdto.address)
                {
                    userDetails.Address = objdto.address;
                    isModified = true;
                }

                if (userDetails.PhoneNo != objdto.phoneNo)
                {
                    userDetails.PhoneNo = objdto.phoneNo;
                    isModified = true;
                }

                if (userDetails.EmailId != objdto.emailId)
                {
                    userDetails.EmailId = objdto.emailId;
                    isModified = true;
                }

                if (userDetails.Gender != objdto.gender)
                {
                    userDetails.Gender = objdto.gender;
                    isModified = true;
                }

                if (userDetails.Aadhaar != objdto.aadhaar)
                {
                    userDetails.Aadhaar = objdto.aadhaar;
                    isModified = true;
                }

                if (userDetails.Dob != objdto.dob)
                {
                    userDetails.Dob = objdto.dob;
                    isModified = true;
                }

                if (userDetails.UserName != objdto.userName)
                {
                    userDetails.UserName = objdto.userName;
                    isModified = true;
                }

                if (userDetails.Pan != objdto.pan)
                {
                    userDetails.Pan = objdto.pan;
                    isModified = true;
                }

                if (userDetails.EmergencyContact != objdto.emergencyContact)
                {
                    userDetails.EmergencyContact = objdto.emergencyContact;
                    isModified = true;
                }

                if (!isModified)
                    return; // no changes detected; skip database round-trip

                userDetails.ModifyBy = userDetails.UserId;
                userDetails.ModifyOn = DateTime.Now;

                await _db.SaveChangesAsync();
            }
            catch (CustomException) // rethrow known errors untouched
            {
                throw;
            }
            catch (Exception ex) // wrap unknowns with context
            {
                throw ex;
            }
        }


        public async Task<Data<List<UserDTO>>> DoGetUserDetails(UserGridDTO objdto)
        {
            try
            {
                var query =  (from user in _db.UserMasters
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
                                   where user.Status == "Active"
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
                                   });



                var result = await query.ToListAsync();

                var totalCount = result.Count();

                int pageNumber = objdto.PageNumber >= 0 ? objdto.PageNumber : 0; // allow 0 as first page
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var pagedResult = await query
                    .Skip(pageNumber * pageSize)   // 0 → first page, 1 → second page, etc.
                    .Take(pageSize)
                    .ToListAsync();

                return new Data<List<UserDTO>>
                {
                    data = pagedResult,
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
