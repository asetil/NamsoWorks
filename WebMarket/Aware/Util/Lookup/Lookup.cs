namespace Aware.Util.Lookup
{
    public class Lookup
    {
        public virtual int ID { get; set; }
        public virtual int Type { get; set; }
        public virtual int Value { get; set; }
        public virtual string Name { get; set; }
        public virtual int Order { get; set; }
        public virtual int LangID { get; set; }
        public virtual bool IsActive { get; set; }
    }
}