namespace Aware.BL.Model
{
    public class OperationResult<T>
    {
        public bool Ok { get; private set; }

        public string Code { get; private set; }

        public string Message { get; private set; }

        public T Value { get; private set; }
        //public List<ValidationItem> ValidationInfo { get; set; }

        public static OperationResult<T> Success(T value = default(T))
        {
            return new OperationResult<T>()
            {
                Ok = true,
                Value = value
            };
        }

        public static OperationResult<T> Success(string code, T value = default(T))
        {
            return new OperationResult<T>()
            {
                Ok = true,
                Code = code,
                Value = value
            };
        }

        public static OperationResult<T> Error(string code, T value = default(T))
        {
            return new OperationResult<T>()
            {
                Ok = false,
                Code = code,
                Value = value
            };
        }

        public OperationResult<T> SetValue(T value)
        {
            Value = value;
            return this;
        }
    }
}
