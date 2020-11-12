using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Modelos
{
    public class ChequePagoReporte
    {
        public long folio { get; set; }
        public string idformadepago { get; set; }
        public decimal? importe { get; set; }
        public decimal? propina { get; set; }
        public decimal? tipodecambio { get; set; }
        public string referencia { get; set; }
        //public long? idturno_cierre { get; set; }
        //public bool? procesado { get; set; }
        //public int? sistema_envio { get; set; }
        public string DescripcionFormaPago { get; set; }
        public string Simporte { get { return string.Format("{0:C}", importe ?? 0); } }
        public string Spropina { get { return string.Format("{0:C}", propina ?? 0); } }
        public string Stipodecambio { get { return (tipodecambio ?? 0) > 1 ? string.Format("{0:C}", tipodecambio) : ""; } }
    }
}
