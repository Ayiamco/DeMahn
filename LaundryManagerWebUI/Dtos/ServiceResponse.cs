using LaundryManagerWebUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class ServiceResponse
    {
        public AppServiceResult Result { get; set; }
        public string Data { get; set; }
    }
}
