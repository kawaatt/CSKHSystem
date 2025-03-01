namespace API.Constant
{
    public class SD
    {
        public static string DomainImageURL = "";

        //Declare Rabbit MQ Name
        public const string DepositReceipt_Upload_ExchangeName = "DEPOSIT_RECEIPT_EXCHANGE";

        public const string DepositReceipt_Upload_Request = "RECEIPT_IMAGE_UPLOAD_REQUEST";
        public const string DepositReceipt_Upload_Response = "RECEIPT_IMAGE_UPLOAD_RESPONSE";

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
    }
}
