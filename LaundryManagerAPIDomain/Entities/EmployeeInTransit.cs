using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Entities
{
    public class EmployeeInTransit
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Laundry Laundry { get; set; }
        public Guid LaundryId { get; set; } 
    }
}
