using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface IIdentityQuery
    {
        public string GetUserRole(ApplicationUser user);
    }
}
