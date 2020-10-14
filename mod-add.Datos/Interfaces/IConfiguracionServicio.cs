using mod_add.Datos.Infraestructura;
using mod_add.Datos.Modelos;

namespace mod_add.Datos.Interfaces
{
    public interface IConfiguracionServicio : IServiceBase<ConfiguracionSistema>
    {
        ConfiguracionSistema ObtenerConfiguracion();
    }
}
