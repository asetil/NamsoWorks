using System;
using System.ComponentModel.DataAnnotations.Schema;
using Aware.ECommerce.Model;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.Authenticate.Model
{
    public class User : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual GenderType Gender { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string Permissions { get; set; }
        public virtual int CustomerID { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual int TitleID { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual Statuses? Status { get; set; }

        [NotMapped]
        public virtual bool RememberMe { get; set; }
        
        [NotMapped]
        public virtual bool IsAdmin
        {
            get
            {
                return Role == UserRole.SuperUser  || Role == UserRole.AdminUser;
            }
        }
    }
}
