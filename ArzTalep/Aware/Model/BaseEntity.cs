using Aware.Util.Enum;
using System;

namespace Aware.Model
{
    public abstract class BaseEntity : IEntity
    {
        public int ID { get; set; }

        public int UserCreated { get; set; }

        public DateTime DateCreated { get; set; }

        public int UserModified { get; set; }

        public DateTime DateModified { get; set; }

        public StatusType Status { get; set; }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
