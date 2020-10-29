using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;

namespace mod_add.Datos.Implementaciones
{
    public class ChequeDetalleServicio : ServiceBase<ChequeDetalle>, IChequeDetalleServicio
    {
        public ChequeDetalleServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
