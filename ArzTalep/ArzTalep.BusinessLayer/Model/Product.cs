using Aware.Model;
using Aware.Util;

namespace ArzTalep.BL.Model
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override bool IsValid()
        {
            return Name.Valid();
        }
    }
}
