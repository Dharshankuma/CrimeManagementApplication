using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class UserMaster
{
    public int UserId { get; set; }

    public string? Identifier { get; set; }

    public string? UserName { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? MiddleName { get; set; }

    public string? PhoneNo { get; set; }

    public string? EmailId { get; set; }

    public string? HashPassword { get; set; }

    public int? RoleId { get; set; }

    public int? Jurisdiction { get; set; }

    public int? Createdby { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Aadhaar { get; set; }

    public string? Pan { get; set; }

    public string? Status { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }

    public string? ProfilePhotoPath { get; set; }

    public int? DesignationId { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
