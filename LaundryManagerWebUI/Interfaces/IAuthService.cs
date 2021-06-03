using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Services;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse> Authenticate(LoginDto model);
        Task<ServiceResponse> CreateLaundry(RegisterDto model);
        Task<ServiceResponse> RefreshJWtToken(JWTDto model);
        Task SendResetPasswordLink(string username);
        Task<ServiceResponse> ResetPassword(ConfirmPasswordResetDto model);
        Task<ServiceResponse> AddEmployee(NewEmployeeDto model);
    }
}