using mod_add.Datos.Contexto;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System.Linq;

namespace mod_add.Datos.Implementaciones
{
    public class RegistroLicenciaServicio : ServiceBase<RegistroLicencia>, IRegistroLicenciaServicio
    {
        public RegistroLicenciaServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public bool Exite(string licencia)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                return context.RegistroLicencias.Any(x => x.Licencia == licencia);
            }
        }
    }
}
