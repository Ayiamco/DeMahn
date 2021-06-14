using AutoMapper;
using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Services
{
    public class CustomerService:ICustomerService
    {
        private readonly ICustomerQuery customerRepo;
        private readonly IUnitOfWork unitOfWork;
        private readonly IIdentityQuery identityRepo;
        private readonly IMapper mapper;

        public CustomerService(ICustomerQuery customerRepo,
            IUnitOfWork unitOfWork,
            IIdentityQuery identityRepo,
            IMapper mapper
            )
        {
            this.customerRepo = customerRepo;
            this.unitOfWork = unitOfWork;
            this.identityRepo = identityRepo;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse> AddNew(NewCustomerDto model)
        {
            var employee=identityRepo.GetUserWithNavProps(model.EmployeeId);
            var response = new ServiceResponse { Result = AppServiceResult.Failed };
            if(employee == null)
            {
                response.Data = JsonConvert.SerializeObject(new { 
                    status="failed",
                    message="employee was not found"
                });
                return response;
            }

            if(CustomerAlreadyExistInLaundry(model.Username,employee.LaundryId))
            {
                response.Data = JsonConvert.SerializeObject(new
                {
                    status = "failed",
                    message = "customer already exist"
                });
                return response;
            }

            var customer = mapper.Map<Customer>(model);
            customer.LaundryId = employee.LaundryId;
            await customerRepo.Create(customer);
            await unitOfWork.SaveAsync();
            response.Result = AppServiceResult.Succeeded;
            return response;
        }

        private bool CustomerAlreadyExistInLaundry(string customerEmail, Guid laundryId)
        {
            var result=customerRepo.Find(x => x.LaundryId == laundryId && x.Username == customerEmail)
                .AsQueryable().ToList();

            return  result.Count > 0;
        }
    }
}
