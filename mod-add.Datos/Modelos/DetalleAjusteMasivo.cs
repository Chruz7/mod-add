using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Datos.Modelos
{
    public class DetalleAjusteMasivo
    {
        public long Folio { get; set; }
        public decimal FolioNotaConsumo { get; set; }
        public DateTime Fecha { get; set; }
        public bool Cancelado { get; set; }
        public bool Facturado { get; set; }
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
