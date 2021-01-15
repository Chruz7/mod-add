using mod_add.Datos.Enums;
using System.Text;

namespace mod_add.Datos.Modelos
{
    public class ChequePago
    {
        public TipoAccion TipoAccion { get; set; }
        public long FolioAnt { get; set; }
        public string IdFormadePagoAnt { get; set; }
        public decimal? ImporteAnt { get; set; }
        public decimal? PropinaAnt { get; set; }
        public decimal? TipodecambioAnt { get; set; }
        public string ReferenciaAnt { get; set; }

        //public long folio { get; set; }
        //public string idformadepago { get; set; }
        //public decimal? importe { get; set; }
        //public decimal? propina { get; set; }
        //public decimal? tipodecambio { get; set; }
        //public string referencia { get; set; }
        //public long? idturno_cierre { get; set; }
        //public bool? procesado { get; set; }
        //public int? sistema_envio { get; set; }
        public long folio { get; set; }
        public string idformadepago { get; set; }
        public decimal? importe { get; set; }
        public decimal? propina { get; set; }
        public decimal? tipodecambio { get; set; }
        public string referencia { get; set; }
        public long? idturno_cierre { get; set; }
        public bool? procesado { get; set; }
        public int sistema_envio { get; set; }
    }
}
