using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSQL.Infrastructure
{
    public class StandardBooleanSuccess
    {
        private string _exceptionText;
        public bool IsSuccess { get; set; }
        public object AdditionalData { get; set; }
        public string ExceptionText
        {
            get
            {
                if (IsSuccess == true)
                    return string.Empty;
                return _exceptionText;
            }
            set
            {
                IsSuccess = false; _exceptionText = value;
            }
        }

        public StandardBooleanSuccess(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
        public StandardBooleanSuccess(string exceptionText)
        {
            ExceptionText = exceptionText;
        }
    }
}