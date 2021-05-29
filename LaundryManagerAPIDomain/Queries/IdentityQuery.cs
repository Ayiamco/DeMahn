using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LaundryManagerAPIDomain.Queries
{
    public class IdentityQuery : IIdentityQuery
    {
        private ApplicationDbContext _context;
        public IdentityQuery(ApplicationDbContext _context)
        {
            this._context = _context;
        }


        public string GetUserRole(ApplicationUser user)
        {
            var userRoles = _context.UserRoles.Where(x => x.UserId == user.Id)
                 .Join(_context.Roles, x => x.RoleId, y => y.Id, (x, y) => y.Name)
                 .AsQueryable();
            return string.Join(",", userRoles);
        }

    }
}
