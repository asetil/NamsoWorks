using Aware.Util.Enums;

namespace Aware.Language.Model
{
    public class Language
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Abbreviate { get; set; }
        public virtual string ImageInfo { get; set; }
        public virtual bool IsDefault { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual Statuses Status { get; set; }
    }
}
