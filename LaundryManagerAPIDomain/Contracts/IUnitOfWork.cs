using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface IUnitOfWork
    {
        public Task<int> SaveAsync();
        public int  Save();
    }
}
