using Aware.Model;

namespace Aware.Util.Lookup
{
    public class Lookup : BaseEntity
    {
        public int Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string ExtraData { get; set; }

        public int Order { get; set; }

        public override bool IsValid()
        {
            return Name.Valid();
        }
    }
}