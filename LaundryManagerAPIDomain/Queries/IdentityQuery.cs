using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerAPIDomain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Queries
{
    public class LaundryEmployeeDto
    {
        public string Username { get; set; }
        public string RoleId { get; set; }
    }
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

        public IEnumerable<string> GetLaundryEmployeesEmail(Guid laundryId)
        {
            var employeeEmails = _context.Users.Where(x => x.LaundryId == laundryId).AsQueryable()?
                .ToList().Select(x => x.UserName);
            return employeeEmails;
                
        }

        public ApplicationUser GetUserWithProfile(string userId)
        {
            var user = _context.Set<ApplicationUser>()
                .Include(x=>x.Profile)
                .ThenInclude(x=> x.Address)
                .Where(x=> x.Id==userId)
                .AsQueryable().SingleOrDefault();
            return user;
        }

        public ApplicationUser GetUserwithLaundry(string userId)
        {
            var user = _context.Set<ApplicationUser>()
                .Include(x => x.Laundry)
                .ThenInclude(x => x.Address)
                .Where(x => x.Id == userId)
                .AsQueryable().SingleOrDefault();
            return user;
        }

    }
}
