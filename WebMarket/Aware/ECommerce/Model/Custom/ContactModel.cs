using Aware.Util.Model;

namespace Aware.ECommerce.Model
{
    public class ContactModel
    {
        public string Name{ get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int UserID { get; set; }
        public Result Result { get; set; }
    }
}