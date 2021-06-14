using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Queries
{
    public class CustomerQuery:GenericQuery<Customer,Guid>
    {
        public CustomerQuery(ApplicationDbContext context):base(context)
        {

        }
    }
}
