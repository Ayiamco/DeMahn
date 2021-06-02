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
        private Guid dummyLaundryId ;
        private Laundry dummyLaundry;

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

            dummyLaundryId = new Guid("dddddddddddddddddddddddddddddddd");
            dummyLaundry = new Laundry() { Id = dummyLaundryId };
        }

        [Test]
        public async Task IdIsLaundryIdAndLaundryDoesNotExist_ReturnsAppServiceFailed()
        {
            //Arrange
            _laundryRepo.Setup(r  => r.Read(dummyLaundryId)).ReturnsAsync((Laundry)null);
            

            //Act
            var result = await _laundryService.GetLaundry(dummyLaundryId);

            //Assert
            _laundryRepo.Verify(x => x.Read(dummyLaundryId));
            _laundryRepo.Verify(x => x.GetLaundryByUserId(dummyLaundryId), Times.Never);
            Assert.That(result.Result, Is.EqualTo(AppServiceResult.Failed));
        }
        
        [Test]
        public async Task IdIsUserIdAndLaundryDoesNotExist_ReturnsAppServiceFailed()
        {
            //Arrange
            _laundryRepo.Setup(r => r.GetLaundryByUserId(dummyLaundryId)).ReturnsAsync((Laundry)null);

            //Act
            var result = await _laundryService.GetLaundry(dummyLaundryId,IsIdentityId:true);

            //Assert
            _laundryRepo.Verify(x => x.GetLaundryByUserId(dummyLaundryId));
            _laundryRepo.Verify(x => x.Read(dummyLaundryId), Times.Never);
            Assert.That(result.Result, Is.EqualTo(AppServiceResult.Failed));
        }



        [Test]
        public async Task IdIsLaundryIdAndLaundryExist_ReturnsAppServiceSucceedede()
        {
            //Arrange
            _laundryRepo.Setup(r => r.Read(dummyLaundryId)).ReturnsAsync(dummyLaundry);

            //Act
            var result = await _laundryService.GetLaundry(dummyLaundryId);

            //Assert
            _laundryRepo.Verify(x => x.Read(dummyLaundryId));
            _laundryRepo.Verify(x => x.GetLaundryByUserId(dummyLaundryId), Times.Never);
            Assert.That(result.Result, Is.EqualTo(AppServiceResult.Succeeded));
        }

        [Test]
        public async Task IdIsUserIdAndLaundryExist_ReturnsAppServiceSucceeded()
        {
            //Arrange
            _laundryRepo.Setup(r => r.GetLaundryByUserId(dummyLaundryId)).ReturnsAsync(dummyLaundry);

            //Act
            var result = await _laundryService.GetLaundry(dummyLaundryId, IsIdentityId: true);

            //Assert
            _laundryRepo.Verify(x => x.GetLaundryByUserId(dummyLaundryId));
            _laundryRepo.Verify(x => x.Read(dummyLaundryId), Times.Never);
            Assert.That(result.Result, Is.EqualTo(AppServiceResult.Succeeded));
        }

    }
}
