using mod_add.Datos.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace mod_add.Datos.Modelos
{
    [Table("Cheques_Pago")]
    public class ChequePago
    {
        public int Id { get; set; }
        public TipoAccion TipoAccion { get; set; }
        public long FolioAnterior { get; set; }

        public long folio { get; set; }
        public string idformadepago { get; set; }
        public decimal? importe { get; set; }
        public decimal? propina { get; set; }
        public decimal? tipodecambio { get; set; }
        public string referencia { get; set; }
        public long? idturno_cierre { get; set; }
        public bool? procesado { get; set; }
        public int? sistema_envio { get; set; }
    }
}
