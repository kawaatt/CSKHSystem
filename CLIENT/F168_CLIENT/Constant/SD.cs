namespace SHBET_CLIENT.Constant
{
    public class SD
    {
        public enum APIType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public enum ContentType
        {
            Json,
            MultiPartFormData,
            ExcelFile,
            FormURLEncodedContent
        }

        public static string[] Project_Code = { "SHBET", "NEW88", "K36", "789BET", "F8BET", "HI88", "CMD", "SHBET_v2", "MB66", "78WIN" };

        public const string TokenCookie = "JWTToken_CSKHSystem";
    }
}
