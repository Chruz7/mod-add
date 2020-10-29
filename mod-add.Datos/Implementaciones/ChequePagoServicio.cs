using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;

namespace mod_add.Datos.Implementaciones
{
    public class ChequePagoServicio : ServiceBase<ChequePago>, IChequePagoServicio
    {
        public ChequePagoServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
