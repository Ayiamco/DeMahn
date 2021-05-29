using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class ResponseDto<T1,T2>
    {
        public T1 Result { get; set; }
        public T2 Data { get; set; }
    }
}
