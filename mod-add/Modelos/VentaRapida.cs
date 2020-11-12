using DocumentFormat.OpenXml.Drawing.Diagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Modelos
{
    public class VentaRapida
    {
        public string Descripcion { get; set; }
        public decimal Total { get; set; }

        public string STotal { get { return string.Format("0:C", Total); } }
    }
}
