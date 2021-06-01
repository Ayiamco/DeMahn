using AutoMapper;
using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerWebUI.Interfaces;
using LaundryManagerWebUI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPI.UnitTests.LaundryServiceTests
{
    [TestFixture]
    public class GetLaundry
    {
        private Mock<ILaundryQuery> _laundryRepo;
        private IMapper _mapper;
        private ILaundryService _laundryService;

        [SetUp]
        public void Setup()
        {
            _laundryRepo = new Mock<ILaundryQuery>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();

            _laundryService = new LaundryService(
                unitOfWorkMock.Object,
                _laundryRepo.Object,
                mapperMock.Object);
        }

        [Test]
        public async Task LaundryDoesNotExist_ReturnsAppServiceFailed()
        {
            //Arrange
            var dummyLaundryId = new Guid("dddddddddddddddddddddddddddddddd");
            _laundryRepo.Setup(r  => r.Read(dummyLaundryId)).Returns<Laundry>(x=> Task.FromResult<Laundry>(null));

            //Act
            var result = await _laundryService.GetLaundry(dummyLaundryId);

            //Assert
            _laundryRepo.Verify(x => x.Read(dummyLaundryId));
            Assert.That(result.Result, Is.EqualTo(AppServiceResult.Failed));
        }

    }
}
