namespace CrimeManagement.DTO
{
    public class CrimeResponseDTO<T>
    {
        public int? responseCode { get; set; }
        public string? responseStatus { get; set; }
        public string? responseDescription { get; set; }
        public DateTime? responseDatetime { get; set; }
        public T? data { get; set; }
    }
}
