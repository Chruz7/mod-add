using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Utils
{
    public static class Mat
    {
        public static decimal Redondeo(decimal valor)
        {
            return Math.Round(valor, 4, App.MidpointRounding);
        }
    }
}
