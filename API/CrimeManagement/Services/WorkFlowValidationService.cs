using CrimeManagement.Context;
using CrimeManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface IWorkFlowValidationService
    {
        Task ValidationStatusTransaction(int? fromStatusId, int? toStatusId, int? roleId);
    }
    public class WorkFlowValidationService : IWorkFlowValidationService
    {
        private readonly CrimeDbContext _db;

        public WorkFlowValidationService(CrimeDbContext db)
        {
            _db = db;
        }


        public async Task ValidationStatusTransaction(int? fromStatusId , int? toStatusId , int? roleId)
        {
            bool allowed = await _db.StatusTransitionRules.AnyAsync(x => x.FromStatusId == fromStatusId && x.ToStatusId == toStatusId && x.AllowedRoleId == roleId && x.IsActive);

            if (!allowed)
            {
                throw new CustomException("Invalid status transaction");
            }
        }
    }
}
