using mod_add.Datos.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace mod_add.Datos.Modelos
{
    [Table("Turnos")]
    public class Turno
    {
        public int Id { get; set; }
        public TipoAccion TipoAccion { get; set; }
        public long IdTurnoInternoAnt { get; set; }
        public long IdTurnoAnt { get; set; }
        public decimal EfectivoAnterior { get; set; }
        public decimal TarjetaAnterior { get; set; }
        public decimal ValesAnterior { get; set; }
        public decimal CreditoAnterior { get; set; }

        public long idturnointerno { get; set; }
        public long? idturno { get; set; }
        public decimal? fondo { get; set; }
        public DateTime? apertura { get; set; }
        public DateTime? cierre { get; set; }
        public string idestacion { get; set; }
        public string cajero { get; set; }
        public decimal? efectivo { get; set; }
        public decimal? tarjeta { get; set; }
        public decimal? vales { get; set; }
        public decimal? credito { get; set; }
        public bool? procesadoweb { get; set; }
        public string idempresa { get; set; }
        public bool? enviadoacentral { get; set; }
        public DateTime? fechaenviado { get; set; }
        public string usuarioenvio { get; set; }
        public bool? offline { get; set; }
        public bool? enviadoaf { get; set; }
        public bool? corte_enviado { get; set; }
        public bool? eliminartemporalesencierre { get; set; }
        public string idmesero { get; set; }
    }
}
