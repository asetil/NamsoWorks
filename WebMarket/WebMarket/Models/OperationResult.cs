namespace WebMarket.Models
{
    public class OperationResult
    {
        public int ResultCode { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string IconClass { get; set; }

        public OperationResult(int code,string title,string iconCss,string content)
        {
            ResultCode = code;
            Title = title;
            IconClass = iconCss;
            Content = content;
        }
    }
}