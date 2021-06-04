using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class EmployeeDto
    {
       public UserProfile Profile { get; set; }
       public string Username { get; set; }

    }
}
