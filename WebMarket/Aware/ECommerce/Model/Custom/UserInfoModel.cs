using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model.Custom
{
    public class UserInfoModel
    {
        public UserInfoModel(int id, UserRole userRole)
        {
            ID = id;
            Role = userRole;
        }

        public int ID { get; set; }
        public UserRole Role { get; set; }

        public bool IsValid
        {
            get { return Role == UserRole.SuperUser || (Role != UserRole.SuperUser && ID > 0); }
        }
    }
}
