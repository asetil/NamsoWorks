using System;

namespace Aware.ECommerce.Enums
{
    public enum OrderStatuses
    {
        None = 0,
        WaitingCustomerApproval = 1,
        WaitingPayment = 2,
        WaitingApproval = 3,
        PreparingOrder = 4,
        ShippingOrder = 5,
        DeliveredOrder = 6,
        CancelledOrder = 7,
        ReturnedOrder = 8,
    }

    public enum SiteSettingsType
    {
        AllowNewComment = 1,
        AllowSocialLogin = 2,
        AllowSocialShare = 3,
        MailHost = 4,
        MailPort = 5,
        MailUser = 6,
        MailPassword = 7,
        DisplayComments = 8,
        AllowProductCompare = 9,
        ShowProductInstallments = 10,
        UseMultiLanguage = 11,

        FacebookApiKey = 12,
        FacebookApiSecret = 13,
        GoogleApiKey = 14,
        GoogleApiSecret = 15,
        ReCaptchaSecret = 16
    }

    public enum OrderSettingsType
    {
        PaymentAtDoor = 1,
        PaymentWithCreditCardAtDoor = 2,
        PaymentWithCreditCard = 3,
        PaymentWithRemittance = 4,
        PaymentWithGarantiPay = 5,

        AllowShipping = 21,
        AllowShippingTrack = 22,
        UseOOSPayment = 30,
        DefaultPos = 31,

    }

    public enum PaymentType
    {
        None = 0,
        AtDoor = 1,
        CreditCardAtDoor = 2,
        CreditCard = 3,
        Remittance = 4,
        GarantiPay = 5
    }

    public enum UserRole
    {
        User = 0,
        AdminUser = 1,
        SuperUser = 3,
        ServiceUser = 4 //Dışarıya açtığım servislere istek yapabilir
    }

    public enum TaskStatus
    {
        Waiting = 0,
        Sleep = 1,
        Controling = 2,
        Executing = 3,
        Failed = 4,
        Success = 5
    }

    public enum CommentStatus
    {
        None = 0,
        Approved = 1,
        Rejected = 2,
        WaitingApproval = 3
    }

    public enum RatingStar
    {
        None = 0,
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3,
        FourStar = 4,
        FiveStar = 5
    }

    public enum AuthenticationMailType
    {
        ActivationMail = 0,
        ForgotPasswordMail = 1
    }

    public enum MeasureUnits
    {
        Unit = 0,
        Gram = 1,
        Kg = 2
    }

    public enum CampaignScope
    {
        None = 0,
        Basket = 1,
        Item = 2,
        Membership = 3,
        InviteFriend = 4,
        OpportunityItems = 5 //Fırsat ürünleri kampanyası ???
    }

    public enum ItemScope
    {
        All = 0,
        OnlySelecteds = 1,
        ExcludeSelecteds = 2,
        FreeBuy = 3, // bir alana bir bedava kampanyası için,
        Once = 4
    }

    public enum DiscountType
    {
        Rate = 0,
        Amount = 1,
        GiftItem = 2,
        FixedPriceItem = 3,
        Shipping = 4,
        FixedPriceShipping = 5,
        CouponAsAmount = 6,
        CouponAsRate = 7
    }

    public enum LogMode
    {
        Disabled = 0,
        Enabled = 1,
        Detailed = 2
    }

    public enum TriggerType
    {
        RunOnce = 0,
        RunDaily = 1,
        RunWeekly = 2
    }

    [Flags]
    public enum DaysOfTheWeek : short
    {
        Sunday = 0x1,
        Monday = 0x2,
        Tuesday = 0x4,
        Wednesday = 0x8,
        Thursday = 0x10,
        Friday = 0x20,
        Saturday = 0x40
    }

    public enum ValidatorType
    {
        NullCheck = 1,
        Email = 2,
        Selection = 3,
        CompareFieldEquality = 4, //İki alan aynı değere sahip olmalı
        CompareFieldUnEquality = 5, //İki alan farklı değere sahip olmalı
        Checkbox = 6,
        PositiveNumber = 7
    }

    public enum PropertyType
    {
        PropertyGroup = 1,
        Text = 2,
        Selection = 3,
        Html = 4,
        Numeric = 5,
        Date = 6,
        Currency = 7,
        Float = 8,
        PropertyOption = 9,
        VariantProperty = 10,
        VariantOption = 11,
        YesNo = 12
    }

    public enum PropertyDisplayMode
    {
        Checkbox = 1,
        RadioGroup = 2,
        Dropdown = 3,
        ButtonGroup = 4
    }

    public enum ItemType
    {
        None = 0,
        SiteSettings = 2,
        UserPermissions = 3,
        OrderSettings = 5
    }

    public enum FilterType
    {
        Category = 1,
        Brand = 2,
        Size = 3
    }

    public enum TaskType
    {
        None = 0,
        ElasticIndexer = 1,
        ProductRefresh = 2
    }

    public enum FileSize
    {
        None = 0,
        Small = 1,
        Medium = 2,
        Large = 3
    }

    public enum CategoryHierarchy
    {
        None = 0,
        OnlyChildren = 1,
        AllDescendants = 2
    }
}
