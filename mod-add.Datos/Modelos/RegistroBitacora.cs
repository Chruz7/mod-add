using System;

namespace mod_add.Datos.Modelos
{
    public class RegistroBitacora
    {
        public int Id { get; set; }
        public DateTime FechaProceso { get; set; }
        public DateTime FechaInicialVenta { get; set; }
        public DateTime FechaFinalVenta { get; set; }
        public int TotalCuentas { get; set; }
        public int CuentasModificadas { get; set; }
        public decimal ImporteAnterior { get; set; }
        public decimal ImporteNuevo { get; set; }
        public decimal Diferencia { get; set; }
    }
}
