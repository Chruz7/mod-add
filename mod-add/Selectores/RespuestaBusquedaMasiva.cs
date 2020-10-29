using mod_add.Enums;
using SR.Datos;
using System.Collections.Generic;

namespace mod_add.Selectores
{
    public class RespuestaBusquedaMasiva
    {
        public TipoRespuesta TipoRespuesta { get; set; }
        public List<SR_turnos> Turnos { get; set; }
        public List<SR_cheques> Cheques { get; set; }
        public List<SR_cheqdet> Cheqdet { get; set; }
        public List<SR_chequespagos> Chequespagos { get; set; }
        public List<SR_formasdepago> Formasdepago { get; set; }
    }
}
