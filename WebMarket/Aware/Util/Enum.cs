namespace Aware.Util.Enums
{
    public enum Statuses
    {
        //0-100 : General Statuses
        None = 0,
        Passive = 2,
        Active = 1,
        Deleted = 3,
        Rejected = 4,
        WaitingApproval = 5,

        //2000-2100 : User Stauses
        PasswordMustChanged = 2000,
        WaitingActivation = 2001,

        //3100-3200 : BasketStatus
        OrderedBasket = 3100
    }

    public enum CurrencyCode
    {
        TRL = 949,
        USD = 840,
        EURO = 978
    }

    public enum PosType
    {
        None = 0,
        GarantiPos = 1,
        AkbankPos = 2,
        Isbank = 3
    }

    public enum PosPaymentMethod
    {
        None = 0,
        XmlApi = 1,
        Secure3D = 2,
        Secure3D_PAY = 3,
        Secure3D_OOS = 4,
        OOS_PAY = 5,
        GarantiPAY = 6
    }

    public enum FieldType
    {
        TextBox = 0,
        Password = 1,
        Label = 2,
        CheckBox = 3,
        Textarea = 4,
        NumberBox = 5,
        Link = 8,
        File = 9,
        Image = 10,
        Select = 11,
        Button = 12,
        ButtonGroup = 13
    }

    public enum FieldDirection
    {
        Horizantal = 0,
        Vertical = 1,
        Inline = 2,
        JustField = 3,
    }

    public enum TicketType
    {
        None = 0,
        Authorize = 1
    }

    public enum RegionType
    {
        None = 0,
        City = 1,
        County = 2,
        District = 3,
    }

    public enum StripOptions
    {
        Default,
        WithoutLineBreaks,
        WithoutLineBreaksAndDecode
    }

    public enum TransactionType
    {
        Sales = 1,
        Cancel = 2,
        Refund = 3,
        PreSale = 4, //Ön otorizasyon
        PostSale = 5 //Ön otorizasyon kapama
    }

    public enum AuthorityType
    {
        None = 0,
        User_AllowLogin = 1,
        User_CreateStore = 2,
        User_ViewStore = 3,
        Check = 4
    }

    public enum AuthorityMode
    {
        Single = 0,
        WithQuota = 1
    }

    public enum GenderType
    {
        None = 0,
        Male = 1,
        Female = 2
    }

    public enum ElasticStatus
    {
        Check = 0,
        Active = 1,
        NotAvailable = 2
    }

    public enum LayoutDirection
    {
        None = 0,
        Vertical = 1,
        Horizantal = 2
    }

    public enum JoinMode
    {
        None = -666,
        InnerJoin = 0,
        LeftOuterJoin = 1,
        RightOuterJoin = 2,
        FullJoin = 4
    }

    public enum AuthorizeLevel
    {
        None = 0,
        Authenticated = 1,
        Admin = 2,
        SuperUser = 3
    }
    public enum ViewTypes
    {
        Editable = 1,
        Read = 2,
        UnAuthorized = 3
    }

    public enum GalleryType
    {
        Mixed = 0,
        Image = 1,
        Pdf = 2
    }

    public enum RelationTypes
    {
        Product = 1,
        Store = 2,
        Category = 3,
        Property = 4,
        Campaign = 5,
        User = 6,
        StoreItem = 7,
        BasketItem = 8,
        Slider = 9,
        Customer = 10
    }

    public enum SliderType
    {
        None = 0,
        Main = 1
    }
}
