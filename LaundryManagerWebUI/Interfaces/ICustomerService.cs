﻿using LaundryManagerWebUI.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public interface ICustomerService
    {
        ServiceResponse AddNew(NewCustomerDto model);
    }
}
