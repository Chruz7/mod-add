using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;

namespace mod_add.Datos.Implementaciones
{
    public class ProductoEliminarServicio : ServiceBase<ProductoEliminacion>, IProductoEliminarServicio
    {
        public ProductoEliminarServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
