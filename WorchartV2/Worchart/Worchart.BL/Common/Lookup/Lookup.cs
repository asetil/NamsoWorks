using Worchart.BL.Model;

namespace Worchart.BL.Lookup
{
    public class Lookup : IEntity
    {
        public virtual int ID { get; set; }
        public virtual LookupType Type { get; set; }
        public virtual int Value { get; set; }
        public virtual string Name { get; set; }
        public virtual int Order { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual Lookup Clone()
        {
            var result = MemberwiseClone() as Lookup;
            result.ID = 0;
            return result;
        }

        public virtual bool IsValid()
        {
            return Name.Valid();
        }
    }
}