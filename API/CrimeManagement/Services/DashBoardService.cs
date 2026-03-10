using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Helper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CrimeManagement.Services
{
    public interface IDashBoardService
    {
        Task<DashBoardDTO> DoGetCrimeDashBoardContent();
    }
    public class DashBoardService : IDashBoardService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CrimeDbContext _db;
        public DashBoardService(IHttpContextAccessor httpContextAccessor,CrimeDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }
        public async Task<DashBoardDTO> DoGetCrimeDashBoardContent()
        {
            try
            {
                var dto = new DashBoardDTO();

                dto.totalComplaints = await _db.ComplaintRequests.CountAsync();
                dto.underInvestigation = await _db.ComplaintRequests.Where(x => x.StatusId == 2).CountAsync();
                dto.evidenceCollected = await _db.EvidenceAttachments.CountAsync();
                dto.resolvedCases = await _db.ComplaintRequests.Where(x => x.StatusId == 3).CountAsync();

                var now = DateTime.Now;
                var previousMonthDate = now.AddMonths(-1);

                var currentMonthCases = await _db.ComplaintRequests.CountAsync(x =>
                    x.CreatedOn.HasValue &&
                    x.CreatedOn.Value.Month == now.Month &&
                    x.CreatedOn.Value.Year == now.Year);

                var previousMonthCases = await _db.ComplaintRequests.CountAsync(x =>
                    x.CreatedOn.HasValue &&
                    x.CreatedOn.Value.Month == previousMonthDate.Month &&
                    x.CreatedOn.Value.Year == previousMonthDate.Year);

                if (previousMonthCases > 0)
                {
                    dto.overAllStatus =
                        ((double)(currentMonthCases - previousMonthCases) / previousMonthCases) * 100;
                }
                else
                {
                    dto.overAllStatus = 0;
                }

                foreach (var claim in _httpContextAccessor.HttpContext.User.Claims)
                {
                    Console.WriteLine($"{claim.Type} : {claim.Value}");
                }

                int.TryParse(
             _httpContextAccessor.HttpContext?.User
                 .FindFirst("UserId")?.Value,
             out int userId); 
                
                if (userId > 0)
                {
                    var roleId = await _db.UserMasters.Where(x => x.UserId == userId).Select(x => x.RoleId).FirstOrDefaultAsync();

                    var query = from cr in _db.ComplaintRequests
                                join crmtypedata in _db.CrimeTypes on cr.CrimeTypeId equals crmtypedata.CrimeId into crmtypedatatemp
                                from crmtype in crmtypedatatemp.DefaultIfEmpty()
                                join usrdata in _db.UserMasters on cr.IoofficerId equals usrdata.UserId into usrdatatemp
                                from usr in usrdatatemp.DefaultIfEmpty()
                                select new
                                {
                                    cr.Identifier,
                                    CrimeType = crmtype.CrimeName,
                                    OfficerName = usr.Firstname + " " + usr.Lastname,
                                    LastModified = cr.ModifyOn ?? cr.CreatedOn,
                                    cr.IoofficerId,
                                    cr.CreatedBy
                                };

                    if (roleId == 1)
                    {
                        query = query;
                    }
                    else if (roleId == 2)
                    {
                        query = query.Where(x => x.IoofficerId == userId);
                    }
                    else if (roleId == 3)
                    {
                        query = query.Where(x => x.CreatedBy == userId);
                    }

                    var recentData = await query.OrderByDescending(x => x.LastModified).Take(5).ToListAsync();

                    dto.recentsComplaints = recentData.Select(x=>new DashBoardCrimeReportDTO
                    {
                        crimeId = x.Identifier,
                        crimeType = x.CrimeType,
                        ioOfficerName = x.OfficerName,
                        lastUpdated = x.LastModified.HasValue ? CustomHelper.DoGetRelativeDateTime(x.LastModified.Value) : ""
                    }).ToList();

                }
                else
                {
                    dto.recentsComplaints = null;
                }

                return dto;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
