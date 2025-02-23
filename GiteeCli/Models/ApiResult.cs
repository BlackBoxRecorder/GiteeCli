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
            if (code != 0)
            {
                Message = $"StatusCode: {code}，{message}";
            }
            else
            {
                Message = message;
            }
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; } = default!;
    }
}
