using Worchart.BL.Constants;
using Worchart.BL.Enum;

namespace Worchart.BL.Model
{
    public class ResourceItem : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Code { get; set; }
        public virtual string Tr { get; set; }
        public virtual string En { get; set; }
        public virtual ResourceScope Scope { get; set; }

        public virtual string this[string lang]
        {
            get
            {
                if (lang == CommonConstants.LanguageEN)
                {
                    return En;
                }
                return Tr;
            }
        }

        public virtual ResourceItem Clone()
        {
            var result = MemberwiseClone() as ResourceItem;
            result.ID = 0;
            return result;
        }

        public virtual bool IsValid()
        {
            return Code.Valid();
        }
    }
}
