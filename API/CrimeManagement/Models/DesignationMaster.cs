using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class DesignationMaster
{
    public int DesignationId { get; set; }

    public string? DesignationName { get; set; }

    public string? Identifier { get; set; }

    public int? RoleId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
