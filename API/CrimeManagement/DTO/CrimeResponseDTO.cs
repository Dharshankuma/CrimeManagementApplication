namespace CrimeManagement.DTO
{
    public class CrimeResponseDTO
    {
        public int? responseCode { get; set; }
        public string? responseStatus { get; set; }
        public string? responseDescription { get; set; }
        public DateTime? responseDatetime { get; set; }

        public class Data<T>
        {
            public T? data { get; set; }
        }


        public class CommonResponseDTO
        {
            public int? responseCode { get; set; }
            public string? responseMessage { get; set; }
            public string? responseStatus { get; set; }
            public DateTime? responseDatetime { get; set; }
            public dynamic? data { get; set; }
        }
    }
}
