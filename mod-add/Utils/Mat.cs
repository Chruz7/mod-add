using System;

namespace mod_add.Utils
{
    public static class Mat
    {
        public static decimal Redondear(decimal valor)
        {
            return Math.Round(valor, 4, App.MidpointRounding);
        }

        public static decimal Redondear(decimal valor, int decimales)
        {
            return Math.Round(valor, decimales, App.MidpointRounding);
        }
    }
}
