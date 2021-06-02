using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Queries
{
    public class EmployeeInTransitQuery: GenericQuery<EmployeeInTransit,Guid>,IEmployeeInTransitQuery
    {
        public EmployeeInTransitQuery(ApplicationDbContext context):base(context)
        {
           
        }
    }
}
