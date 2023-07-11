using System;
using Aware.ECommerce.Model;
using Aware.Util.Enums;

namespace Aware.Crm.Model
{
    public class Customer : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Logo { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual Statuses Status { get; set; }
    }
}
