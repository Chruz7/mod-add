using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Modelos
{
    public class ImpuestoVenta
    {
        public int porcentaje { get; set; }
        public decimal impuesto { get; set; }
        public decimal venta { get; set; }

        public string Sporcentaje { get { return $"{porcentaje}%"; } }
        public string  Simpuesto { get { return string.Format("{0:C}", impuesto); } }
        public string  Sventa { get { return string.Format("{0:C}", venta); } }
    }
}
