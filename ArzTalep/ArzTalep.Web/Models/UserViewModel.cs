using Aware.Model;
using Aware.BL.Model;

namespace ArzTalep.Web.Models
{
    public class UserViewModel
    {
        public User User { get; set; }
        public OperationResult<User> SaveResult { get; set; }
    }
}
