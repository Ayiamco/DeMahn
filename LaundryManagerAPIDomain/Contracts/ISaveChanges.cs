using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface ISaveChanges
    {
        public Task SaveAsync();
        public void  Save();
    }
}
