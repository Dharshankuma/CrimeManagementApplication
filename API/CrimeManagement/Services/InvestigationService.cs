using CrimeManagement.Context;
using CrimeManagement.DTO;
using CrimeManagement.Helper;
using CrimeManagement.Models;
using Microsoft.EntityFrameworkCore;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Services
{
    public interface IInvestigationService
    {
        Task<Data<List<InvestigationViewDTO>>> DoGetInvestigationOverviewDetails(InvestigationRequestviewDTO objdto);
        Task<InvestigationDTO> DoGetInvestigationDetailsById(string identifier);
        Task DoAddCaseComment(CaseNoteDTO objdto);


        Task DoUpdateInvestigation(InvestigationDTO objdto);
    }
    public class InvestigationService : IInvestigationService
    {
        private readonly CrimeDbContext _db;
        private readonly IWorkFlowValidationService _workflowService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public InvestigationService(CrimeDbContext db, IWorkFlowValidationService workflowService,IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _workflowService = workflowService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Data<List<InvestigationViewDTO>>> DoGetInvestigationOverviewDetails(InvestigationRequestviewDTO objdto)
        {
            try
            {
                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);
                if (userId == 0)
                    throw new CustomException("Invalid User");

                var roleId = await _db.UserMasters
                                .AsNoTracking()
                                .Where(u => u.UserId == userId)
                                .Select(u => u.RoleId)
                                .FirstOrDefaultAsync();

                var query = from inv in _db.Investigations.AsNoTracking()

                            join userdata in _db.UserMasters.AsNoTracking() 
                               on inv.IoOfficerId equals userdata.UserId into userdatatemp
                            from user in userdatatemp.DefaultIfEmpty()

                            join cmp in _db.ComplaintRequests.AsNoTracking()
                                on inv.ComplaintId equals cmp.ComplaintRequestId into cmpTemp
                            from cmp in cmpTemp.DefaultIfEmpty()

                            join crm in _db.CrimeTypes.AsNoTracking()
                                on cmp.CrimeTypeId equals crm.CrimeId into crmTemp
                            from crm in crmTemp.DefaultIfEmpty()

                            join jurisdictiondata in _db.JurisdictionMasters.AsNoTracking()
                                on cmp.JurisdictionId equals jurisdictiondata.JurisdictionId into jurisdictionTemp
                            from jurisdiction in jurisdictionTemp.DefaultIfEmpty()

                            join sts in _db.Statusmasters.AsNoTracking()
                                on inv.StatusId equals sts.Statusid into stsTemp
                            from sts in stsTemp.DefaultIfEmpty()

                            select new
                            {
                                inv.Identifier,
                                inv.InvestigationId,
                                ComplaintName = cmp.ComplaintName,
                                CrimeType = crm.CrimeName,
                                CrimeStatus = sts.Status,
                                inv.StatusId,
                                LastUpdatedDate = inv.ModifyOn ?? inv.CreatedOn,
                                Location = jurisdiction.JurisdictionName,
                                ComplaintIdentifier = cmp.Identifier,
                                inv.IoOfficerId,
                                ioOfficerName = user.Firstname + " " + user.Lastname
                            };

                // Filter for IO officer
                if (roleId == 2)
                {
                    query = query.Where(x => x.IoOfficerId == userId);
                }

                // Sorting
                if (!string.IsNullOrEmpty(objdto.columnName))
                {
                    bool ascending = objdto.sortOrder ?? true;

                    query = objdto.columnName.ToLower() switch
                    {
                        "complaintname" => ascending ? query.OrderBy(x => x.ComplaintName) : query.OrderByDescending(x => x.ComplaintName),
                        "crimetype" => ascending ? query.OrderBy(x => x.CrimeType) : query.OrderByDescending(x => x.CrimeType),
                        "crimestatus" => ascending ? query.OrderBy(x => x.CrimeStatus) : query.OrderByDescending(x => x.CrimeStatus),
                        "lastupdateddate" => ascending ? query.OrderBy(x => x.LastUpdatedDate) : query.OrderByDescending(x => x.LastUpdatedDate),
                        _ => query.OrderByDescending(x => x.LastUpdatedDate)
                    };
                }
                else
                {
                    query = query.OrderByDescending(x => x.LastUpdatedDate);
                }
                // Search
                if (!string.IsNullOrWhiteSpace(objdto.search))
                {
                    var search = objdto.search.ToLower();

                    query = query.Where(x =>
                        (x.ComplaintName != null && x.ComplaintName.ToLower().Contains(search)) ||
                        (x.CrimeType != null && x.CrimeType.ToLower().Contains(search)) ||
                        (x.CrimeStatus != null && x.CrimeStatus.ToLower().Contains(search)) ||
                        (x.Location != null && x.Location.ToLower().Contains(search)) ||
                        (x.ioOfficerName != null && x.ioOfficerName.ToLower().Contains(search))
                    );
                }

                var totalCount = await query.CountAsync();

                int pageNumber = objdto.PageNumber >= 0 ? objdto.PageNumber : 0;
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var result = await query
                    .Skip((pageNumber - 1)* pageSize)
                    .Take(pageSize)
                    .Select(x => new InvestigationViewDTO
                    {
                        investigationIdentifer = x.Identifier,
                        investigationId = x.InvestigationId.ToString(),
                        complaintName = x.ComplaintName,
                        crimeType = x.CrimeType,
                        crimeStatus = x.CrimeStatus,
                        lastUpdatedDate = x.LastUpdatedDate.HasValue
                                            ? x.LastUpdatedDate.Value.ToString("dd-MM-yyyy")
                                            : string.Empty,
                        Location = x.Location,
                        statusId = x.StatusId,
                        complaintIdentifier = x.ComplaintIdentifier,
                        ioOfficerName = x.ioOfficerName
                    })
                    .ToListAsync();

                return new Data<List<InvestigationViewDTO>>
                {
                    data = result,
                    totalCount = totalCount
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<InvestigationDTO> DoGetInvestigationDetailsById(string identifier)
        {
            try
            {
                var investigation = await (from inv in _db.Investigations
                                           join c in _db.ComplaintRequests on inv.ComplaintId equals c.ComplaintRequestId into cgroup
                                           from complaint in cgroup.DefaultIfEmpty()
                                           join jurisdictiondata in _db.JurisdictionMasters on complaint.JurisdictionId equals jurisdictiondata.JurisdictionId into jurisdictiontemp
                                           from jurisdiction in jurisdictiontemp.DefaultIfEmpty()
                                           join crime in _db.CrimeTypes on complaint.CrimeTypeId equals crime.CrimeId into crgroup
                                           from cr in crgroup.DefaultIfEmpty()
                                           join status in _db.Statusmasters on inv.StatusId equals status.Statusid into stgroup
                                           from st in stgroup.DefaultIfEmpty()
                                           join usdata in _db.UserMasters on complaint.CreatedBy equals usdata.UserId into usdatatemp
                                           from us in usdatatemp.DefaultIfEmpty()
                                           where inv.Identifier == identifier
                                           select new InvestigationDTO
                                           {
                                               Identifier = inv.Identifier,
                                               ComplaintId = inv.ComplaintId,
                                               complaintIdentifier = complaint != null ? complaint.Identifier : null,
                                               IoOfficerId = inv.IoOfficerId,
                                               ioOfficerIdentifier = inv.IoOfficerId != null ? _db.UserMasters.Where(u => u.UserId == inv.IoOfficerId).Select(u => u.Identifier).FirstOrDefault() : null,
                                               StartDate = inv.StartDate,
                                               startDateString = inv.StartDate.HasValue ? inv.StartDate.Value.ToString("yyyy-MM-dd HH:mm") : null,
                                               EndDate = inv.EndDate,
                                               endDateString = inv.EndDate.HasValue ? inv.EndDate.Value.ToString("yyyy-MM-dd HH:mm") : null,
                                               Priority = inv.Priority,
                                               StatusId = inv.StatusId,
                                               statusIdentifier = st != null ? st.Status : null,
                                               EvidenceAttachmentId = inv.EvidenceAttachmentId,
                                               CrimeId = inv.CrimeId,
                                               crimeIdentifier = cr != null ? cr.Identifier : null,
                                               CriminalId = inv.CriminalId,
                                               FirattachmentId = inv.FirattachmentId,
                                               InvestigationDescription = inv.InvestigationDescription,
                                               CreatedBy = inv.CreatedBy,
                                               CreatedOn = inv.CreatedOn,
                                               ModifyBy = inv.ModifyBy,
                                               ModifyOn = inv.ModifyOn,
                                               complaintDescription = complaint.CrimeDescription,
                                               complaintRaiseName = us.UserName,
                                               location = jurisdiction.JurisdictionName,
                                               crimeType = cr.CrimeName,
                                               complaintName = complaint.ComplaintName,
                                               statusName = st.Status
                                           }).FirstOrDefaultAsync();


                var finaldata = investigation;

                return finaldata;
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

        public async Task DoUpdateInvestigation(InvestigationDTO objdto)
        {
            try
            {
                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);

                var user = await _db.UserMasters
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    throw new CustomException("Invalid user");

                
                var investigation = await _db.Investigations
                    .FirstOrDefaultAsync(i => i.Identifier == objdto.Identifier);

                if (investigation == null)
                    throw new CustomException("Investigation not found");


                if (user.RoleId != 2)
                    throw new CustomException("Only assigned officer can update investigation");

                if (!string.IsNullOrEmpty(objdto.Priority))
                    investigation.Priority = objdto.Priority;

                var statusId = await _db.Statusmasters
            .Where(x => x.Identifier == objdto.statusIdentifier)
            .Select(x => x.Statusid)
            .FirstOrDefaultAsync();

                if (statusId == 0)
                    throw new CustomException("Invalid Status");

                int oldStatus = investigation.StatusId ?? 0;

                await _workflowService.ValidationStatusTransaction(oldStatus, statusId, user.RoleId);

                if (!string.IsNullOrEmpty(objdto.InvestigationDescription))
                    investigation.InvestigationDescription = objdto.InvestigationDescription;

                if (!string.IsNullOrEmpty(objdto.endDateString))
                {
                    var enddate = Helper.CustomHelper.ParseDate(objdto.endDateString);
                    investigation.EndDate = enddate;
                }

                var closedNoActionStatusId = await _db.Statusmasters
            .Where(x => x.Status == "Closed - No Action")
            .Select(x => x.Statusid)
            .FirstOrDefaultAsync();

                

                if (statusId == closedNoActionStatusId &&
                    string.IsNullOrWhiteSpace(objdto.InvestigationDescription))
                {
                    throw new CustomException("Reason is mandatory for closing a case as No Action");
                }

                investigation.StatusId = statusId;
                investigation.ModifyBy = userId;
                investigation.ModifyOn = DateTime.Now;

                _db.Investigations.Update(investigation);



                var history = new InvestigationStageHistory
                {
                    InvestigationId = investigation.InvestigationId,
                    FromStatusId = oldStatus,
                    ToStatusId = statusId,
                    ChangedBy = user.UserId,
                    ChangedOn = DateTime.Now
                };

                _db.InvestigationStageHistories.Add(history);

                await _db.SaveChangesAsync();


                var complaintDetails = await _db.ComplaintRequests.Where(x => x.ComplaintRequestId == investigation.ComplaintId).FirstOrDefaultAsync();

                if(complaintDetails == null)
                {
                    throw new CustomException("No Complaint Request is found");
                }

                complaintDetails.StatusId = statusId;
                complaintDetails.ModifyBy = user.UserId;
                complaintDetails.ModifyOn = CustomHelper.DoGetDateTime();
                _db.ComplaintRequests.Update(complaintDetails);
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

        public async Task DoAddCaseComment(CaseNoteDTO objdto)
        {
            try
            {
                if (objdto == null || string.IsNullOrWhiteSpace(objdto.commentText))
                    throw new CustomException("Comment cannot be empty");

                int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value, out int userId);

                if (userId == 0)
                    throw new CustomException("Invalid user");

                // Get CrimeId from identifier
                var crimeId = await _db.ComplaintRequests
                    .Where(c => c.Identifier == objdto.crimeIdentifier)
                    .Select(c => c.ComplaintRequestId)
                    .FirstOrDefaultAsync();

                if (crimeId == 0)
                    throw new CustomException("Invalid complaint");

                CaseNote note = new CaseNote
                {
                    Identifier = Helper.CustomHelper.DoGenerateGuid(),
                    CaseNote1 = objdto.commentText.Trim(),
                    CrimeId = crimeId,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now
                };

                await _db.CaseNotes.AddAsync(note);
                await _db.SaveChangesAsync();
            }
            catch (CustomException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
