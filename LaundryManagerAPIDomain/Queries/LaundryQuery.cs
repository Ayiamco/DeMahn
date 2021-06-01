using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Queries
{
    public class LaundryQuery : GenericQuery<Laundry, Guid>, ILaundryQuery
    {
        public LaundryQuery(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<Laundry> GetLaundryByUserId(Guid userId)
        {
           var user=  await _context.Set<ApplicationUser>()
                .Include(x=> x.Profile)
                .ThenInclude(x=>x.Laundry)
                .ThenInclude(x=>x.Address)
                .AsQueryable()
                .FirstAsync(x=> x.Id==userId.ToString());
            return user?.Profile?.Laundry;
        }
    }
}
