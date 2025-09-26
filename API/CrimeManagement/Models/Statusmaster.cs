using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class Statusmaster
{
    public int Statusid { get; set; }

    public string? Status { get; set; }

    public string? Identifier { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Createdon { get; set; }

    public int? Modifyby { get; set; }

    public DateTime? Modifyon { get; set; }
}
