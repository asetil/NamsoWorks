namespace Aware.Util.Exceptions
{
    public class AwareException : System.Exception
    {
        public string Code { get; set; }

        public AwareException(string code)
        {
            Code = code;
        }
    }
}
