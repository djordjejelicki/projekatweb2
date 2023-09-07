using Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
    public class ResponsePackage<T>
    {
        public ResponseStatus Status { get; set; } = ResponseStatus.OK;
        public string Message { get; set; }
        public T Data { get; set; }

        public ResponsePackage() {
            Status = ResponseStatus.OK;
            Message = string.Empty;
        }

        public ResponsePackage(T data,ResponseStatus status = ResponseStatus.OK, string message = "")
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public ResponsePackage(ResponseStatus status, string message)
        {
            Status = status;
            Message = message;
            
        }
    }

   
}
