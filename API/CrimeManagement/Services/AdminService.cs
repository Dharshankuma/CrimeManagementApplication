using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Services
{
    public interface IAdminService
    {
        Task<AdminGridResponse> DoGetMasterDetails(AdminDTO objdto);
        Task DoUpdateMasterDetails(AdminUpdateDTO objdto);
        Task DoDeleteMasterDetails(AdminUpdateDTO objdto);
        Task<AdminGridResponse> DoGetJurisdictionDetails(AdminDTO objdto);
        Task DoUpdateOfficerJurisdiction(AdminUpdateDTO objdto);

        Task<Data<List<UserDTO>>> DoGetUserDetails(UserGridDTO objdto);

    }
    public class AdminService : IAdminService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CrimeDbContext _db;

        public AdminService(IHttpContextAccessor httpContextAccessor, CrimeDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public async Task<AdminGridResponse> DoGetMasterDetails(AdminDTO objdto)
        {
            try
            {
                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);

                var roleId = await _db.UserMasters.Where(x => x.UserId == userId).Select(x => x.RoleId).FirstOrDefaultAsync();

                if (roleId != 1)
                    throw new CustomException("Not authorized to use the page");

                IQueryable<AdminGridView> query = null;

                switch (objdto.adminType)
                {
                    case 1:
                        query = _db.CrimeTypes.Where(x=>x.IsActive == true).Select(x=>new AdminGridView
                        {
                            name = x.CrimeName,
                            identifier = x.Identifier,
                            status = x.IsActive == true ? 1 : 0
                        });
                        break;
                    case 2:
                        query = _db.Statusmasters.Where(x=>x.IsActive == true).Select(x => new AdminGridView
                        {
                            name = x.Status,
                            identifier = x.Identifier,
                            status = x.IsActive == true ? 1 : 0
                        });
                        break;
                    case 3:
                        query = _db.JurisdictionMasters.Where(x=>x.IsActive == true).Select(x => new AdminGridView
                        {
                            name = x.JurisdictionName,
                            identifier = x.Identifier,
                            status = x.IsActive == true ? 1 : 0
                        });
                        break;
                    default:
                        throw new CustomException("Invalid admin type");
                }
                var totalCount = await query.CountAsync();

                int pageNumber = objdto.PageNumber >= 0 ? objdto.PageNumber : 0; // allow 0 as first page
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var result = await query.ToListAsync();



                var pagedResult = await query
                    .Skip(pageNumber * pageSize)   // 0 → first page, 1 → second page, etc.
                    .Take(pageSize)
                    .ToListAsync();

                return new AdminGridResponse
                {
                    gridDetails = pagedResult,
                    totalCount = totalCount
                };

            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task DoUpdateMasterDetails(AdminUpdateDTO objdto)
        {
            try
            {
                int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value ?? "0");

                var roleId = await _db.UserMasters.Where(x => x.UserId == 1).Select(x => x.RoleId).FirstOrDefaultAsync();

                if (roleId != 1)
                    throw new CustomException("Not authorized to use the page");

                

                switch (objdto.adminType)
                {
                    case 1:
                        var crimeType = await _db.CrimeTypes.Where(x => x.Identifier == objdto.identifier).FirstOrDefaultAsync();
                        if (crimeType == null)
                            throw new CustomException("Record not found");
                        crimeType.CrimeName = objdto.name ?? crimeType.CrimeName;
                        
                        break;
                    case 2:
                        var status = await _db.Statusmasters.Where(x => x.Identifier == objdto.identifier).FirstOrDefaultAsync();
                        if (status == null)
                            throw new CustomException("Record not found");
                        status.Status = objdto.name ?? status.Status;
                        break;
                    case 3:
                        var jurs = await _db.JurisdictionMasters.Where(x => x.Identifier == objdto.identifier).FirstOrDefaultAsync();
                        if (jurs == null)
                            throw new CustomException("Record not found");
                        jurs.JurisdictionName = objdto.name ?? jurs.JurisdictionName;
                        break;
                    default:
                        throw new CustomException("Invalid admin type");
                }

                await _db.SaveChangesAsync();
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

        public async Task DoDeleteMasterDetails(AdminUpdateDTO objdto)
        {
            try
            {
                int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value ?? "0");

                var roleId = await _db.UserMasters.Where(x => x.UserId == 1).Select(x => x.RoleId).FirstOrDefaultAsync();

                if(roleId!= 1)
                    throw new CustomException("Not authorized to use the page");

                switch (objdto.adminType)
                {
                    case 1:
                        var crimeType = await _db.CrimeTypes.Where(x => x.Identifier == objdto.identifier).FirstOrDefaultAsync();
                        if (crimeType == null)
                            throw new CustomException("Record not found");
                        crimeType.IsActive = false;
                        break;
                    case 2:
                        var status = await _db.Statusmasters.Where(x => x.Identifier == objdto.identifier).FirstOrDefaultAsync();
                        if (status == null)
                            throw new CustomException("Record not found");
                        status.IsActive = false;
                        break;
                    case 3:
                        var jurs = await _db.JurisdictionMasters.Where(x => x.Identifier == objdto.identifier).FirstOrDefaultAsync();
                        if (jurs == null)
                            throw new CustomException("Record not found");
                        jurs.IsActive = false;
                        break;

                    default: 
                        throw new CustomException("Invalid admin type");    
                }

                await _db.SaveChangesAsync();
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AdminGridResponse> DoGetJurisdictionDetails(AdminDTO objdto)
        {
            try
            {
                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);

                var roleId = await _db.UserMasters
                    .Where(x => x.UserId == userId)
                    .Select(x => x.RoleId)
                    .FirstOrDefaultAsync();

                if (roleId != 1)
                    throw new CustomException("Not authorized to view officer deployment");

                var query = from officer in _db.UserMasters
                            join assign in _db.IojurisdictionAssigns
                                on officer.UserId equals assign.UserId into assignTemp
                            from assignData in assignTemp.DefaultIfEmpty()

                            join jurisdiction in _db.JurisdictionMasters
                                on assignData.JurisdictionId equals jurisdiction.JurisdictionId into jurisdictionTemp
                            from jurisdictionData in jurisdictionTemp.DefaultIfEmpty()

                            where officer.RoleId == 2
                            select new AdminGridView
                            {
                                name = officer.Firstname + " " + officer.Lastname,
                                
                                jurisdiction = jurisdictionData.JurisdictionName ?? "Unassigned",
                                
                                identifier = officer.Identifier
                            };

                var totalCount = await query.CountAsync();

                int pageNumber = objdto.PageNumber >= 0 ? objdto.PageNumber : 0;
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var pagedResult = await query
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new AdminGridResponse
                {
                    gridDetails = pagedResult,
                    totalCount = totalCount
                };
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DoUpdateOfficerJurisdiction(AdminUpdateDTO objdto)
        {
            try
            {
                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);

                var roleId = await _db.UserMasters
                    .Where(x => x.UserId == userId)
                    .Select(x => x.RoleId)
                    .FirstOrDefaultAsync();

                if (roleId != 1)
                    throw new CustomException("Not authorized to update jurisdiction");

                if (string.IsNullOrWhiteSpace(objdto.userIdentifier) || string.IsNullOrWhiteSpace(objdto.identifier))
                    throw new CustomException("Invalid request data");

                // Get officer
                var officer = await _db.UserMasters
                    .Where(x => x.Identifier == objdto.userIdentifier)
                    .Select(x => new { x.UserId })
                    .FirstOrDefaultAsync();

                if (officer == null)
                    throw new CustomException("Officer not found");

                // Get jurisdiction
                var jurisdiction = await _db.JurisdictionMasters
                    .Where(x => x.Identifier == objdto.identifier)
                    .Select(x => new { x.JurisdictionId })
                    .FirstOrDefaultAsync();

                if (jurisdiction == null)
                    throw new CustomException("Jurisdiction not found");

                // Check existing assignment
                var existingAssignment = await _db.IojurisdictionAssigns
                    .FirstOrDefaultAsync(x => x.UserId == officer.UserId);

                if (existingAssignment != null)
                {
                    existingAssignment.JurisdictionId = jurisdiction.JurisdictionId;
                    existingAssignment.ModifyOn = DateTime.Now;
                    existingAssignment.ModifyBy = userId;

                    _db.IojurisdictionAssigns.Update(existingAssignment);
                }
                else
                {
                    var newAssignment = new IojurisdictionAssign
                    {
                        UserId = officer.UserId,
                        JurisdictionId = jurisdiction.JurisdictionId,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userId
                    };

                    await _db.IojurisdictionAssigns.AddAsync(newAssignment);
                }

                await _db.SaveChangesAsync();
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data<List<UserDTO>>> DoGetUserDetails(UserGridDTO objdto)
        {
            try
            {
                var query = (from user in _db.UserMasters
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
