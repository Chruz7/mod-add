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

        public string SImporte { get { return string.Format("{0:C}", importe); } }
        public string SPropina { get { return string.Format("{0:C}", propina); } }
    }
}
