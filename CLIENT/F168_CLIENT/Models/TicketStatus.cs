using System.ComponentModel.DataAnnotations;

namespace SHBET_CLIENT.Models
{
    public class TicketStatus
    {
        public Guid ID { get; set; }
        public string StatusName { get; set; }
    }
}
