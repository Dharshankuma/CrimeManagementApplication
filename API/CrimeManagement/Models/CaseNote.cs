using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class CaseNote
{
    public int CaseNoteId { get; set; }

    public string? Identifier { get; set; }

    public string? CaseNote1 { get; set; }

    public int? CrimeId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifyBy { get; set; }

    public DateTime? ModifyOn { get; set; }
}
