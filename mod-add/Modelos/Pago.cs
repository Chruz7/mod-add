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
        public decimal Total { get { return importe + propina; } }
        public bool Relleno { get; set; }

        public string Descripcion { get { return Relleno ? " " : (descripcion.Length <= 14 ? descripcion : descripcion.Substring(0, 14)); } }
        public string SImporte { get { return Relleno ? " " : string.Format("{0:C}", importe); } }
        public string SPropina { get { return Relleno ? " " : string.Format("{0:C}", propina); } }
        public string STotal { get { return Relleno ? " " : string.Format("{0:C}", Total); } }
    }
}
