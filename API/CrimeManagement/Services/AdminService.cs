using CrimeManagement.Context;
using CrimeManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface IAdminService
    {
        Task<AdminGridResponse> DoGetMasterDetails(AdminDTO objdto);
        Task DoUpdateMasterDetails(AdminUpdateDTO objdto);
        Task DoDeleteMasterDetails(AdminUpdateDTO objdto);

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
                        query = _db.CrimeTypes.Select(x=>new AdminGridView
                        {
                            name = x.CrimeName,
                            identifier = x.Identifier,
                        });
                        break;
                    case 2:
                        query = _db.Statusmasters.Select(x => new AdminGridView
                        {
                            name = x.Status,
                            identifier = x.Identifier,
                        });
                        break;
                    case 3:
                        query = _db.JurisdictionMasters.Select(x => new AdminGridView
                        {
                            name = x.JurisdictionName,
                            identifier = x.Identifier
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
    }
}
