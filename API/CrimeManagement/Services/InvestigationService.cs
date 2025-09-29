using CrimeManagement.Context;
using CrimeManagement.DTO;
using Microsoft.EntityFrameworkCore;
using static CrimeManagement.DTO.CrimeResponseDTO;

namespace CrimeManagement.Services
{
    public interface IInvestigationService
    {
        Task<Data<List<InvestigationViewDTO>>> DoGetInvestigationOverviewDetails(InvestigationRequestviewDTO objdto);
        Task<InvestigationDTO> DoGetInvestigationDetailsById(string identifier);

        Task DoUpdateInvestigation(InvestigationDTO objdto);
    }
    public class InvestigationService : IInvestigationService
    {
        private readonly CrimeDbContext _db;

        public InvestigationService(CrimeDbContext db)
        {
            _db = db;
        }

        public async Task<Data<List<InvestigationViewDTO>>> DoGetInvestigationOverviewDetails(InvestigationRequestviewDTO objdto)
        {
            try
            {
                // Step 1: Get user
                var user = await _db.UserMasters
                                    .FirstOrDefaultAsync(u => u.Identifier == objdto.userIdentifier);
                if (user == null)
                    throw new CustomException("Invalid User");

                // Step 2: Build base query (keep dates as DateTime?)
                var query = from inv in _db.Investigations
                            join cmp in _db.ComplaintRequests on inv.ComplaintId equals cmp.ComplaintRequestId into cmpTemp
                            from cmp in cmpTemp.DefaultIfEmpty()
                            join crm in _db.CrimeTypes on cmp.CrimeTypeId equals crm.CrimeId into crmTemp
                            from crm in crmTemp.DefaultIfEmpty()
                            join jurisdictiondata in _db.JurisdictionMasters on cmp.JurisdictionId equals jurisdictiondata.JurisdictionId into jurisdictiontemp
                            from jurisdiction in jurisdictiontemp.DefaultIfEmpty()
                            join sts in _db.Statusmasters on inv.StatusId equals sts.Statusid into stsTemp
                            from sts in stsTemp.DefaultIfEmpty()
                            select new
                            {
                                inv.Identifier,
                                inv.InvestigationId,
                                ComplaintName = cmp != null ? cmp.ComplaintName : null,
                                CrimeType = crm != null ? crm.CrimeName : null,
                                CrimeStatus = sts != null ? sts.Status : null,
                                statusId = inv.StatusId,
                                LastUpdatedDate = inv.ModifyOn ?? inv.CreatedOn,
                                Location = jurisdiction.JurisdictionName
                            };

                // Step 3: Filter by IoOfficer if role is 3
                if (user.RoleId == 3)
                {
                    query = query.Where(x => _db.Investigations
                                                .Where(i => i.IoOfficerId == user.UserId)
                                                .Select(i => i.InvestigationId)
                                                .Contains(x.InvestigationId));
                }

                // Step 4: Apply sorting
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

                // Step 5: Get total count
                var totalCount = await query.CountAsync();

                // Step 6: Apply paging
                int pageNumber = objdto.PageNumber >= 0 ? objdto.PageNumber : 0; // allow 0 as first page
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var pagedData = await query
                    .Skip(pageNumber * pageSize)   // 0 → first page, 1 → second page, etc.
                    .Take(pageSize)
                    .ToListAsync();

                // Step 7: Map to DTO and format date in memory
                var result = pagedData.Select(x => new InvestigationViewDTO
                {
                    investigationIdentifer = x.Identifier,
                    investigationId = x.InvestigationId.ToString(),
                    complaintName = x.ComplaintName,
                    crimeType = x.CrimeType,
                    crimeStatus = x.CrimeStatus,
                    lastUpdatedDate = x.LastUpdatedDate?.ToString("dd-MM-yyyy") ?? string.Empty,
                    Location = x.Location,
                    statusId = x.statusId
                }).ToList();

                return new Data<List<InvestigationViewDTO>>
                {
                    data = result,
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

                return investigation;
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
                
                var user = await _db.UserMasters
                    .FirstOrDefaultAsync(u => u.Identifier == objdto.userIdentifier);

                if (user == null)
                    throw new CustomException("Invalid user");

                
                var investigation = await _db.Investigations
                    .FirstOrDefaultAsync(i => i.Identifier == objdto.Identifier);

                if (investigation == null)
                    throw new CustomException("Investigation not found");

                
                if (user.RoleId == 3 && investigation.IoOfficerId != user.UserId)
                    throw new CustomException("You are not authorized to update this investigation");

                
                if (!string.IsNullOrEmpty(objdto.Priority))
                    investigation.Priority = objdto.Priority;

                var statusId = await _db.Statusmasters.Where(x=>x.Identifier == objdto.statusIdentifier).Select(x=>x.Statusid).FirstOrDefaultAsync();

                investigation.StatusId = statusId != 0 ? statusId : investigation.StatusId;

                if (!string.IsNullOrEmpty(objdto.InvestigationDescription))
                    investigation.InvestigationDescription = objdto.InvestigationDescription;

                if (!string.IsNullOrEmpty(objdto.endDateString))
                {
                    var enddate = Helper.CustomHelper.ParseDate(objdto.endDateString);
                    investigation.EndDate = enddate;
                }

                investigation.ModifyBy = user.UserId;
                investigation.ModifyOn = DateTime.Now;

                _db.Investigations.Update(investigation);
                await _db.SaveChangesAsync();


                var complaintDetails = await _db.ComplaintRequests.Where(x => x.ComplaintRequestId == investigation.ComplaintId).FirstOrDefaultAsync();

                if(complaintDetails == null)
                {
                    throw new CustomException("No Complaint Request is found");
                }

                complaintDetails.StatusId = statusId;
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

    }
}
