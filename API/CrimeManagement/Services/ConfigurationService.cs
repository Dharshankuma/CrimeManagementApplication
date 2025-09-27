using CrimeManagement.Context;
using CrimeManagement.DTO;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagement.Services
{
    public interface IConfigurationService
    {
        Task<ConfigurationDTO> DoGetConfigurationDetails();
    }
    public class ConfigurationService : IConfigurationService
    {
        private readonly CrimeDbContext _db;

        public ConfigurationService(CrimeDbContext db)
        {
            _db = db;
        }


        public async Task<ConfigurationDTO> DoGetConfigurationDetails()
        {
            try
            {
                var CountryMaster = await _db.CountryMasters.Select(x => new { identifer = x.Identifier, name = x.CountryName }).ToListAsync();
                var StateMaster = await _db.StateMasters.Select(x => new { identifer = x.Identifier, name = x.StateName }).ToListAsync();
                var JurisdictionMaster = await _db.JurisdictionMasters.Select(x => new { identifer = x.Identifier, name = x.JurisdictionName }).ToListAsync();
                var crimeTypes = await _db.CrimeTypes.Select(x => new { identifier = x.Identifier, name = x.CrimeName }).ToListAsync();
                var statusMaster = await _db.Statusmasters.Select(x => new { identifier = x.Identifier, name = x.Status }).ToListAsync();

                var data  = new
                {
                    CountryMaster = CountryMaster,
                    StateMaster = StateMaster,
                    JurisdictionMaster = JurisdictionMaster,
                    CrimeTypes = crimeTypes,
                    StatusMaster = statusMaster
                };

                return new ConfigurationDTO
                {
                    data = data
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
    }
}
