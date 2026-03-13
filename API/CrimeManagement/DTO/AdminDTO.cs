using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CrimeManagement.DTO
{
    public class AdminDTO
    {
        public int? adminType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class  AdminGridResponse
    {
        public List<AdminGridView> gridDetails { get; set; }
        public int? totalCount { get; set; }
    }

    public class AdminGridView
    {
        public string? name { get; set; }
        public string?  identifier{ get; set; }
        public int? status{ get; set; }
        public int? totalCount { get; set; }
        public string? jurisdiction { get; set; }
        public string? userStatus { get; set; }

    }

    public class AdminUpdateDTO
    {
        public string? identifier { get; set; }
        public string? userIdentifier { get; set; }
        public string? name { get; set; }
        public string? status { get; set; }
        public int? adminType { get; set; }
    }
}
