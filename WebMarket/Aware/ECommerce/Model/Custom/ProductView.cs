using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Aware.Util;
using Aware.ECommerce.Util;
using Aware.ECommerce.Enums;
using Aware.File.Model;

namespace Aware.ECommerce.Model
{
    public partial class Product
    {
        private IEnumerable<PropertyView> _properties;
        private IEnumerable<StoreItem> _items;

        public virtual FileRelation DefaultImage
        {
            get
            {
                var images = ImageInfo.GetFiles();
                return images.Any() ? images.FirstOrDefault() : new FileRelation()
                {
                    ID = 0,
                    Path = "Product/0.png"
                };
            }
        }

        public virtual decimal CommentRating
        {
            get
            {
                if (!string.IsNullOrEmpty(CommentInfo))
                {
                    return CommentInfo.Split(';').FirstOrDefault().Replace(".", ",").Dec();
                }
                return 0;
            }
        }

        public virtual int CommentCount
        {
            get
            {
                if (!string.IsNullOrEmpty(CommentInfo))
                {
                    return CommentInfo.Split(';').LastOrDefault().Int();
                }
                return 0;
            }
        }


        [NotMapped]
        public virtual IEnumerable<StoreItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                DefaultItem = new StoreItem();

                if (_items != null && _items.Any())
                {
                    _items.OrderBy(i => i.SalesPrice).FirstOrDefault().AddScore(25);
                    DefaultItem = _items.OrderByDescending(i => i.Score).FirstOrDefault();
                    ItemCount = _items.Count();
                }
            }
        }

        [NotMapped]
        public virtual StoreItem DefaultItem { get; set; }

        [NotMapped]
        public virtual int ItemCount { get; set; }

        [NotMapped]
        public virtual string ItemsInfo
        {
            get
            {
                if (ItemCount > 0)
                {
                    var info = Items.Select(i => new
                    {
                        id = i.ID,
                        store = (i.Store != null ? i.Store.Name : string.Empty),
                        price = i.SalesPrice.ToPrice(),
                        lprice = i.ListPrice.ToPrice(),
                        hasstock = i.HasStock(1),
                        discount = i.DiscountRate,
                        unit = UnitDescription.Short(2, string.Empty),
                        forsale = i.IsForSale,
                        hasvariant = i.HasVariant,
                        isdefault = (i.ID == DefaultItem.ID)
                    });
                    return Aware.Util.Common.Serialize(info);
                }
                return "{}";
            }
        }

        [NotMapped]
        public virtual string ImageList
        {
            get
            {
                var images = ImageInfo.GetFiles();
                if (images != null && images.Any())
                {
                    var info = images.Take(3).Select(i => new
                    {
                        path = i.Path
                    });
                    return Common.Serialize(info);
                }
                return "{ \"path\" : \"Product/0.png\"}";
            }
        }

        public virtual IEnumerable<PropertyView> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = PropertyView.GetProperties(PropertyInfo);
                }
                return _properties;
            }
        }

        public virtual string UnitDescription
        {
            get
            {
                switch (Unit)
                {
                    case MeasureUnits.Gram:
                        return "gr";
                    case MeasureUnits.Kg:
                        return "kg";
                    case MeasureUnits.Unit:
                        return "adet";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual bool IsForSale
        {
            get { return Items != null && Items.Any(i => i.IsForSale); }
        }

        public virtual decimal UnitFactor
        {
            get
            {
                if (Unit == MeasureUnits.Kg) { return 0.5M; }
                if (Unit == MeasureUnits.Gram) { return 50M; }
                return 1;
            }
        }
    }
}
