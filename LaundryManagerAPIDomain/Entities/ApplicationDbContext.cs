using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Entities
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Laundry> Laundries { get; set; }
        DbSet<EmployeeInTransit> EmployeesInTransit { get; set; }
        DbSet<Customer> Customers { get; set; }

    }
}
