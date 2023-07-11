using Aware.ECommerce.Enums;

namespace Aware.Language.Model
{
    public class LanguageValue
    {
        public virtual int ID { get; set; }
        public virtual int LangID { get; set; }
        public virtual int RelationType { get; set; }
        public virtual int RelationID { get; set; }
        public virtual string FieldName { get; set; }
        public virtual string Content { get; set; }
    }
}
