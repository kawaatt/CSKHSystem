using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADMIN.Models.ViewModel.AplicationVm
{
    public class LiveStreamBaseVm
    {
        public string Id { get; set; }
        public string ConfigStreamId { get; set; }
        public ConfigStreamBaseVm? ConfigSite { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public bool IsActive { get; set; }

    }
    public class LiveStreamDtoVm : LiveStreamBaseVm
    {

    }

    public class LiveStreamVm : LiveStreamBaseVm
    {
    }
    public class InputFieldLiveStream
    {
        public string ConfigStreamId { get; set; }
        public string Title { get; set; }

        public string Url { get; set; }

        public string Code { get; set; }
    }

    public class LiveStreamAddOrEdit : InputFieldLiveStream
    {
        public string? Id { get; set; }
    }
}
