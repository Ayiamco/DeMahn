using LaundryManagerAPIDomain.Entities;
using LaundryManagerAPIDomain.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface IEmployeeInTransitQuery:IGenericQuery<EmployeeInTransit,Guid>
    {
    }
}
