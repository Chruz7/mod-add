using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;

namespace mod_add.Datos.Implementaciones
{
    public class BitacoraServicio : ServiceBase<RegistroBitacora>, IBitacoraServicio
    {
        public BitacoraServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
