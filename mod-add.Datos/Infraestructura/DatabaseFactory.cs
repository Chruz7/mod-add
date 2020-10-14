using mod_add.Datos.Contexto;

namespace mod_add.Datos.Infraestructura
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private ApplicationDbContext dataContext;

        public ApplicationDbContext Get()
        {
            return dataContext = new ApplicationDbContext();
        }
        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }
    }
}
