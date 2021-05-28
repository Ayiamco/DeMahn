using LaundryManagerAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Queries
{
    public abstract class GenericRepository<T> where T:class
    {
        protected ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public  async Task  Create(T entity)
        {
           await  _context.Set<T>().AddAsync(entity);
           return;
        }
        public async Task<T> Read(int id)
        {
           return await  _context.Set<T>().FindAsync(id);
        }

        public void Delete (T entity)
        {
            _context.Set<T>().Remove(entity);
        }
 

        public IEnumerable<T> GetAll(int pageSize=0,int currentPage=0)
        {
            return _context.Set<T>().Skip(currentPage * pageSize).Take(pageSize).AsQueryable().ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> func_predicate)
        {
            return _context.Set<T>().Where(func_predicate);
        }
    }
}
