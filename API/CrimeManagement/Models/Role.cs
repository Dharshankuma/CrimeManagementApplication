using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? Identifier { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
