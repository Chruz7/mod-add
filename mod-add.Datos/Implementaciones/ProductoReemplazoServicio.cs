using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Datos.Implementaciones
{
    public class ProductoReemplazoServicio : ServiceBase<ProductoReemplazo>, IProductoReemplazoServicio
    {
        public ProductoReemplazoServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }
    }
}
