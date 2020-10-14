using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System.Data.Entity;
using System.Linq;

namespace mod_add.Datos.Implementaciones
{
    public class ConfiguracionServicio : ServiceBase<ConfiguracionSistema>, IConfiguracionServicio
    {
        public ConfiguracionServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public ConfiguracionSistema ObtenerConfiguracion()
        {
            return (from configuracion in dbset.AsNoTracking() select configuracion).FirstOrDefault();
        }
    }
}
