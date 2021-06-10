using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface IIdentityQuery
    {
        public string GetUserRole(ApplicationUser user);
        IEnumerable<string> GetLaundryEmployeesEmail(Guid laundryId);
        ApplicationUser GetUserWithNavProps(string userId);
    }
}
