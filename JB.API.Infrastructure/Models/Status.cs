using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JB.Infrastructure.Models
{
    public class Status
    {
        private string message { get; set; }
        private ErrorCode errorCode { get; set; }
        
        public ErrorCode ErrorCode 
        {
            get => errorCode;
            set
            {
                errorCode = value;
                message = EnumHelper.GetDescriptionFromEnumValue(value);
            } 
        }

        public string Message
        {
            get => string.IsNullOrEmpty(message) ? EnumHelper.GetDescriptionFromEnumValue(ErrorCode) : message;
            set => message = value;
        }

        [JsonIgnore]
        public Exception Exception { get; set; }

        public bool IsSuccess { get => ErrorCode == ErrorCode.Success; }

        public Status(ErrorCode errorCode = ErrorCode.Success)
        {
            ErrorCode = errorCode;
        }

        public void SetStatusMessage(ErrorCode errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
