using System;
using System.Web.Mvc;
using Aware.ECommerce.Model;

namespace CleanFramework.Business.Model
{
    public class Entry
    {
        public virtual int ID { get; set; }
        public virtual int UserID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual string Name { get; set; }

        [AllowHtml]
        public virtual string Summary { get; set; }

        [AllowHtml]
        public virtual string Content { get; set; }
        public virtual string Keywords { get; set; }
        public virtual string ImageInfo { get; set; }
        public virtual string SortOrder { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual Aware.Util.Enums.Statuses Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual string Author { get; set; }
    }
}
