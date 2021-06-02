using LaundryManagerAPIDomain.Entities;
using System;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface ILaundryQuery: IGenericQuery<Laundry,Guid>
    {
        Task<Laundry> GetLaundryByUserId(Guid userId);
       
    }
}