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
    public class LaundryService : ILaundryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILaundryQuery _laundryRepo;
        private readonly IMapper _mapper;

        public LaundryService(IUnitOfWork unitOfWork,
            ILaundryQuery laundryRepo, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _laundryRepo = laundryRepo;
            _mapper = mapper;
           
        }

        public async Task<ServiceResponse> GetLaundry(Guid id,bool IsIdentityId=false)
        {
            try
            {
                Laundry laundry;
                if (IsIdentityId) laundry = await _laundryRepo.GetLaundryByUserId(id);
                else laundry = await _laundryRepo.Read(id);

                if (laundry == null) return new ServiceResponse
                {
                    Result = AppServiceResult.Failed,
                    Data = JsonConvert.SerializeObject(new
                    {
                        Errors = new
                        {
                            Laundry = new string[] { "laundry was not found" }
                        }
                    })


                };

                var laundryDto = _mapper.Map<LaundryDto>(laundry);
                return new ServiceResponse
                {
                    Result = AppServiceResult.Succeeded,
                    Data = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            Laundry = laundryDto
                        }
                    })


                };
            }
            catch(Exception e)
            {
                //log error
                return new ServiceResponse
                {
                    Result = AppServiceResult.Unknown,
                    Data = JsonConvert.SerializeObject(new
                    {
                        Errors = new
                        {
                            Server = new string[] {"internal server error"}
                        }
                    })


                };
            }
            
        }

       
    }
}
