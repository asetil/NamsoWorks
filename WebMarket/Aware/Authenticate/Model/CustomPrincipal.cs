using System.Security.Principal;
using Aware.ECommerce.Enums;

namespace Aware.Authenticate.Model
{
    public class CustomPrincipal : IPrincipal
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public IIdentity Identity { get; set; }
        public bool IsInRole(string role)
        {
            return false;
        }

        public bool IsAdmin
        {
            get { return Role == UserRole.SuperUser || Role == UserRole.AdminUser; }
        }

        public bool IsSuper
        {
            get { return Role == UserRole.SuperUser; }
        }
    }
}
