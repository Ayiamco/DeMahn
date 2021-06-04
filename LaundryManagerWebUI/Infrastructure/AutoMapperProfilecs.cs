using AutoMapper;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Infrastructure
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Laundry, LaundryDto>();
            CreateMap<ApplicationUser, EmployeeDto>();
            CreateMap<NewEmployeeDto, ApplicationUser>()
                .ForMember(x => x.Email, y => y.MapFrom(x => x.Username));
        }
    }
}
