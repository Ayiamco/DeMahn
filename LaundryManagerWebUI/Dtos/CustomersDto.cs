using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class NewCustomerDto
    {
        [EmailAddress]
        public string Username { get; set; }

        public string Name { get; set; }

        public Location Address { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
        public string EmployeeId { get; set; }
    }
}
