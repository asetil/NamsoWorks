using System;
using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Enums;
using Aware.File.Model;
using Aware.Util;
using Aware.Util.Enums;

namespace Aware.ECommerce.Model
{
    public class Campaign
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string ImageInfo { get; set; }
        public virtual CampaignScope Scope { get; set; }
        public virtual ItemScope ItemScope { get; set; }
        public virtual DiscountType DiscountType { get; set; }
        public virtual decimal Discount { get; set; } //oran yada tutar
        public virtual decimal MinimumAmount { get; set; } //bu tutarın üstüne uygulansın
        public virtual decimal MinimumQuantity { get; set; } //ürün bazlı kampanyalarda en az iki ürün olması koşulunu sağlamak için
        public virtual int OwnerID { get; set; } //Kim oluşturdu?
        public virtual string FilterInfo { get; set; } //Hangi ürünlere uygulanacak
        public virtual DateTime PublishDate { get; set; }
        public virtual int ExpireDays { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateModified { get; set; }
        public virtual Statuses Status { get; set; }
        public virtual ICollection<CampaignItem> Items { get; set; }

        public virtual bool IsExpired
        {
            get { return PublishDate.AddDays(ExpireDays) < DateTime.Now; }
        }

        public virtual FileRelation DefaultImage
        {
            get
            {
                var images = ImageInfo.GetFiles();
                if (images.Any()) { return images.FirstOrDefault(); }
                return new FileRelation()
                {
                    ID = 0,
                    Path = "Campaign/0.jpg"
                };
            }
        }

        public virtual double RemainedTime
        {
            get
            {
                var remained = PublishDate.AddDays(ExpireDays).AddMinutes(-5).Subtract(DateTime.Now).TotalMilliseconds;
                return remained < 0 ? 0 : remained;
            }
        }

        public virtual Campaign Clone()
        {
            var result = MemberwiseClone() as Campaign;
            result.ID = 0;
            result.PublishDate = DateTime.Now;
            return result;
        }
    }
}