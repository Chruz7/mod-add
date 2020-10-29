using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System.Data.Entity;
using System.Linq;

namespace mod_add.Datos.Implementaciones
{
    public class ChequeServicio : ServiceBase<Cheque>, IChequeServicio
    {
        public ChequeServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public object[] ObtenerFolios()
        {
            var result = (from cheque in dbset.AsNoTracking() 
                          select (object)cheque.folio).ToArray();

            return result;
        }
    }
}
