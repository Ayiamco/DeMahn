using LaundryManagerAPIDomain.Contracts;
using LaundryManagerAPIDomain.Entities;
using LaundryManagerAPIDomain.Services;
using LaundryManagerAPIDomain.Services.EmailService;
using LaundryManagerWebUI.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryManagerWebUI.Interfaces;
using AutoMapper;

namespace LaundryManagerWebUI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeInTransitQuery _employeeInTransitRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEmailSender _mailService;
        private readonly IIdentityQuery _identityRepo;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeInTransitQuery emploeyeeInTransitRepo,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IEmailSender mailService,
            IIdentityQuery IdentityRepo,
            IMapper mapper)
        {
            _employeeInTransitRepo = emploeyeeInTransitRepo;
            _unitOfWork = unitOfWork;
            _config = config;
            _mailService = mailService;
            _identityRepo = IdentityRepo;
            _mapper = mapper;
        }
        public async Task<ServiceResponse> AddEmployeeToTransit(EmployeeInTransitDto model)
        {
            var currentLaundryEmployeesEmail = _identityRepo.GetLaundryEmployeesEmail(model.LaundryId);
            if (currentLaundryEmployeesEmail.Contains(model.Username)) return new ServiceResponse
            {
                Result = AppServiceResult.Failed,
                Data = JsonConvert.SerializeObject(new { errors = new 
                { 
                    username = new string[] { "Username already exist" } 
                }})
            };

            var employee = new EmployeeInTransit { LaundryId = model.LaundryId, Username = model.Username };
            await _employeeInTransitRepo.Create(employee);
            await _unitOfWork.SaveAsync();
            var url = _config[AppConstants.ClientBaseUrl] + $"?id={employee.Id},username={model.Username}";
            var message = new Message(new List<string> { model.Username },
                model.LaundryName + " Employee Registration",
                $"<div> <h4>Hi,</h4><p>Please click <a href='{url}'>here</a> " +
                $"to complete your employee registration for {model.LaundryName} Laundry</p> </div>"
                );
            await _mailService.SendEmailAsync(message, IsHTML: true);

            return new ServiceResponse
            {
                Result = AppServiceResult.Succeeded,
                Data = JsonConvert.SerializeObject(new
                {
                    Message = "Employee registration link sent successfully"
                })
            };
        }

        public ServiceResponse GetEmployee(string userId)
        {
            var user = _identityRepo.GetUserWithProfile(userId);
            if(user == null) return new ServiceResponse
            {
                Result=AppServiceResult.Failed,
                Data= JsonConvert.SerializeObject(new
                {
                    errors = new { userId = new string[] { "user was not found" } }
                })
             };

            return new ServiceResponse
            {
                Result = AppServiceResult.Succeeded,
                Data = JsonConvert.SerializeObject(new
                {
                   data = _mapper.Map<EmployeeDto>(user)
                })
            };
            
        }
    }
}
