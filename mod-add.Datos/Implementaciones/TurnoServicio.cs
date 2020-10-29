using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System.Data.Entity;
using System.Linq;

namespace mod_add.Datos.Implementaciones
{
    public class TurnoServicio : ServiceBase<Turno>, ITurnoServicio
    {
        public TurnoServicio(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public object[] Obteneridturno()
        {
            var result = (from turno in dbset.AsNoTracking() 
                          select (object)turno.idturno.Value).ToArray();

            return result;
        }
    }
}
