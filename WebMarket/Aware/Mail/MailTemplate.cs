using System.ComponentModel.DataAnnotations.Schema;
using Aware.ECommerce.Model;

namespace Aware.Mail
{
    public class MailTemplate : IEntity
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int ParentID { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Content { get; set; }

        [NotMapped]
        public virtual MailTemplate Parent { get; set; }

        public virtual string PreviewHtml
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
    }
}