using System.ComponentModel.DataAnnotations;

namespace API.Models.CSKHAuto
{
    public class BOAccount
    {
        [Key]
        public Guid ID { get; set; }
        public string Account { get; set; }
        public string CardHolder { get; set; }
        public string System { get; set; }
    }

    public class BOAccountDTO
    {
        public string Account { get; set; }
        public string CardHolder { get; set; }
        public string System { get; set; }
        public int RequestCount { get; set; }
    }
}
