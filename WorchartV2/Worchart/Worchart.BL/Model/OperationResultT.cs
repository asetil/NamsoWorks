﻿using System.Collections.Generic;

namespace Worchart.BL.Model
{
    public class OperationResult<T>
    {
        public OperationResult(string code)
        {
            Code = code;
        }

        public bool Success { get { return Code == "000"; } }
        public string Code { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }
        public List<ValidationItem> ValidationInfo { get; set; }
    }
}
