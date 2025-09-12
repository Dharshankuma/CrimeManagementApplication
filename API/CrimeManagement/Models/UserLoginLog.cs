using System;
using System.Collections.Generic;

namespace CrimeManagement.Models;

public partial class UserLoginLog
{
    public int UserLoginId { get; set; }

    public int? UserId { get; set; }

    public DateTime? LoginDateTime { get; set; }
}
