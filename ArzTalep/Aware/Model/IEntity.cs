using Aware.Util.Enum;
using System;

namespace Aware.Model
{
    public interface IEntity
    {
        int ID { get; set; }

        int UserCreated { get; set; }

        DateTime DateCreated { get; set; }

        int UserModified { get; set; }

        DateTime DateModified { get; set; }

        StatusType Status { get; set; }

        bool IsValid();
    }
}
