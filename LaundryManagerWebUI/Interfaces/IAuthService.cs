using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Services;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<AuthServiceResult, string>> Authenticate(LoginDto model);
        Task<ResponseDto<AuthServiceResult, string>> CreateLaundry(RegisterDto model);
    }
}