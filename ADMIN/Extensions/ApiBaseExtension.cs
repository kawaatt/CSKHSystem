using ADMIN.Constant;
using static ADMIN.Constant.SD;

namespace ADMIN.Extensions
{
    public class ApiBaseExtension
    {
        public static string ToQueryString(object obj)
        {
            var properties = obj.GetType().GetProperties();
            var keyValuePairs = properties
                .Where(property => property.GetValue(obj) != null)
                .Select(property =>
                {
                    var value = property.GetValue(obj);

                    // Kiểm tra kiểu DateTime và format
                    if (value is DateTime dateTimeValue)
                    {
                        return $"{property.Name}={Uri.EscapeDataString(dateTimeValue.ToString("yyyy-MM-ddTHH:mm:ss"))}";
                    }

                    // Xử lý kiểu dữ liệu khác
                    return $"{property.Name}={Uri.EscapeDataString(value.ToString())}";
                });

            return string.Join("&", keyValuePairs);
        }

        public class RequestApiDto
        {
            public APIType APIType { get; set; } = APIType.GET;
            public string Url { get; set; }
            public string ProjectCode { get; set; }
            public object Data { get; set; }
            public string AccessToken { get; set; }
            public ContentType ContentType { get; set; } = ContentType.Json;
        }
    }
}
