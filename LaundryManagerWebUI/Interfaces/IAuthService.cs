using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Services;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<AppServiceResult, string>> Authenticate(LoginDto model);
        Task<ResponseDto<AppServiceResult, string>> CreateLaundry(RegisterDto model);
        Task<ResponseDto<AppServiceResult, string>> RefreshJWtToken(JWTDto model);
        Task SendResetPasswordLink(string username);
        Task<ResponseDto<AppServiceResult, string>> ResetPassword(ConfirmPasswordResetDto model);
    }
}