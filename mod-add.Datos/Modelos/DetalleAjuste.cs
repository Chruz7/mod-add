using mod_add.Datos.Enums;
using System;

namespace mod_add.Datos.Modelos
{
    [Serializable()]
    public class DetalleAjuste
    {
        public long Folio { get; set; }
        public decimal FolioNotaConsumo { get; set; }
        public DateTime Fecha { get; set; }
        public TipoLogico Cancelado { get; set; }
        public TipoLogico Facturado { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalOriginal { get; set; }
        public int TotalArticulos { get; set; }
        public int ProductosEliminados { get; set; }
        public decimal TotalConDescuento { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Tarjeta { get; set; }
        public decimal Vales { get; set; }
        public decimal Otros { get; set; }
        public bool Modificar { get; set; }
    }
}
