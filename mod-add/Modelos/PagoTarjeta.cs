namespace mod_add.Modelos
{
    public class PagoTarjeta
    {
        public int numcheque { get; set; }
        public string descripcion { get; set; }
        public decimal importe { get; set; }
        public decimal propina { get; set; }

        public decimal Cargo
        {
            get
            {
                return importe + propina;
            }
        }
    }
}
