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

namespace LaundryManagerWebUI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeInTransitQuery _employeeInTransitRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IEmailSender _mailService;

        public EmployeeService(IEmployeeInTransitQuery emploeyeeInTransitRepo,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IEmailSender mailService)
        {
            _employeeInTransitRepo = emploeyeeInTransitRepo;
            _unitOfWork = unitOfWork;
            _config = config;
            _mailService = mailService;
        }
        public async Task<ServiceResponse> AddEmployeeToTransit(EmployeeInTransitDto model)
        {
            var employee = new EmployeeInTransit { LaundryId = model.LaundryId, Username = model.Username };
            await _employeeInTransitRepo.Create(employee);
            await _unitOfWork.SaveAsync();
            var url = _config[AppConstants.ClientBaseUrl] + $"?id={employee.Id},username={model.Username}";
            var message = new Message(new List<string> { model.Username },
                model.LaundryName + " Employee Registration",
                $"<div> <h4>Hi,</h4><p>Please click <a href='{url}'>here</a> to complete your employee registration for {model.LaundryName} Laundry</p> </div>"
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
    }
}
