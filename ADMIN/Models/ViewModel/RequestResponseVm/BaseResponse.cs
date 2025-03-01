namespace ADMIN.Models.ViewModel.RequestResponseVm
{
    public class BaseResponse
    {
        /// <summary>
        /// Tin nhắn trả về khi thành công
        /// </summary>
        //public string? StatusCode { get; set; }
        //public string? Message { get; set; }
        public bool Success { get; set; }
        public int? StatusCode { get; set; }
        public string? Message { get; set; }

    }

    public class BaseResponse<T> : BaseResponse
    {
        /// <summary>
        /// dữ liệu trả về
        /// </summary>
        public T? Data { get; set; }
    }

    public class BaseResponsePagination<T> : BaseResponse<T>
    {
        /// <summary>
        /// Dữ liệu phân trang
        /// </summary>
        public PaginationResponseVm Paging { get; set; } = null;
        public PaginationResponseVm Pagination { get; set; } = null;

        /// <summary>
        /// dữ liệu trả về
        /// </summary>
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public int? StatusCode { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public object Detail { get; set; }

    }
}
