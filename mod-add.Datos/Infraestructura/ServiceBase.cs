using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using mod_add.Datos.Contexto;

namespace mod_add.Datos.Infraestructura
{
    public abstract class ServiceBase<T> : IDisposable, IServiceBase<T> where T : class
    {
        public ApplicationDbContext dataContext;
        public readonly IDbSet<T> dbset;

        protected ServiceBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbset = DataContext.Set<T>();
            dbset.AsNoTracking();
            dataContext.Configuration.LazyLoadingEnabled = true;
        }

        protected ServiceBase(IDatabaseFactory databaseFactory, bool noTracking, bool lazyloading = true)
        {
            DatabaseFactory = databaseFactory;
            dbset = DataContext.Set<T>();
            if (noTracking)
            {
                dbset.AsNoTracking<T>();
            }
            dataContext.Configuration.LazyLoadingEnabled = lazyloading;
        }

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected ApplicationDbContext DataContext
        {
            get { return dataContext ?? (dataContext = DatabaseFactory.Get()); }
        }

        public T Get(int id)
        {
            return dbset.Find(id);
        }

        public T Get(Expression<Func<T, bool>> @where)
        {
            return dbset.AsNoTracking().Where(where).FirstOrDefault();
        }

        public IQueryable<T> GetMany(Expression<Func<T, bool>> @where)
        {
            return dbset.AsNoTracking().Where(where);
        }

        public IQueryable<T> GetAll()
        {
            return dbset.AsNoTracking();
        }


        public int Update(T entity)
        {
            try
            {
                dbset.Attach(entity);
                DataContext.Entry(entity).State = EntityState.Modified;

                return Save();
            }
            catch
            {
                DataContext.Entry(entity).State = EntityState.Detached;
                throw;
            }
        }

        public int Save()
        {
            return DataContext.SaveChanges();
        }

        public int Create(T entity)
        {
            try
            {
                dbset.Add(entity);
                return Save();
            }
            catch
            {
                DataContext.Entry(entity).State = EntityState.Detached;
                throw;
            }
        }

        public int Delete(int id)
        {
            var entity = Get(id);
            dbset.Remove(entity);
            return Save();
        }

        public IQueryable<T> GetMany(Expression<Func<T, bool>> where, string order, bool descending, int take, int skip = 0)
        {
            if (take <= 0)
            {
                if (descending)
                {
                    return dbset.OrderByDescending(order).Where(where);
                }
                else
                {
                    return dbset.OrderBy(order).Where(where);
                }
            }
            else
            {
                if (descending)
                {
                    return dbset.Where(where).OrderByDescending(order).Skip((skip - 1) * take).Take(take);
                }
                else
                {
                    return dbset.Where(where).OrderBy(order).Skip((skip - 1) * take).Take(take);
                }
            }
        }

        public virtual void LoadCollection(T model, string collectionName)
        {
            dataContext.Entry<T>(model).Collection(collectionName).Load();

        }

        public int Count()
        {
            return dbset.Count();
        }

        public int Count(Expression<Func<T, bool>> expression)
        {
            return dbset.Count(expression);
        }

        public void Dispose()
        {
            dataContext.Dispose();
        }
    }

    static class IOrderedQueryable
    {
        #region Order By String Column Name
        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property;

            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields. 
                String[] childProperties = propertyName.Split('.');
                var childProperty = typeof(T).GetProperty(childProperties[0]);
                property = Expression.MakeMemberAccess(param, childProperty);

                for (int i = 1; i < childProperties.Length; i++)
                {
                    Type t = childProperty.PropertyType;
                    if (!t.IsGenericType)
                    {
                        childProperty = t.GetProperty(childProperties[i]);
                    }
                    else
                    {
                        childProperty = t.GetGenericArguments().First().GetProperty(childProperties[i]);
                    }

                    property = Expression.MakeMemberAccess(property, childProperty);
                }
            }
            else
            {
                property = Expression.PropertyOrField(param, propertyName);
            }

            LambdaExpression sort = Expression.Lambda(property, param);

            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, false);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, false);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, false, true);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            return OrderingHelper(source, propertyName, true, true);
        }
        #endregion
    }
}
