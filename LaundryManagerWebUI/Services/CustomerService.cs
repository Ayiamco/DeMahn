using LaundryManagerAPIDomain.Contracts;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Interfaces;
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

        public CustomerService(ICustomerQuery customerRepo,
            IUnitOfWork unitOfWork
            )
        {
            this.customerRepo = customerRepo;
            this.unitOfWork = unitOfWork;
        }

        public ServiceResponse AddNew(NewCustomerDto model)
        {
            throw new NotImplementedException();
        }
    }
}
