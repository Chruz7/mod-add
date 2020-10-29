using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Datos.Infraestructura
{
    public interface IServiceBase<T> where T : class
    {
        T Get(int id);
        T Get(Expression<Func<T, bool>> where);

        IQueryable<T> GetAll();
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);
        IQueryable<T> GetMany(Expression<Func<T, bool>> where, string order, bool descending, int take, int skip);

        int Update(T entity);
        int Save();
        int Create(T entity);
        int Delete(int id);
        int Count();
        int Count(Expression<Func<T, bool>> where);
    }
}
