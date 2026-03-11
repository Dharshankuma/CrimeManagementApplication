using CrimeManagement.Context;
using CrimeManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface IAdminService
    {
        Task<AdminGridResponse> DoGetMasterDetails(AdminDTO objdto);
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
    }
}
