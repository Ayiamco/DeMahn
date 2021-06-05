using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface IGenericQuery<T1, T2> where T1 : class
    {
        Task Create(T1 entity);
        void Delete(T1 entity);
        IEnumerable<T1> Find(Expression<Func<T1, bool>> func_predicate);
        IEnumerable<T1> GetAll(int pageSize = 0, int currentPage = 0);
        Task<T1> Read(T2 id);
        void Update(T1 entity);
    }
}