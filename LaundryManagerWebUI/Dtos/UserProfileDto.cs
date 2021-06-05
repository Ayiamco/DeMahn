using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Dtos
{
    public class UserProfileDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
        public Gender Gender { get; set; }
        public string UserId { get; set; }
    }
}
