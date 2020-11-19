namespace mod_add.Modelos
{
    public class Pago
    {
        public string idformadepago { get; set; }
        public string descripcion { get; set; }
        public decimal tipodecambio { get; set; }
        public decimal importe { get; set; }
        public decimal propina { get; set; }
        public decimal prioridadboton { get; set; }
        public bool Relleno { get; set; }

        public string Descripcion
        {
            get
            {
                if (Relleno)
                    return " ";

                return descripcion.Length <= 14 ? descripcion : descripcion.Substring(0, 14);
            }
        }
        public string SImporte
        {
            get
            {
                if (Relleno)
                    return " ";

                return string.Format("{0:C}", importe);
            }
        }
        public string SPropina
        {
            get
            {
                if (Relleno)
                    return " ";

                return string.Format("{0:C}", propina);
            }
        }
    }
}
