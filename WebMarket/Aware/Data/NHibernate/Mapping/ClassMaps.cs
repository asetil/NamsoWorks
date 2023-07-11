using Aware.Authenticate.Model;
using Aware.Authority.Model;
using Aware.Crm.Model;
using Aware.ECommerce.Model;
using FluentNHibernate.Mapping;
using Aware.Dependency;
using Aware.Mail;
using Aware.Notification;
using Aware.Payment.Model;
using Aware.Regional.Model;
using Aware.Task.Model;
using Aware.Util.Model;
using Aware.Util.Enums;
using Aware.ECommerce.Enums;
using Aware.File.Model;
using Aware.Util.Lookup;
using Aware.Util.Slider;

namespace Aware.Data.NHibernate.Mapping
{
    public class AddressMap : TableMapper<Address>
    {
        public AddressMap() : base("Address")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.CityID);
            Map(x => x.CountyID);
            Map(x => x.DistrictID);
            Map(x => x.Phone);
            Map(x => x.UserID);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class LanguageMap : TableMapper<Language.Model.Language>
    {
        public LanguageMap()
            : base("Language")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Abbreviate);
            Map(x => x.ImageInfo);
            Map(x => x.SortOrder);
            Map(x => x.IsDefault);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class LanguageValueMap : TableMapper<Language.Model.LanguageValue>
    {
        public LanguageValueMap()
            : base("LanguageValue")
        {
            Id(x => x.ID);
            Map(x => x.LangID);
            Map(x => x.RelationID);
            Map(x => x.FieldName);
            Map(x => x.Content).CustomType("StringClob").CustomSqlType("nvarchar(max)"); ;
            Map(x => x.RelationType);
        }
    }

    public class AuthorityDefinitionMap : TableMapper<AuthorityDefinition>
    {
        public AuthorityDefinitionMap() : base("AuthorityDefinition")
        {
            Id(x => x.ID);
            Map(x => x.Title);
            Map(x => x.Type).CustomType<AuthorityType>();
            Map(x => x.Mode).CustomType<AuthorityMode>();
        }
    }

    public class AuthorityUsageMap : TableMapper<AuthorityUsage>
    {
        public AuthorityUsageMap() : base("AuthorityUsage")
        {
            Id(x => x.ID);
            Map(x => x.DefinitionID);
            Map(x => x.RelationID);
            Map(x => x.AssignBy);
            Map(x => x.Quota);
            Map(x => x.Usage);
            Map(x => x.ExpireDate);
            Map(x => x.DateCreated);
            Map(x => x.RelationType);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class BasketMap : TableMapper<Basket>
    {
        public BasketMap() : base("Basket")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.UserID);
            Map(x => x.DateCreated);
            Map(x => x.DateModified);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class BasketItemMap : TableMapper<BasketItem>
    {
        public BasketItemMap() : base("BasketItem")
        {
            Id(x => x.ID);
            Map(x => x.BasketID);
            Map(x => x.ItemID);
            Map(x => x.ProductID);
            Map(x => x.StoreID);
            Map(x => x.Quantity);
            Map(x => x.Price);
            Map(x => x.ListPrice);
            Map(x => x.GrossTotal);
            Map(x => x.VariantCode);
            Map(x => x.VariantPrice);
            Map(x => x.VariantDescription);
            Map(x => x.DateCreated);
            Map(x => x.DateModified);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class CampaignMap : TableMapper<Campaign>
    {
        public CampaignMap() : base("Campaign")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.ImageInfo);
            Map(x => x.Scope).CustomType<CampaignScope>();
            Map(x => x.ItemScope).CustomType<ItemScope>();
            Map(x => x.DiscountType).CustomType<DiscountType>();
            Map(x => x.Discount);
            Map(x => x.MinimumAmount);
            Map(x => x.MinimumQuantity);
            Map(x => x.OwnerID);
            Map(x => x.FilterInfo);
            Map(x => x.PublishDate);
            Map(x => x.ExpireDays);
            Map(x => x.DateCreated);
            Map(x => x.DateModified);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class CommentMap : TableMapper<Comment>
    {
        public CommentMap() : base("Comment")
        {
            Id(x => x.ID);
            Map(x => x.EvaluatorID);
            Map(x => x.OwnerID);
            Map(x => x.OwnerName);
            Map(x => x.Rating);
            Map(x => x.RelationID);
            Map(x => x.RelationType);
            Map(x => x.DateCreated);
            Map(x => x.DateModified);
            Map(x => x.Value);
            Map(x => x.Title);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class CategoryMap : TableMapper<Category>
    {
        public CategoryMap() : base("Category")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.ImageInfo);
            Map(x => x.ParentID);
            Map(x => x.SortOrder);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class DiscountMap : TableMapper<Discount>
    {
        public DiscountMap() : base("Discount")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.Amount);
            Map(x => x.BasketID);
            Map(x => x.CampaignID);
            Map(x => x.DiscountType).CustomType<DiscountType>();
            Map(x => x.Factor);
            Map(x => x.Total);
            Map(x => x.UserID);
            Map(x => x.Code);
            Map(x => x.IsUsed);
            Map(x => x.ExpireDate);
            Map(x => x.DateCreated);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class FavoriteMap : TableMapper<Favorite>
    {
        public FavoriteMap() : base("Favorite")
        {
            Id(x => x.ID);
            Map(x => x.ProductID);
            Map(x => x.StoreID);
            Map(x => x.UserID);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class FileRelationMap : TableMapper<FileRelation>
    {
        public FileRelationMap() : base("FileRelation")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Path);
            Map(x => x.SortOrder);
            Map(x => x.RelationID);
            Map(x => x.Size).CustomType<FileSize>();
            Map(x => x.RelationType);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class MailTemplateMap : TableMapper<MailTemplate>
    {
        public MailTemplateMap() : base("MailTemplate")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Content);
            Map(x => x.Description);
            Map(x => x.ParentID);
            Map(x => x.Subject);
        }
    }

    public class OrderMap : TableMapper<Order>
    {
        public OrderMap() : base("Order")
        {
            Id(x => x.ID);
            Map(x => x.BasketID);
            Map(x => x.UserID);
            Map(x => x.StoreID);
            Map(x => x.ShippingAddressID);
            Map(x => x.BillingAddressID);
            Map(x => x.ShippingMethodID);
            Map(x => x.Note);
            Map(x => x.BasketTotal);
            Map(x => x.DiscountTotal);
            Map(x => x.ShipmentCost);
            Map(x => x.GrossTotal);
            Map(x => x.DateCreated);
            Map(x => x.DateModified);
            Map(x => x.PaymentType);
            Map(x => x.SubPaymentType);
            Map(x => x.Installment);
            Map(x => x.PaymentCost);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class ProductMap : TableMapper<Product>
    {
        public ProductMap() : base("Product")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.ShortDescription);
            Map(x => x.Description).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.Brand);
            Map(x => x.Barcode);
            Map(x => x.CategoryID);
            Map(x => x.CommentInfo);
            Map(x => x.DateModified);
            Map(x => x.ImageInfo);
            Map(x => x.PropertyInfo);
            Map(x => x.Unit).CustomType<MeasureUnits>();
            Map(x => x.Status).CustomType<Statuses>();
            //HasOne(x => x.Category).ForeignKey("CategoryID");
        }
    }

    public class PropertyRelationMap : TableMapper<PropertyRelation>
    {
        public PropertyRelationMap() : base("PropertyRelation")
        {
            Id(x => x.ID);
            Map(x => x.PropertyValueID);
            Map(x => x.RelationID);
            Map(x => x.Value);
            Map(x => x.SortOrder);
            Map(x => x.RelationType);
            Map(x => x.Status).CustomType<Statuses>();
            //HasOne(x => x.PropertyValue).ForeignKey("PropertyValueID").Cascade.All().PropertyRef(it => it.ID);
            //TODO# : Her relation için sql istek yapıyor, Criteria mı geçsek!
            References(x => x.PropertyValue).Column("PropertyValueID").Cascade.All().LazyLoad(Laziness.False).ReadOnly();
        }
    }

    public class PropertyValueMap : TableMapper<PropertyValue>
    {
        public PropertyValueMap() : base("PropertyValue")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.ParentID);
            Map(x => x.SortOrder);
            Map(x => x.Type).CustomType<PropertyType>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class VariantPropertyMap : TableMapper<VariantProperty>
    {
        public VariantPropertyMap() : base("VariantProperty")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.DisplayName);
            Map(x => x.MaxSelection);
            Map(x => x.IsRequired);
            Map(x => x.TrackStock);
            Map(x => x.SortOrder);
            Map(x => x.DisplayMode).CustomType<PropertyDisplayMode>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class VariantRelationMap : TableMapper<VariantRelation>
    {
        public VariantRelationMap() : base("VariantRelation")
        {
            Id(x => x.ID);
            Map(x => x.RelationID);
            Map(x => x.RelationType);
            Map(x => x.VariantID);
            Map(x => x.VariantValue);
            Map(x => x.Price);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class VariantSelectionMap : TableMapper<VariantSelection>
    {
        public VariantSelectionMap() : base("VariantSelection")
        {
            Id(x => x.ID);
            Map(x => x.RelationID);
            Map(x => x.RelationType);
            Map(x => x.VariantCombination);
            Map(x => x.Stock);
            Map(x => x.Price);
            Map(x => x.PriceInfo);
            Map(x => x.Code);
        }
    }

    public class RegionMap : TableMapper<Region>
    {
        public RegionMap() : base("Region")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.ParentID);
            Map(x => x.SortOrder);
            Map(x => x.Type).CustomType<RegionType>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class ShippingMethodMap : TableMapper<ShippingMethod>
    {
        public ShippingMethodMap() : base("ShippingMethod")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.Price);
            Map(x => x.RegionInfo);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class SimpleItemMap : TableMapper<SimpleItem>
    {
        public SimpleItemMap() : base("SimpleItem")
        {
            Id(x => x.ID);
            Map(x => x.Title);
            Map(x => x.Value);
            Map(x => x.Url);
            Map(x => x.SubType);
            Map(x => x.Cost);
            Map(x => x.SortOrder);
            Map(x => x.Type).CustomType<ItemType>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class SliderItemMap : TableMapper<SliderItem>
    {
        public SliderItemMap() : base("SliderItem")
        {
            Id(x => x.ID);
            Map(x => x.Title);
            Map(x => x.SubTitle);
            Map(x => x.ImagePath);
            Map(x => x.Url);
            Map(x => x.SortOrder);
            Map(x => x.Type).CustomType<SliderType>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class BrandMap : TableMapper<Brand>
    {
        public BrandMap() : base("Brand")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.ImagePath);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class StoreItemMap : TableMapper<StoreItem>
    {
        public StoreItemMap() : base("StoreItem")
        {
            Id(x => x.ID);
            Map(x => x.SalesPrice);
            Map(x => x.ListPrice);
            Map(x => x.Stock);
            Map(x => x.StoreID);
            Map(x => x.ProductID);
            Map(x => x.DateModified);
            Map(x => x.IsForSale);
            Map(x => x.HasVariant);
            Map(x => x.Status).CustomType<Statuses>();
            References(x => x.Product).Column("ProductID").ForeignKey("ProductID").Fetch.Join().ReadOnly();
        }
    }

    public class StoreMap : TableMapper<Store>
    {
        public StoreMap() : base("Store")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.DisplayName);
            Map(x => x.Description);
            Map(x => x.ImageInfo);
            Map(x => x.MinOrderAmount);
            Map(x => x.AllowSocialShare);
            Map(x => x.RegionInfo);
            Map(x => x.Guid);
            Map(x => x.ParentID);
            Map(x => x.CustomerID);
            Map(x => x.WorkTimeInfo);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class UserMap : TableMapper<User>
    {
        public UserMap() : base("User")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Email);
            Map(x => x.Password);
            Map(x => x.BirthDate);
            Map(x => x.Gender).CustomType<GenderType>();
            Map(x => x.CustomerID);
            Map(x => x.TitleID);
            Map(x => x.Permissions);
            Map(x => x.Status).CustomType<Statuses>();
            Map(x => x.Role).CustomType<UserRole>();
            Map(x => x.DateModified);
        }
    }

    public class CustomerMap : TableMapper<Customer>
    {
        public CustomerMap() : base("Customer")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Logo);
            Map(x => x.DateCreated);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class TaskDefinitionMap : TableMapper<TaskDefinition>
    {
        public TaskDefinitionMap() : base("TaskDefinition")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.WorkTimesInfo);
            Map(x => x.TriggerDefinition);
            Map(x => x.LastBegin);
            Map(x => x.LastEnd);
            Map(x => x.LastMessage);
            Map(x => x.Type).CustomType<TaskType>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class OnlineSalesMap : TableMapper<OnlineSales>
    {
        public OnlineSalesMap() : base("OnlineSales")
        {
            Id(x => x.ID);
            Map(x => x.PosID);
            Map(x => x.UserID);
            Map(x => x.OrderID);
            Map(x => x.Email);
            Map(x => x.AuthCode);
            Map(x => x.Code);
            Map(x => x.ReasonCode);
            Map(x => x.RetrefNum);
            Map(x => x.ProvDate);
            Map(x => x.HashData);
            Map(x => x.CurrencyCode).CustomType<CurrencyCode>();
            Map(x => x.IPAddress);
            Map(x => x.Amount);
            Map(x => x.Installment);
            Map(x => x.Message);
            Map(x => x.ErrorMsg);
            Map(x => x.SysErrMsg);
            Map(x => x.IsSuccess);
            Map(x => x.Type).CustomType<PaymentType>();
        }
    }

    public class PosDefinitionMap : TableMapper<PosDefinition>
    {
        public PosDefinitionMap() : base("Payment_PosDefinition")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.TerminalID);
            Map(x => x.UserID);
            Map(x => x.Password);
            Map(x => x.RefundUserID);
            Map(x => x.RefundPassword);
            Map(x => x.MerchantID);
            Map(x => x.StoreKey);
            Map(x => x.PostUrl);
            Map(x => x.XmlUrl);
            Map(x => x.IsTest);
            Map(x => x.SuccessUrl);
            Map(x => x.ErrorUrl);
            Map(x => x.ImageUrl);
            Map(x => x.PosType).CustomType<PosType>();
            Map(x => x.PaymentMethod).CustomType<PosPaymentMethod>();
        }
    }

    public class BankInfoMap : TableMapper<BankInfo>
    {
        public BankInfoMap() : base("Payment_BankInfo")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.BranchName);
            Map(x => x.IBAN);
            Map(x => x.AccountNumber);
            Map(x => x.ImageUrl);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class InstallmentInfoMap : TableMapper<InstallmentInfo>
    {
        public InstallmentInfoMap() : base("Payment_InstallmentInfo")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.PosID);
            Map(x => x.Count);
            Map(x => x.Commission);
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class NotificationMap : TableMapper<Notification.Notification>
    {
        public NotificationMap()
            : base("Notification")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Content);
            Map(x => x.TargetList);
            Map(x => x.Expire);
            Map(x => x.PublishDate);
            Map(x => x.Target).CustomType<NotificationTarget>();
            Map(x => x.DisplayMode).CustomType<NotificationDisplayMode>();
            Map(x => x.Status).CustomType<Statuses>();
        }
    }

    public class LookupMap : TableMapper<Lookup>
    {
        public LookupMap() : base("Lookup")
        {
            Id(x => x.ID);
            Map(x => x.Name);
            Map(x => x.Type);
            Map(x => x.Value);
            Map(x => x.Order).Column(ToColumnName("Order"));
            Map(x => x.IsActive);
            Map(x => x.LangID).Column(ToColumnName("Lang"));
        }
    }

    public class ItemMap : ClassMap<Item>
    {
        public ItemMap()
        {
            Id(x => x.ID);
            Map(x => x.Value).Column("Value");
        }
    }

    public class TableMapper<T> : ClassMap<T> where T : class
    {
        public TableMapper(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                tableName = ToTableName(tableName);
                Table(tableName);
            }
        }

        public string ToTableName(string tableName)
        {
            var dbType = Aware.Util.Config.DatabaseType;
            switch (dbType)
            {
                case DatabaseType.MsSQL:
                    return string.Format("[{0}]", tableName);
                case DatabaseType.MySQL:
                    return string.Format("`{0}`", tableName.ToLowerInvariant());
            }
            return tableName;
        }

        public string ToColumnName(string columnName)
        {
            var dbType = Aware.Util.Config.DatabaseType;
            switch (dbType)
            {
                case DatabaseType.MsSQL:
                    return string.Format("[{0}]", columnName);
                case DatabaseType.MySQL:
                    return string.Format("{0}", columnName);
            }
            return columnName;
        }
    }
}