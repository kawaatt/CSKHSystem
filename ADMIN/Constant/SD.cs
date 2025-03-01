namespace ADMIN.Constant
{
    public class SD
    {
        public enum APIType
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH
        }

        public enum ContentType
        {
            Json,
            MultiPartFormData,
            ExcelFile,
            FormURLEncodedContent
        }

        public static string[] Project_Code = { "789BET", "K36", "CMD", "NEW88", "SHBET", "F168", "F8BET", "OK9", "MB66", "78WIN", "HI88", "QQ88" };

        public const string TokenCookie = "JWTToken_ADMIN_CSKHSystem";
    }
}
