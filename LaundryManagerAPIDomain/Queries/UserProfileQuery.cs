using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaundryManagerAPIDomain.Queries
{
    public class UserProfileQuery:GenericQuery<UserProfile,int>,IUserProfileQuery
    {
        public UserProfileQuery(ApplicationDbContext context):base(context)
        {

        }
    }
}
