using Microsoft.AspNetCore.Identity;
using System;

namespace LaundryManagerAPIDomain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public UserProfile Profile { get; set; }
        public int ProfileId {get;set;}
        public string RefreshToken { get; set; }
        public Guid LaundryId { get; set; }
        public Laundry Laundry { get; set; }
    }
}
