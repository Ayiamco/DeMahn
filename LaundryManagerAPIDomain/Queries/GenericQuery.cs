using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Queries
{
    public abstract class GenericQuery<T1,T2> where T1:class
    {
        protected ApplicationDbContext _context;

        public GenericQuery(ApplicationDbContext context)
        {
            _context = context;
        }
        public  async Task  Create(T1 entity)
        {
           await  _context.Set<T1>().AddAsync(entity);
           return;
        }
        public async Task<T1> Read(T2 id)
        {
           return await  _context.Set<T1>().FindAsync(id);
        }

        public void Delete (T1 entity)
        {
            _context.Set<T1>().Remove(entity);
        }
 

        public IEnumerable<T1> GetAll(int pageSize=0,int currentPage=0)
        {
            return _context.Set<T1>().Skip(currentPage * pageSize).Take(pageSize).AsQueryable().ToList();
        }

        public IEnumerable<T1> Find(Expression<Func<T1, bool>> func_predicate)
        {
            return _context.Set<T1>().Where(func_predicate);
        }
    }
}
