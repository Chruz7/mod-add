using mod_add.Datos.Infraestructura;
using mod_add.Datos.Modelos;
using System.Collections.Generic;

namespace mod_add.Datos.Interfaces
{
    public interface IChequeServicio : IServiceBase<Cheque>
    {
        object[] ObtenerFolios();

        long PrimerFolio();
    }
}
