using mod_add.Datos.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace mod_add.Datos.Modelos
{
    [Table("Bitatora_Modificaciones")]
    public class BitacoraModificacion
    {
        public int Id { get; set; }
        public TipoAjuste TipoAjuste { get; set; }
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
