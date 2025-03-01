using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ADMIN.Models.ViewModel.RequestResponseVm
{

    public class PaginationRequestVm
    {
        /// <summary>
        /// Tìm kiếm
        /// </summary>
      
        public string? TextSearch { get; set; }
     
        /// <summary>
        /// Bắt đầu bằng 1
        /// </summary>
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = $"Index buộc phải lớn hơn hoặc bằng 1")]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// Tối thiểu 5 - Tối đa 100
        /// </summary>
        [DefaultValue(20)]
        [Range(5, 100, ErrorMessage = "Size buộc phải lớn hơn bằng 5 và nhỏ hơn 100")]
        public int PageSize { get; set; } = 20;
    }
   
    public class PaginationRequestConfigStreamVm: PaginationRequestVm
    {
        public string? OrderBy { get; set; }
        public string? orderByField { get; set; }
    }
}
