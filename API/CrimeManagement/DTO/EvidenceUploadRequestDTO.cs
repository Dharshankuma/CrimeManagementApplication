namespace CrimeManagement.DTO
{
    public class EvidenceUploadRequestDTO
    {
        public string? complaintIdentifier { get; set; }
        public List<IFormFile>? files { get; set; }
    }
}
