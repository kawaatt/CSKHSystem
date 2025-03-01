namespace ADMIN.Models.ViewModel.RequestResponseVm
{
    public class PaginationResponseVm
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số dòng
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int Pages => Total <= 0 || PageSize <= 0 ? 0 : (int)Math.Ceiling(Convert.ToDouble(Total) / PageSize);


        public PaginationResponseVm(int pageIndex = 1, int pageSize = 20, long total = 0)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Total = total;
        }
    }
}
