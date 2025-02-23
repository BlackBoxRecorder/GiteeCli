using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteeCli.Models
{
    public class ApiResult<T>
        where T : class
    {
        public ApiResult(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; } = default!;
    }
}
