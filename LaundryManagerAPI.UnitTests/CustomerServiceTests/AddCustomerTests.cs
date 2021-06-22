using AutoMapper;
using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPI.UnitTests.CustomerServiceTests
{
    [TestFixture]
    class AddCustomerTests
    {
        //Employee exist
        //Employee does  not exist
        //Customer email already exist in  laundry
        //Customer email does not already exist in laundry
        private CustomerService service;
        private Mock<ICustomerQuery> _customerRepo;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IIdentityQuery> _identityRepo;
        private Mock<IMapper> _mapper;
        private Customer customer;
        private NewCustomerDto customerDto;
        private Guid dummyUserId;
        private ApplicationUser dummyEmployee;

        [SetUp]
        public void SetUp()
        {
            _customerRepo = new Mock<ICustomerQuery>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _identityRepo = new Mock<IIdentityQuery>();
            _mapper = new Mock<IMapper>();
            dummyUserId = new Guid();
            dummyEmployee = new ApplicationUser { Id = dummyUserId.ToString() };
            customerDto = new NewCustomerDto { EmployeeId = dummyUserId.ToString() };
            customer = new Customer
            {
                EmployeeId = dummyEmployee.Id,
                Username = "dummyemail@dummy.com",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            service = new CustomerService(_customerRepo.Object,
                _unitOfWork.Object,_identityRepo.Object,_mapper.Object);
        }

        [Test]
        public async Task EmployeeExistAndCustomerDoesNotExist_ReturnsSuccess() 
        {
            //Arrange
            
            _identityRepo.Setup(r => r.GetUserWithNavProps(dummyUserId.ToString())).Returns(dummyEmployee);
            _mapper.Setup(r => r.Map<Customer>(customerDto)).Returns(customer);
            _unitOfWork.Setup(r => r.SaveAsync()).ReturnsAsync(1);

            //Act
            var result =await service.AddNew(customerDto);

            //Assert
            _identityRepo.Verify(r => r.GetUserWithNavProps(dummyUserId.ToString()), Times.Once);
            _customerRepo.Verify(r => r.Create(customer),Times.Once);
            _unitOfWork.Verify(r => r.SaveAsync(), Times.Once);
            Assert.That(result.Result.Equals(AppServiceResult.Succeeded));
        }

        [Test]
        public async Task CustomerAlreadyExist_ReturnsFailed()
        {
            //Arrange
            _identityRepo.Setup(r => r.GetUserWithNavProps(dummyUserId.ToString())).Returns(dummyEmployee);
            _mapper.Setup(r => r.Map<Customer>(customerDto)).Returns(customer);
            _customerRepo.Setup(r => r.Find(x => x.LaundryId == dummyEmployee.LaundryId && x.Username == customerDto.Username))
                .Returns(new List<Customer> { customer });
            _unitOfWork.Setup(r => r.SaveAsync()).ReturnsAsync(1);

            //Act
            var result = await service.AddNew(customerDto);

            //Assert
            _identityRepo.Verify(r => r.GetUserWithNavProps(dummyUserId.ToString()), Times.Once);
            _customerRepo.Verify(r => r.Create(customer), Times.Never);
            _customerRepo.Verify(r => r.Find(x => x.LaundryId == dummyEmployee.LaundryId && x.Username == customerDto.Username),
                Times.Once);
            _unitOfWork.Verify(r => r.SaveAsync(), Times.Never);
            Assert.That(result.Result.Equals(AppServiceResult.Failed));
        }

        [Test]
        public async Task EmployeeDoesNotExist_ReturnsFailed()
        {
            //Arrange
            _identityRepo.Setup(r => r.GetUserWithNavProps(dummyUserId.ToString())).Returns((ApplicationUser)null);
            _mapper.Setup(r => r.Map<Customer>(customerDto)).Returns(customer);
            _unitOfWork.Setup(r => r.SaveAsync()).ReturnsAsync(1);

            //Act
            var result = await service.AddNew(customerDto);

            //Assert
            _identityRepo.Verify(r => r.GetUserWithNavProps(dummyUserId.ToString()), Times.Once);
            _customerRepo.Verify(r => r.Create(customer), Times.Never);
            _unitOfWork.Verify(r => r.SaveAsync(), Times.Never);
            Assert.That(result.Result.Equals(AppServiceResult.Failed));
        }

    }
}
