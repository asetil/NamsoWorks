using System.Collections.Generic;

namespace Worchart.BL.Model
{
    public class OperationResult
    {
        public OperationResult(string code)
        {
            Code = code;
        }

        public bool Success { get { return Code == "000"; } }

        public string Code { get; set; }

        public string Message { get; set; }

        public object Value { get; set; }

        public List<ValidationItem> ValidationInfo { get; set; }

        public T ValueAs<T>()
        {
            if (Value != null)
            {
                try
                {
                    return (T)Value;
                }
                catch { }
            }
            return default(T);
        }
    }
}
