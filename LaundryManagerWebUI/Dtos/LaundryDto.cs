using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class LaundryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
        public int AddressId { get; set; }
        public int EmployeeCount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CustomerCount { get; set; }
    }
}
