namespace CrimeManagement.DTO
{
    public class UserDTO
    {
        public string? identifier { get; set; }

        public string? userName { get; set; }

        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string? middleName { get; set; }

        public string? phoneNo { get; set; }

        public string? emailId { get; set; }

        public string? hashPassword { get; set; }

        public string? password { get; set; }

        public string? confirmPassword { get; set; }

        public int? roleId { get; set; }

        public string? roleName { get; set; }

        public string? roleIdentifier { get; set; }

        public int? jurisdiction { get; set; }

        public string? jurisdictionIdentifier { get; set; }

        public int? createdby { get; set; }

        public DateTime? createdOn { get; set; }

        public DateOnly? dob { get; set; }

        public string? aadhaar { get; set; }

        public string? pan { get; set; }

        public string? status { get; set; }

        public string? gender { get; set; }

        public string? address { get; set; }

        public string? emergencyContact { get; set; }

        public string? profilePhotoPath { get; set; }

        public int? designationId { get; set; }

        public string? designationIdentifier { get; set; }

        public int? modifyBy { get; set; }

        public DateTime? modifyOn { get; set; }

        public string? stateIdentifier { get; set; }

        public string? locationIdentifier { get; set; }
        
        public string? stateIdentifer { get; set; }

        public string? countryIdentifier { get; set; }
    }


    public class UserGridDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class UserLoginDetails
    {
        public string? userIdentifier { get; set; }
        public string? UserName { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? MiddleName { get; set; }
        public string? EmailId { get; set; }
        public string? HashPassword { get; set; }
        public int? RoleId { get; set; }
        public string? Role { get; set; }
        public int? Jurisdiction { get; set; }
        public string? Status { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public int? DesignationId { get; set; }
        public string? Designation { get; set; }

    }
}
