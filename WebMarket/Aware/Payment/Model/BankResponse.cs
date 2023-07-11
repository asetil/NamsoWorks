namespace Aware.Payment.Model
{
    public class BankResponse
    {
        public string Cavv { get; set; }
        public string Eci { get; set; }
        public string Xid { get; set; }
        public string Md { get; set; }
        public int MdStatus { get; set; }
        public string ErrorMessage { get; set; }
        public string MdErrorMessage { get; set; }
    }
}