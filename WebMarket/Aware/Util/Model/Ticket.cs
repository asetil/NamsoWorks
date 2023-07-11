using System;

namespace Aware.Util.Model
{
    public class Ticket
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public Enums.TicketType Type { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
