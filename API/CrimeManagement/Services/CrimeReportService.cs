using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using static CrimeManagement.DTO.CrimeResponseDTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CrimeManagement.Services
{
    public interface ICrimeReportService
    {
        Task<Data<List<CrimeReportViewDTO>>> DoGetCrimeReportDetails(CrimeRequestviewDTO objdto);
        Task DoRaiseCrimeReport(CrimeReportDTO objdto);

        Task<CrimeReportDTO> DoGetCrimeReportDetailsById(string identifer);
    }
    public class CrimeReportService : ICrimeReportService 
    {
        private readonly CrimeDbContext _db;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        public CrimeReportService(CrimeDbContext db, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IConfiguration config)
        {
            _db = db;
            _environment = environment;
            _config = config;
        }

        public async Task<Data<List<CrimeReportViewDTO>>> DoGetCrimeReportDetails(CrimeRequestviewDTO objdto)
        {
            try
            {
                var userId = await _db.UserMasters
                                      .Where(u => u.Identifier == objdto.userIdentifier)
                                      .Select(u => u.UserId)
                                      .FirstOrDefaultAsync();
                if (userId == 0)
                    throw new CustomException("User does not exist");

                var roleId = await _db.UserMasters
                                      .Where(u => u.UserId == userId)
                                      .Select(u => u.RoleId)
                                      .FirstOrDefaultAsync();       //1 - admin , 2 - officer , 3 - user

                var query = from crime in _db.ComplaintRequests
                            join userdata in _db.UserMasters on crime.CreatedBy equals userdata.UserId into usertemp
                            from user in usertemp.DefaultIfEmpty()
                            join jurisdictiondata in _db.JurisdictionMasters on crime.JurisdictionId equals jurisdictiondata.JurisdictionId into jurisdictiontemp
                            from jurisdiction in jurisdictiontemp.DefaultIfEmpty()
                            join crimeTypedata in _db.CrimeTypes on crime.CrimeTypeId equals crimeTypedata.CrimeId into crimeTypetemp
                            from crimeType in crimeTypetemp.DefaultIfEmpty()
                            join stsdata in _db.Statusmasters on crime.StatusId equals stsdata.Statusid into stsdatatemp
                            from sts in stsdatatemp.DefaultIfEmpty()
                            where (roleId == 1 || crime.CreatedBy == userId)
                            select new CrimeReportViewDTO
                            {
                                reportIdentifer = crime.Identifier,
                                complaintName = crime.ComplaintName,
                                jurisdictionName = jurisdiction.JurisdictionName,
                                crimeType = crimeType.CrimeName,
                                crimeStatus = Convert.ToString(crime.StatusId),
                                crimeStatusStr = sts.Status,
                                crimeReportDate = crime.DateReported.HasValue
                                                 ? crime.DateReported.Value.ToString("dd-MM-yyyy")
                                                 : string.Empty
                            };

                // Apply sorting
                if (!string.IsNullOrEmpty(objdto.columnName))
                {
                    bool ascending = objdto.sortOrder ?? true; // default ascending
                    query = objdto.columnName.ToLower() switch
                    {
                        "reportidentifier" => ascending ? query.OrderBy(q => q.reportIdentifer) : query.OrderByDescending(q => q.reportIdentifer),
                        "complaintname" => ascending ? query.OrderBy(q => q.complaintName) : query.OrderByDescending(q => q.complaintName),
                        "jurisdictionname" => ascending ? query.OrderBy(q => q.jurisdictionName) : query.OrderByDescending(q => q.jurisdictionName),
                        "crimetype" => ascending ? query.OrderBy(q => q.crimeType) : query.OrderByDescending(q => q.crimeType),
                        "crimestatus" => ascending ? query.OrderBy(q => q.crimeStatus) : query.OrderByDescending(q => q.crimeStatus),
                        "crimereportdate" => ascending ? query.OrderBy(q => q.crimeReportDate) : query.OrderByDescending(q => q.crimeReportDate),
                        _ => query
                    };
                }

                var result = await query.ToListAsync();

                var totalCount = result.Count();

                int pageNumber = objdto.PageNumber >= 0 ? objdto.PageNumber : 0; // allow 0 as first page
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var pagedResult = await query
                    .Skip(pageNumber * pageSize)   // 0 → first page, 1 → second page, etc.
                    .Take(pageSize)
                    .ToListAsync();

                return new Data<List<CrimeReportViewDTO>>
                {
                    data = pagedResult,
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

        public async Task DoRaiseCrimeReport(CrimeReportDTO objdto)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Validate user
                var userId = await _db.UserMasters
                    .Where(u => u.Identifier == objdto.userIdentifier)
                    .Select(u => u.UserId)
                    .FirstOrDefaultAsync();

                if (userId == 0)
                    throw new CustomException("User does not exist");

                // Validate jurisdiction
                var jurisdictionId = await _db.JurisdictionMasters
                    .Where(j => j.Identifier == objdto.jurisdictionIdentifier)
                    .Select(j => j.JurisdictionId)
                    .FirstOrDefaultAsync();

                if (jurisdictionId == 0)
                    throw new CustomException("Invalid jurisdiction");

                // Validate crime type
                var crimeTypeId = await _db.CrimeTypes
                    .Where(c => c.Identifier == objdto.crimeTypeIdentifier)
                    .Select(c => c.CrimeId)
                    .FirstOrDefaultAsync();

                if (crimeTypeId == 0)
                    throw new CustomException("Invalid crime type");

                // Create complaint
                ComplaintRequest complaint = new ComplaintRequest
                {
                    Identifier = Helper.CustomHelper.DoGenerateGuid(),
                    ComplaintName = objdto.ComplaintName,
                    JurisdictionId = jurisdictionId,
                    CrimeTypeId = crimeTypeId,
                    CrimeDescription = objdto.CrimeDescription,
                    PhoneNumber = objdto.PhoneNumber,
                    DateReported = !string.IsNullOrEmpty(objdto.dateReportString)
                                    ? Helper.CustomHelper.ParseDate(objdto.dateReportString)
                                    : null,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    StatusId = await GetStatusIdByName("Under Investigation")
                };

                await _db.ComplaintRequests.AddAsync(complaint);
                await _db.SaveChangesAsync();

                // Allocate IO and create investigation
                await AllocateIoOfficerandCreateInvestigation(complaint);

                await transaction.CommitAsync();
            }
            catch (CustomException ex)
            {
                await transaction.RollbackAsync();
                // log custom exception
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // log exception
                throw;
            }
        }

        public async Task<CrimeReportDTO> DoGetCrimeReportDetailsById(string identifer)
        {
            try
            {
                var data = await (from cmp in _db.ComplaintRequests
                                  join jurisdiction in _db.JurisdictionMasters on cmp.JurisdictionId equals jurisdiction.JurisdictionId into jurisdictiontemp
                                  from jurisdictiondata in jurisdictiontemp.DefaultIfEmpty()
                                  join crimeType in _db.CrimeTypes on cmp.CrimeTypeId equals crimeType.CrimeId into crimeTypetemp
                                  from crimeTypedata in crimeTypetemp.DefaultIfEmpty()
                                  join user in _db.UserMasters on cmp.CreatedBy equals user.UserId into usertemp
                                  from userdata in usertemp.DefaultIfEmpty()
                                  join invdata in _db.Investigations on cmp.InvestigationId equals invdata.InvestigationId into invdatatemp
                                  from inv in invdatatemp.DefaultIfEmpty()
                                  join invuserdata in _db.UserMasters on inv.IoOfficerId equals invuserdata.UserId into invusertemp
                                  from invuser in invusertemp.DefaultIfEmpty()
                                  join stdata in _db.Statusmasters on cmp.StatusId equals stdata.Statusid into stdatatemp
                                  from sts in stdatatemp.DefaultIfEmpty()
                                  where cmp.Identifier == identifer
                                  select new CrimeReportDTO
                                  {
                                      ComplaintName = cmp.ComplaintName,
                                      jurisdictionIdentifier = jurisdictiondata.Identifier,
                                      jurisdictionName = jurisdictiondata.JurisdictionName,
                                      crimeTypeIdentifier = crimeTypedata.Identifier,
                                      crimeTypeName = crimeTypedata.CrimeName,
                                      CrimeDescription = cmp.CrimeDescription,
                                      PhoneNumber = cmp.PhoneNumber,
                                      dateReportString = cmp.DateReported.HasValue ? cmp.DateReported.Value.ToString("dd-MM-yyyy") : string.Empty,
                                      userIdentifier = userdata.Identifier,
                                      investigationDescription = inv.InvestigationDescription,
                                      startDateString = inv.StartDate.HasValue ? inv.StartDate.Value.ToString("dd-MM-yyyy") : string.Empty,
                                      endDateString = inv.EndDate.HasValue? inv.EndDate.Value.ToString("dd-MM-yyyy") : string.Empty,
                                      victimName = userdata.UserName,
                                      ioOfficerName = invuser.UserName,
                                      statusName = sts.Status, 
                                  }).FirstOrDefaultAsync();

                return data;
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

        private async Task AllocateIoOfficerandCreateInvestigation(ComplaintRequest complaint)
        {
            // Find IO officer for jurisdiction
            //var ioOfficer = await _db.IojurisdictionAssigns
            //    .Where(ioj => ioj.JurisdictionId == complaint.JurisdictionId)
            //    .Join(_db.UserMasters,
            //          ioj => ioj.UserId,
            //          um => um.UserId,
            //          (ioj, um) => um)
            //    .FirstOrDefaultAsync();

            var ioOfficer = await _db.IojurisdictionAssigns.Where(x => x.JurisdictionId == complaint.JurisdictionId).Select(x=>x.UserId).FirstOrDefaultAsync();

            if (ioOfficer == null)
                throw new CustomException("No IO Officer found for the given jurisdiction");

            // Create investigation
            Investigation investigation = new Investigation
            {
                Identifier = Helper.CustomHelper.DoGenerateGuid(),
                ComplaintId = complaint.ComplaintRequestId,
                IoOfficerId = ioOfficer,
                StartDate = DateTime.Now,
                Priority = "Medium", // default
                StatusId = await GetStatusIdByName("Under Investigation"),
                CreatedBy = complaint.CreatedBy,
                CreatedOn = DateTime.Now
            };

            await _db.Investigations.AddAsync(investigation);
            await _db.SaveChangesAsync();

            // Update complaint with IO & Investigation info
            complaint.IoofficerId = ioOfficer;
            complaint.InvestigationId = investigation.InvestigationId;
            complaint.ModifyBy = complaint.CreatedBy;
            complaint.ModifyOn = DateTime.Now;

            _db.ComplaintRequests.Update(complaint);
            await _db.SaveChangesAsync();

            // Create Complaint-IO mapping
            ComplaintIoassign complaintIoAssign = new ComplaintIoassign
            {
                Identifier = Helper.CustomHelper.DoGenerateGuid(),
                ComplaintId = complaint.ComplaintRequestId,
                UserId = ioOfficer,
                ComplaintStatus = complaint.StatusId,
                CreatedBy = complaint.CreatedBy,
                CreatedOn = DateTime.Now
            };

            await _db.ComplaintIoassigns.AddAsync(complaintIoAssign);
            await _db.SaveChangesAsync();

            // Send notification
            //await SendNotification(ioOfficer.UserId,"New Complaint Assigned",$"A new complaint (ID: {complaint.Identifier}) has been assigned to you for investigation.");
        }

        private async Task<int> GetStatusIdByName(string statusName)
        {
            var status = await _db.Statusmasters.FirstOrDefaultAsync(s => s.Status == statusName);
            if (status != null)
                return status.Statusid;

            return statusName switch
            {
                "Open" => 1,
                "Under Investigation" => 2,
                "Resolved" => 3,
                _ => throw new CustomException($"Unknown status name: {statusName}")
            };
        }

        private async Task SendNotification(int userId, string title, string message)
        {
            Notification newNotification = new Notification
            {
                Identifier = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = "Complaint Assignment",
                IsRead = false,
                CreatedOn = DateTime.Now,
                CreatedBy = 1 // configure system user ID
            };

            await _db.Notifications.AddAsync(newNotification);
            await _db.SaveChangesAsync();
        }

    }
}
