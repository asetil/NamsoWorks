using Aware.ECommerce.Enums;
using Aware.ECommerce.Util;
using Aware.Notification;
using Aware.Util.Enums;
using Aware.Util.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aware.Util.View
{
    public static class ViewList222
    {
        private static List<KeyValuePair<string, Item>> _enumValueList;
        private static bool _loaded;
        private static List<KeyValuePair<string, Item>> EnumValueList
        {
            get
            {
                if (!_loaded && _enumValueList == null)
                {
                    LoadEnumValues();
                }
                return _enumValueList;
            }
        }

        public static List<Item> For<T>() where T : struct, IConvertible
        {
            var enumKey = typeof(T).Name;
            return EnumValueList.Where(i => i.Key == enumKey).Select(s => s.Value).ToList();
        }

        public static List<Item> For(string typeName)
        {
            return EnumValueList.Where(i => i.Key == typeName).Select(s => s.Value).ToList();
        }

        public static string Value<T>(T enumKey) where T : struct, IConvertible
        {
            var enumType = typeof(T).Name;
            int enumID = Convert.ToInt32(enumKey);
            var enumValue = EnumValueList.FirstOrDefault(i => i.Key == enumType && i.Value.ID == enumID);
            return !enumValue.Equals(default(KeyValuePair<string, Item>)) ? enumValue.Value.Value : string.Empty;
        }

        public static string Value<T>(string enumType, T enumKey) where T : struct, IConvertible
        {
            int enumID = Convert.ToInt32(enumKey);
            var enumValue = EnumValueList.FirstOrDefault(i => i.Key == enumType && i.Value.ID == enumID);
            return !enumValue.Equals(default(KeyValuePair<string, Item>)) ? enumValue.Value.Value : string.Empty;
        }

        public static void SetStatus()
        {
            var typeName = typeof(Statuses).Name;
            AddEnumValue(typeName, (int)Statuses.Active, "Aktif");
            AddEnumValue(typeName, (int)Statuses.Passive, "Pasif");
        }

        public static void SetUserValues()
        {
            var typeName = "UserStatus";
            AddEnumValue(typeName, (int)Statuses.Active, "Aktif");
            AddEnumValue(typeName, (int)Statuses.Passive, "Pasif");
            AddEnumValue(typeName, (int)Statuses.WaitingActivation, "Aktivasyon Bekleniyor");

            typeName = typeof(UserRole).Name;
            AddEnumValue(typeName, (int)UserRole.SuperUser, "Sistem Yöneticisi");
            AddEnumValue(typeName, (int)UserRole.AdminUser, "Admin Kullanıcısı");
            AddEnumValue(typeName, (int)UserRole.ServiceUser, "Servis Kullanıcısı");
        }

        public static void SetAuthorityValues()
        {
            var typeName = typeof(AuthorityMode).Name;
            AddEnumValue(typeName, (int)AuthorityMode.Single, "Limitsiz");
            AddEnumValue(typeName, (int)AuthorityMode.WithQuota, "Kotalı");

            typeName = typeof(AuthorityType).Name;
            AddEnumValue(typeName, (int)AuthorityType.User_AllowLogin, "Giriş Yapabilir");
            AddEnumValue(typeName, (int)AuthorityType.User_CreateStore, "Mağaza Oluşturma");
            AddEnumValue(typeName, (int)AuthorityType.User_ViewStore, "Mağaza Görüntüleme");
        }

        public static void Clear()
        {
            _enumValueList = new List<KeyValuePair<string, Item>>();
            _loaded = true;
        }

        public static void AddItem(string typeName, Item item)
        {
            _enumValueList.Add(new KeyValuePair<string, Item>(typeName, item));
        }

        private static void LoadEnumValues()
        {
            _enumValueList = _enumValueList ?? new List<KeyValuePair<string, Item>>();
            _loaded = true;

            SetStatus();
            SetUserValues();
            SetAuthorityValues();

            var typeName = typeof(CommentStatus).Name;
            AddEnumValue(typeName, (int)CommentStatus.WaitingApproval, "Onay Bekleyen");
            AddEnumValue(typeName, (int)CommentStatus.Rejected, "Reddedildi");
            AddEnumValue(typeName, (int)CommentStatus.Approved, "Onaylanmış");

            typeName = typeof(RatingStar).Name;
            AddEnumValue(typeName, (int)RatingStar.OneStar, "1 Yıldızlı");
            AddEnumValue(typeName, (int)RatingStar.TwoStar, "2 Yıldızlı");
            AddEnumValue(typeName, (int)RatingStar.ThreeStar, "3 Yıldızlı");
            AddEnumValue(typeName, (int)RatingStar.FourStar, "4 Yıldızlı");
            AddEnumValue(typeName, (int)RatingStar.FiveStar, "5 Yıldızlı");

            typeName = typeof(CampaignScope).Name;
            AddEnumValue(typeName, (int)CampaignScope.Basket, "Alışveriş Sepetine Özgü");
            AddEnumValue(typeName, (int)CampaignScope.Item, "Ürüne Özgü");
            AddEnumValue(typeName, (int)CampaignScope.Membership, "Yeni Üye");
            AddEnumValue(typeName, (int)CampaignScope.InviteFriend, "Arkadaşını Davet Et");
            AddEnumValue(typeName, (int)CampaignScope.OpportunityItems, "Fırsat Ürünleri");

            typeName = typeof(DiscountType).Name;
            AddEnumValue(typeName, (int)DiscountType.Amount, "Tutar Bazlı");
            AddEnumValue(typeName, (int)DiscountType.Rate, "Oran Bazlı");
            AddEnumValue(typeName, (int)DiscountType.GiftItem, "İndirimli Ürün");
            AddEnumValue(typeName, (int)DiscountType.FixedPriceItem, "Sabit Fiyatlı Ürün");
            AddEnumValue(typeName, (int)DiscountType.Shipping, "İndirimli Kargo");
            AddEnumValue(typeName, (int)DiscountType.FixedPriceShipping, "Sabit Fiyatlı Kargo");
            AddEnumValue(typeName, (int)DiscountType.CouponAsAmount, "İndirim Kodu (Tutar)");
            AddEnumValue(typeName, (int)DiscountType.CouponAsRate, "İndirim Kodu (Oran)");

            typeName = "OrderStatus";
            AddEnumValue(typeName, (int)OrderStatuses.WaitingCustomerApproval, "Müşteri Onayı Bekliyor");
            AddEnumValue(typeName, (int)OrderStatuses.WaitingPayment, "Ödeme Bekliyor");
            AddEnumValue(typeName, (int)OrderStatuses.WaitingApproval, "Onay Bekliyor");
            AddEnumValue(typeName, (int)OrderStatuses.PreparingOrder, "Hazırlanıyor");
            AddEnumValue(typeName, (int)OrderStatuses.ShippingOrder, "Kargoya Verildi");
            AddEnumValue(typeName, (int)OrderStatuses.DeliveredOrder, "Teslim Edildi");
            AddEnumValue(typeName, (int)OrderStatuses.CancelledOrder, "İptal Edildi");
            AddEnumValue(typeName, (int)OrderStatuses.ReturnedOrder, "İade Edildi");

            typeName = typeof(PropertyType).Name;
            AddEnumValue(typeName, (int)PropertyType.PropertyGroup, "Özellik Grubu");
            AddEnumValue(typeName, (int)PropertyType.Text, "Metin");
            AddEnumValue(typeName, (int)PropertyType.Selection, "Seçimlik");
            AddEnumValue(typeName, (int)PropertyType.YesNo, "Var/Yok");
            AddEnumValue(typeName, (int)PropertyType.Html, "Geniş Metin");
            AddEnumValue(typeName, (int)PropertyType.Numeric, "Numerik");
            AddEnumValue(typeName, (int)PropertyType.Float, "Ondalık Sayı");
            AddEnumValue(typeName, (int)PropertyType.Date, "Tarih");

            typeName = Constants.YesNoOptions;
            AddEnumValue(typeName, 1, "Evet");
            AddEnumValue(typeName, 0, "Hayır");

            typeName = Constants.HasNoOptions;
            AddEnumValue(typeName, 1, "Var");
            AddEnumValue(typeName, 2, "Yok");

            typeName = typeof(MeasureUnits).Name;
            AddEnumValue(typeName, (int)MeasureUnits.Unit, "Adet");
            AddEnumValue(typeName, (int)MeasureUnits.Kg, "Kg");
            AddEnumValue(typeName, (int)MeasureUnits.Gram, "Gram");

            typeName = typeof(PropertyDisplayMode).Name;
            AddEnumValue(typeName, (int)PropertyDisplayMode.Checkbox, "Çoklu Seçim");
            AddEnumValue(typeName, (int)PropertyDisplayMode.RadioGroup, "Radyo Buton");
            AddEnumValue(typeName, (int)PropertyDisplayMode.Dropdown, "Dropdown Seçim");
            AddEnumValue(typeName, (int)PropertyDisplayMode.ButtonGroup, "Buton Grubu");

            typeName = typeof(PosType).Name;
            AddEnumValue(typeName, (int)PosType.GarantiPos, "Garanti Bankası");
            AddEnumValue(typeName, (int)PosType.AkbankPos, "AkBank");

            typeName = typeof(PosPaymentMethod).Name;
            AddEnumValue(typeName, (int)PosPaymentMethod.XmlApi, "XML API");
            AddEnumValue(typeName, (int)PosPaymentMethod.Secure3D_PAY, "3D Güvenli Ödeme");
            AddEnumValue(typeName, (int)PosPaymentMethod.Secure3D_OOS, "3D Ortak Ödeme Sayfası");
            AddEnumValue(typeName, (int)PosPaymentMethod.OOS_PAY, "Ortak Ödeme Sayfası");

            typeName = "PaymentTypes";
            AddEnumValue(typeName, (int)OrderSettingsType.PaymentAtDoor, "Kapıda Ödeme");
            AddEnumValue(typeName, (int)OrderSettingsType.PaymentWithCreditCardAtDoor, "Kapıda Kredi Kartı ile");
            AddEnumValue(typeName, (int)OrderSettingsType.PaymentWithRemittance, "Havale ile");
            AddEnumValue(typeName, (int)OrderSettingsType.PaymentWithCreditCard, "Kredi Kartı ile");

            typeName = typeof(NotificationTarget).Name;
            AddEnumValue(typeName, (int)NotificationTarget.All, "Tümü");
            AddEnumValue(typeName, (int)NotificationTarget.Members, "Üyelere");
            AddEnumValue(typeName, (int)NotificationTarget.NonMember, "Ziyaretçilere");
            AddEnumValue(typeName, (int)NotificationTarget.ExcludeMembers, "Seçilenler Hariç");

            typeName = typeof(NotificationDisplayMode).Name;
            AddEnumValue(typeName, (int)NotificationDisplayMode.Message, "Mesaj");
            AddEnumValue(typeName, (int)NotificationDisplayMode.Popup, "Popup");
        }

        private static void AddEnumValue(string typeName, int id, string value)
        {
            _enumValueList.Add(new KeyValuePair<string, Item>(typeName, Item.New(id, value)));
        }
    }
}
