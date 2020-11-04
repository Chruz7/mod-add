using mod_add.Datos.Enums;
using System;

namespace mod_add.Datos.Modelos
{
    public class DetalleAjuste
    {
        public int Folio { get; set; }
        public int FolioNotaConsumo { get; set; }
        public DateTime Fecha { get; set; }
        public TipoLogico Cancelado { get; set; }
        public TipoLogico Facturado { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalOriginal { get; set; }
        public decimal TotalArticulos { get; set; }
        public decimal ProductosEliminados { get; set; }
        public decimal TotalConDescuento { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Tarjeta { get; set; }
        public decimal Vales { get; set; }
        public decimal Otros { get; set; }
        public bool RealizarAccion { get; set; }

        public bool IsEnable { get; set; }
    }
}
