namespace ADMIN.Extensions
{
    public static class GenerateDataExtension
    {
        public static string GenGuidString()
        {
            return Guid.NewGuid().ToString();
        }
        /// <summary>
        /// Hash mật khẩu
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Hash(this string text)
        {
            return Crypt.HashPassword(text, 10, false);
        }
    }

}
