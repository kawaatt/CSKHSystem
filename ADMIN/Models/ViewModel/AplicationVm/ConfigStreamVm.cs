using System.ComponentModel.DataAnnotations;

namespace ADMIN.Models.ViewModel.AplicationVm
{
    public class ConfigStreamBaseVm
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string userCode { get; set; }
        public string displayName { get; set; }
        public string? avatar { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public int? order { get; set; }
        public bool isStreaming { get; set; }

    }
    public class ConfigStreamDtoVm : ConfigStreamBaseVm
    {

    }

    public class ConfigStreamVm : ConfigStreamBaseVm
    {
    }
    public class InputFieldConfigStream
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string userCode { get; set; }
        public string displayName { get; set; }
        public string? avatar { get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public int? order { get; set; }
        public bool isStreaming { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class ConfigStreamAddOrEdit : InputFieldConfigStream
    {
        public string? Id { get; set; }
    }
}
