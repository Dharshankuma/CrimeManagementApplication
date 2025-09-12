using Microsoft.Identity.Client;

namespace CrimeManagement.DTO
{
    public class LoginDTO
    {
        public string? emailId { get; set; }
        public string? password { get; set; }
        public string? confirmPassword { get; set; }
    }

    public class LoginDetails
    {
        public UserLoginDetails? userDetails { get; set; }
        public string? token { get; set; }
        public int? expiryTime { get; set; }
    }

    public class UserRegisterDetails
    {
        public string? userName { get; set; }
        public string? password { get; set; }
        public string? emailId { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleName { get; set; }
        public string? confirmPassword { get; set; }
        public string? phoneNo { get; set; }
    }
}
