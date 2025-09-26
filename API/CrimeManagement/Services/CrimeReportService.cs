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
                            where (roleId == 1 || crime.CreatedBy == userId) &&
                                  (string.IsNullOrEmpty(objdto.crimeType) || crimeType.CrimeName == objdto.crimeType) &&
                                  (string.IsNullOrEmpty(objdto.crimeIdentifier) || crime.Identifier == objdto.crimeIdentifier) &&
                                  (string.IsNullOrEmpty(objdto.reportDate) ||
                                   EF.Functions.DateDiffDay(crime.DateReported, DateTime.Parse(objdto.reportDate)) == 0)
                            select new CrimeReportViewDTO
                            {
                                reportIdentifer = crime.Identifier,
                                complaintName = crime.ComplaintName,
                                jurisdictionName = jurisdiction.JurisdictionName,
                                crimeType = crimeType.CrimeName,
                                crimeStatus = Convert.ToString(crime.StatusId),
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

                var crimeReports = await query.ToListAsync();

                return new Data<List<CrimeReportViewDTO>>
                {
                    data = crimeReports
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
            try
            {

                var userId = await _db.UserMasters.Where(u => u.Identifier == objdto.userIdentifier).Select(u => u.UserId).FirstOrDefaultAsync();

                if(userId == null)
                {
                    throw new CustomException("User does not exisit");
                }

                var jurisdictionId = await _db.JurisdictionMasters
                                           .Where(j => j.Identifier == objdto.jurisdictionIdentifier)
                                           .Select(j => j.JurisdictionId)
                                           .FirstOrDefaultAsync();

                var crimeTypeId = await _db.CrimeTypes
                                           .Where(c => c.Identifier == objdto.crimeTypeIdentifier)
                                           .Select(c => c.CrimeId)
                                           .FirstOrDefaultAsync();

                ComplaintRequest complaint = new ComplaintRequest
                {
                    Identifier = Helper.CustomHelper.DoGenerateGuid(),
                    ComplaintName = objdto.ComplaintName,
                    JurisdictionId = jurisdictionId,
                    CrimeTypeId = crimeTypeId,
                    CrimeDescription = objdto.CrimeDescription,
                    PhoneNumber = objdto.PhoneNumber,
                    DateReported = objdto.dateReportString != null ? DateTime.Parse(objdto.dateReportString) : null,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    StatusId = await GetStatusIdByName("Open")
                };

                _db.ComplaintRequests.AddAsync(complaint);
                await _db.SaveChangesAsync();

                await AllocateIoOfficerandCreateInvestigation(complaint);
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

        private async Task AllocateIoOfficerandCreateInvestigation(ComplaintRequest complaint)
        {
            try
            {
                var ioOfficer = await _db.IojurisdictionAssigns
                                     .Where(ioj => ioj.JurisdictionId == complaint.JurisdictionId)
                                     .Join(_db.UserMasters,
                                           ioj => ioj.UserId,
                                           um => um.UserId,
                                           (ioj, um) => um)
                                     .FirstOrDefaultAsync();

                if(ioOfficer == null)
                {
                    throw new CustomException("No IO Officer found for the given jurisdiction");
                }

                Investigation investigation = new Investigation
                {
                    Identifier = Helper.CustomHelper.DoGenerateGuid(),
                    ComplaintId = complaint.ComplaintRequestId,
                    IoOfficerId = ioOfficer.UserId,
                    StartDate = DateTime.Now,
                    Priority = "Medium", // Default priority, can be dynamic
                    StatusId = await GetStatusIdByName("Under Investigation"),
                    CreatedBy = complaint.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                _db.Investigations.Add(investigation);
                await _db.SaveChangesAsync();

                complaint.IoofficerId = ioOfficer.UserId;
                complaint.InvestigationId = investigation.InvestigationId;
                complaint.ModifyBy = complaint.CreatedBy;
                complaint.ModifyOn = DateTime.Now;
                _db.ComplaintRequests.Update(complaint);
                await _db.SaveChangesAsync();

                ComplaintIoassign complaintIoAssign = new ComplaintIoassign
                {
                    Identifier = Guid.NewGuid().ToString(),
                    ComplaintId = complaint.ComplaintRequestId,
                    UserId = ioOfficer.UserId,
                    ComplaintStatus = complaint.StatusId, // The status of the complaint when assigned
                    CreatedBy = complaint.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                _db.ComplaintIoassigns.Add(complaintIoAssign);
                await _db.SaveChangesAsync();


                await SendNotification(ioOfficer.UserId, "New Complaint Assigned", $"A new complaint (ID: {complaint.Identifier}) has been assigned to you for investigation.");
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

        private async Task<int> GetStatusIdByName(string statusName)
        {
            var status = await _db.Statusmasters.FirstOrDefaultAsync(s => s.Status == statusName);

            if (status != null)
                return status.Statusid;

            // fallback if not found in DB
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
                Type = "Complaint Assignment", // or "Investigation"
                IsRead = false,
                CreatedOn = DateTime.Now,
                CreatedBy = 1 // Assuming a system user for notifications
            };

            _db.Notifications.Add(newNotification);
            await _db.SaveChangesAsync();
        }
    }
}
