using CrimeManagement.Context;
using CrimeManagement.DTO;

namespace CrimeManagement.Services
{
    public interface ILoginService
    {
        Task DoRegisterUser(RegisterUserDTO objdto)
    }
    public class LoginService : ILoginService
    {
        private readonly CrimeDbContext _db;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private readonly IConfiguration _config;

        public LoginService(CrimeDbContext db, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IConfiguration config)
        {
            _db = db;
            _environment = environment;
            _config = config;
        }


        public async Task DoRegisterUser(RegisterUserDTO objdto)
        {
            try
            {

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
