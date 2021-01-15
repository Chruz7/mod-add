using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Utils
{
    public static class Valores
    {
        public static CultureInfo Ingles { get; private set; } = new CultureInfo("en-US");
        public static CultureInfo Espanol { get; private set; } = new CultureInfo("es-MX");
    }
}
