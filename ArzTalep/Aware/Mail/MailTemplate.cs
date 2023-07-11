using Aware.Model;
using Aware.Util;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aware.Mail
{
    public class MailTemplate : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int ParentID { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        [NotMapped]
        public MailTemplate Parent { get; set; }

        public string PreviewHtml
        {
            get
            {
                if (Parent != null && !string.IsNullOrEmpty(Parent.Content))
                {
                    return string.Format(Parent.Content, Content);
                }
                return Content;
            }
        }

        public override bool IsValid()
        {
            return Name.Valid();
        }
    }
}