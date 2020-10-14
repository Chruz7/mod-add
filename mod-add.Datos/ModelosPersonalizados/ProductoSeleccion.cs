using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Datos.ModelosPersonalizados
{
    public class ProductoSeleccion
    {
        public string Clave { get; set; }
        public string Grupo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Display_Precio { get { return string.Format("{0:C}", Precio); } }
    }
}
