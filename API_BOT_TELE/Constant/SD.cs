namespace TELEBOT_CSKH.Constant
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
            ExcelFile
        }

        public static string[] Project_Code = { "SHBET", "NEW88", "K36", "789BET", "F8BET", "HI88", "CMD", "SHBET_v2", "MB66", "78WIN" };

        public static string DomainURL = "https://api-tele.attcloud.org";

        public static bool fBotCheckAgentUpdate = false;
        public static bool fBotShareTeleUpdate = false;
        public static bool fTeleResposeDataUpdate = true;
    }
}
