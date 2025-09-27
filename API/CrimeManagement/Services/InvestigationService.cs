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
                var user = await _db.UserMasters.Where(u => u.Identifier == objdto.userIdentifier).FirstOrDefaultAsync();
                if(user == null)
                {
                    throw new CustomException("Invalid User");
                }

                var query = from inv in _db.Investigations
                            join cmpdata in _db.ComplaintRequests on inv.ComplaintId equals cmpdata.ComplaintRequestId into cmpdatatemp
                            from cmp in cmpdatatemp.DefaultIfEmpty()
                            join crmdata in _db.CrimeTypes on inv.CrimeId equals crmdata.CrimeId into crmdatatemp
                            from crm in crmdatatemp.DefaultIfEmpty()
                            join stsdata in _db.Statusmasters on inv.StatusId equals stsdata.Statusid into stsdatatemp
                            from sts in stsdatatemp.DefaultIfEmpty()
                            select new InvestigationViewDTO
                            {
                                investigationIdentifer = inv.Identifier,
                                investigationId = inv.InvestigationId.ToString(),
                                complaintName = cmp != null ? cmp.ComplaintName : null,
                                crimeType = crm != null ? crm.CrimeName : null,
                                crimeStatus = sts != null ? sts.Status : null,
                                lastUpdatedDate = inv.ModifyOn != null ? inv.ModifyOn.Value.ToString("dd-MM-yyyy")
                                                                       : inv.CreatedOn != null ? inv.CreatedOn.Value.ToString("dd-MM-yyyy") : null,
                            };

                // Execute query asynchronously
                

                if(user.RoleId == 3)
                {
                    query = query.Where(x => _db.Investigations
                                .Where(i => i.IoOfficerId == user.UserId)
                                .Select(i => i.InvestigationId)
                                .Contains(Convert.ToInt32(x.investigationId)));
                }

                if (!string.IsNullOrEmpty(objdto.columnName))
                {
                    bool ascending = objdto.sortOrder ?? true;

                    query = objdto.columnName.ToLower() switch
                    {
                        "complaintname" => ascending ? query.OrderBy(x => x.complaintName) : query.OrderByDescending(x => x.complaintName),
                        "crimetype" => ascending ? query.OrderBy(x => x.crimeType) : query.OrderByDescending(x => x.crimeType),
                        "crimestatus" => ascending ? query.OrderBy(x => x.crimeStatus) : query.OrderByDescending(x => x.crimeStatus),
                        "lastupdateddate" => ascending ? query.OrderBy(x => x.lastUpdatedDate) : query.OrderByDescending(x => x.lastUpdatedDate),
                        _ => query.OrderByDescending(x => x.lastUpdatedDate)
                    };
                }
                else
                {
                    // Default sorting
                    query = query.OrderByDescending(x => x.lastUpdatedDate);
                }

                var result = await query.ToListAsync();

                var totalCount = result.Count();

                int pageNumber = objdto.PageNumber > 0 ? objdto.PageNumber : 1;
                int pageSize = objdto.PageSize > 0 ? objdto.PageSize : 10;

                var pagedResult = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();


                return new Data<List<InvestigationViewDTO>>
                {
                    data = pagedResult,
                    totalCount = totalCount
                };
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

        public async Task<InvestigationDTO> DoGetInvestigationDetailsById(string identifier)
        {
            try
            {
                var investigation = await (from inv in _db.Investigations
                                           join c in _db.ComplaintRequests on inv.ComplaintId equals c.ComplaintRequestId into cgroup
                                           from complaint in cgroup.DefaultIfEmpty()
                                           join crime in _db.CrimeTypes on inv.CrimeId equals crime.CrimeId into crgroup
                                           from cr in crgroup.DefaultIfEmpty()
                                           join status in _db.Statusmasters on inv.StatusId equals status.Statusid into stgroup
                                           from st in stgroup.DefaultIfEmpty()
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
                                               ModifyOn = inv.ModifyOn
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
                    if (DateTime.TryParse(objdto.endDateString, out DateTime endDate))
                        investigation.EndDate = endDate;
                }

                investigation.ModifyBy = user.UserId;
                investigation.ModifyOn = DateTime.Now;

                _db.Investigations.Update(investigation);
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
