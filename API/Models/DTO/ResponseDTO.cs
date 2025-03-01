namespace API.Models.DTO
{
    public class ResponseDTO
    {
        public int? StatusCode { get; set; } = StatusCodes.Status200OK;
        public int TotalCount { get; set; } = 0;
        public object Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }

}
