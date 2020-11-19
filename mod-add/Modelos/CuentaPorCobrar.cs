using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Modelos
{
    public class CuentaPorCobrar
    {
        public string Descripcion { get; set; }
        public decimal Importe { get; set; }
        public bool Relleno { get; set; }
        public string SDescripcion
        {
            get
            {
                if (Relleno)
                    return " ";

                return Descripcion;
            }
        }
        public string SImporte
        {
            get
            {
                if (Relleno)
                    return " ";

                return string.Format("{0:C}", Importe);
            }
        }
    }
}
