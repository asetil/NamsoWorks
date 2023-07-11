//namespace Aware.Util.Model
//{
//    public class Result 
//    {
//        public byte IsSuccess { get; set; }
//        public bool OK { get { return IsSuccess == 1; } }
//        public string Message { get; set; }
//        public object Value { get; set; }
//        public int ResultCode { get; set; }

//        public T ValueAs<T>() where T : class 
//        {
//            var result = Value as T;
//            return result;
//        }

//        public static Result Success(object value = null, string message = "", int resultCode=0)
//        {
//            return new Result(1, message, value) { ResultCode = resultCode };
//        }

//        public static Result Error(int resultCode,string message = "İşlem Başarısız", object value = null)
//        {
//            return new Result(0, message,value) {ResultCode = resultCode};
//        }

//        public static Result Error(string message = "İşlem Başarısız", object value = null)
//        {
//            return new Result(0, message, value);
//        }

//        public Result()
//        {
//            IsSuccess = 1;
//            Message = string.Empty;
//        }

//        public Result(byte success, string message = "", object value = null)
//        {
//            IsSuccess = success;
//            Message = message;
//            Value = value;
//        }
//    }
//}
