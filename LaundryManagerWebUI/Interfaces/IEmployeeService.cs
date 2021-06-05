using LaundryManagerWebUI.Dtos;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public interface IEmployeeService
    {
        Task<ServiceResponse> AddEmployeeToTransit(EmployeeInTransitDto model);
        ServiceResponse GetEmployee(string userId);
        Task<ServiceResponse> UpdateEmployeeProfile(UserProfileDto profile);
    }
}