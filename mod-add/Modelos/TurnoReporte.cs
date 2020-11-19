using System;
using System.Globalization;

namespace mod_add.Modelos
{
    public class TurnoReporte
    {
        //public long idturnointerno { get; set; }
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
        public decimal Total { get; set; }
        public decimal Cargo { get; set; }
        public decimal Propina { get; set; }
        //public bool? procesadoweb { get; set; }
        //public string idempresa { get; set; }
        //public bool? enviadoacentral { get; set; }
        //public DateTime? fechaenviado { get; set; }
        //public string usuarioenvio { get; set; }
        //public bool? offline { get; set; }
        //public bool? enviadoaf { get; set; }
        //public bool? corte_enviado { get; set; }
        //public bool? eliminartemporalesencierre { get; set; }
        //public string idmesero { get; set; }

        public string Sapertura { get { return apertura.HasValue ? apertura.Value.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("US")) : ""; } }
        public string Scierre 
        { 
            get 
            { 
                if (apertura.HasValue && cierre.HasValue && apertura.Value.Day == cierre.Value.Day)
                    return cierre.HasValue ? cierre.Value.ToString("hh:mm:ss tt", CultureInfo.CreateSpecificCulture("US")) : "";

                return cierre.HasValue ? cierre.Value.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("US")) : ""; 
            } 
        }
        public string Sidturno { get { return $"{idturno}"; } }
        public string Sfondo { get { return string.Format("{0:C}", fondo ?? 0); } }
        public string Sefectivo { get { return string.Format("{0:C}", efectivo ?? 0); } }
        public string Starjeta { get { return string.Format("{0:C}", tarjeta ?? 0); } }
        public string Svales { get { return string.Format("{0:C}", vales ?? 0); } }
        public string Scredito { get { return string.Format("{0:C}", credito ?? 0); } }
        public string STotal { get { return string.Format("{0:C}", Total); } }
        public string SCargo { get { return string.Format("{0:C}", Cargo); } }
        public string SPropina { get { return string.Format("{0:C}", Propina); } }
    }
}
