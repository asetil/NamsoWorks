namespace Worchart.BL.Exceptions
{
    public class WorchartException : System.Exception
    {
        public string Code { get; set; }

        public WorchartException(string code)
        {
            Code = code;
        }
    }
}
