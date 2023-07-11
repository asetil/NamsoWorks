using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Aware.Util;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;

namespace Aware.ECommerce.Model
{
    public partial class Product
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ShortDescription { get; set; }

        [AllowHtml]
        public virtual string Description { get; set; }
        public virtual string Brand { get; set; }

        [ForeignKey("Category")]
        public virtual int CategoryID { get; set; }

        public virtual Category Category { get; set; }

        public virtual string PropertyInfo { get; set; } //JSON

        public virtual string ImageInfo { get; set; } //JSON

        public virtual string CommentInfo { get; set; } // rating;count

        public virtual MeasureUnits Unit { get; set; }

        public virtual string Barcode { get; set; }

        public virtual DateTime DateModified { get; set; }

        public virtual Statuses Status { get; set; }
    }
}
