using LaundryManagerWebUI.Dtos;
using System;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Interfaces
{
    public interface ILaundryService
    {
        Task<ServiceResponse> GetLaundry(Guid id, bool IsIdentityId = false);
    }
}