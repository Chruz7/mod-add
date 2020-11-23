namespace mod_add.Modelos
{
    public class PagoTarjeta
    {
        public int numcheque { get; set; }
        public string descripcion { get; set; }
        public decimal importe { get; set; }
        public decimal propina { get; set; }

        public string Snumcheque { get { return $"{numcheque}"; } }
        public string Simporte { get { return string.Format("{0:C}", importe); } }
        public string Spropina { get { return string.Format("{0:C}", propina); } }
        public decimal Cargo
        {
            get
            {
                return importe + propina;
            }
        }
        public string SCargo { get { return string.Format("{0:C}", Cargo); } }
    }
}
