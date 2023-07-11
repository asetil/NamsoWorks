namespace Aware.Util.View
{
    public class Field
    {
        public Enums.FieldType Type { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public string ID { get; set; }
        public string Css { get; set; }
        public string ParentCss { get; set; }
        public string Attributes { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public string Placeholder { get; set; }
        public string Extra { get; set; }

        public bool HasAffix
        {
            get { return !string.IsNullOrEmpty(Prefix) || !string.IsNullOrEmpty(Suffix); }
        }

        public Field SetAffix(string suffix, string prefix = "")
        {
            Suffix = suffix;
            Prefix = prefix;
            return this;
        }

        public Field SetAttr(string attributeString)
        {
            if (!string.IsNullOrEmpty(attributeString))
            {
                Attributes += attributeString;
            }
            return this;
        }

        public Field SetAttr(bool disabled = false)
        {
            if (disabled)
            {
                Attributes += " disabled='disabled'";
            }
            return this;
        }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case Enums.FieldType.Password:
                        return "password";
                }
                return "text";
            }
        }
    }
}
