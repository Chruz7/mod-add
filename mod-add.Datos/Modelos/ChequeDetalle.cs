using mod_add.Datos.Enums;
using System;

namespace mod_add.Datos.Modelos
{
    public class ChequeDetalle
    {
        public TipoAccion TipoAccion { get; set; }
        public TipoClasificacion TipoClasificacion { get; set; }
        public bool Cambiado { get; set; }
        public decimal PrecionEnUnidad { get; set; }
        public long? FolioAnt { get; set; }
        public decimal? CantidadAnt { get; set; }
        public string IdProductoAnt { get; set; }
        public decimal? PrecioAnt { get; set; }
        public decimal? Impuesto1Ant { get; set; }
        public decimal? Impuesto2Ant { get; set; }
        public decimal? Impuesto3Ant { get; set; }
        public decimal? PrecioSinImpuestosAnt { get; set; }
        public decimal ImpuestoImporte3Ant { get; set; }
        public decimal? PrecioCatalogoAnt { get; set; }
        public string IdProductoCompuestoAnt { get; set; }

        public long? foliodet { get; set; }
        public decimal? movimiento { get; set; }
        public string comanda { get; set; }
        public decimal? cantidad { get; set; }
        public string idproducto { get; set; }
        public decimal? descuento { get; set; }
        public decimal? precio { get; set; }
        public decimal? impuesto1 { get; set; }
        public decimal? impuesto2 { get; set; }
        public decimal? impuesto3 { get; set; }
        public decimal? preciosinimpuestos { get; set; }
        public string tiempo { get; set; }
        public DateTime? hora { get; set; }
        public bool? modificador { get; set; }
        public decimal? mitad { get; set; }
        public string comentario { get; set; }
        public string idestacion { get; set; }
        public string usuariodescuento { get; set; }
        public string comentariodescuento { get; set; }
        public string idtipodescuento { get; set; }
        public DateTime? horaproduccion { get; set; }
        public string idproductocompuesto { get; set; }
        public bool? productocompuestoprincipal { get; set; }
        public decimal? preciocatalogo { get; set; }
        public bool? marcar { get; set; }
        public string idmeseroproducto { get; set; }
        public string prioridadproduccion { get; set; }
        public decimal? estatuspatin { get; set; }
        public string idcortesia { get; set; }
        public string numerotarjeta { get; set; }
        public decimal? estadomonitor { get; set; }
        public string llavemovto { get; set; }
        public DateTime? horameserofinalizado { get; set; }
        public string meserofinalizado { get; set; }
        public int sistema_envio { get; set; }
        public long? idturno_cierre { get; set; }
        public bool? procesado { get; set; }
        public bool promovolumen { get; set; }
        public int iddispositivo { get; set; }
        public int productsyncidsr { get; set; }
        public decimal subtotalsrx { get; set; }
        public decimal totalsrx { get; set; }
        public long idmovtobillar { get; set; }
        public int? idlistaprecio { get; set; }
        public decimal? tipocambio { get; set; }
        public decimal impuestoimporte3 { get; set; }
        public string estrateca_DiscountCode { get; set; }
        public string estrateca_DiscountID { get; set; }
        public decimal estrateca_DiscountAmount { get; set; }
        //public long? foliodet { get; set; }
        //public decimal? movimiento { get; set; }
        //public string comanda { get; set; }
        //public decimal? cantidad { get; set; }
        //public string idproducto { get; set; }
        //public decimal? descuento { get; set; }
        //public decimal? precio { get; set; }
        //public decimal? impuesto1 { get; set; }
        //public decimal? impuesto2 { get; set; }
        //public decimal? impuesto3 { get; set; }
        //public decimal? preciosinimpuestos { get; set; }
        //public string tiempo { get; set; }
        //public DateTime? hora { get; set; }
        //public bool? modificador { get; set; }
        //public decimal? mitad { get; set; }
        //public string comentario { get; set; }
        //public string idestacion { get; set; }
        //public string usuariodescuento { get; set; }
        //public string comentariodescuento { get; set; }
        //public string idtipodescuento { get; set; }
        //public DateTime? horaproduccion { get; set; }
        //public string idproductocompuesto { get; set; }
        //public bool? productocompuestoprincipal { get; set; }
        //public decimal? preciocatalogo { get; set; }
        //public bool? marcar { get; set; }
        //public string idmeseroproducto { get; set; }
        //public string prioridadproduccion { get; set; }
        //public decimal? estatuspatin { get; set; }
        //public string idcortesia { get; set; }
        //public string numerotarjeta { get; set; }
        //public decimal? estadomonitor { get; set; }
        //public string llavemovto { get; set; }
        //public DateTime? horameserofinalizado { get; set; }
        //public string meserofinalizado { get; set; }
        //public int sistema_envio { get; set; }
        //public long? idturno_cierre { get; set; }
        //public bool? procesado { get; set; }
        //public bool? promovolumen { get; set; }
        //public int? iddispositivo { get; set; }
        //public int? productsyncidsr { get; set; }
        //public decimal? subtotalsrx { get; set; }
        //public decimal? totalsrx { get; set; }
        //public int? idlistaprecio { get; set; }
        //public decimal? tipocambio { get; set; }
        //public long idmovtobillar { get; set; }
        //public decimal? impuestoimporte3 { get; set; }
        //public string estrateca_DiscountCode { get; set; }
        //public string estrateca_DiscountID { get; set; }
        //public decimal estrateca_DiscountAmount { get; set; }
        //public int CantidadEntera
        //{
        //    get { return (int)Math.Truncate(cantidad.Value); }
        //}
        public decimal ImporteCISD
        {
            get { return Math.Round(precio.Value * cantidad.Value, 4, MidpointRounding.AwayFromZero); }
        }
        public decimal ImporteCICD
        {
            get { return Math.Round(precio.Value * cantidad.Value * (100m - descuento.Value) / 100, 4, MidpointRounding.AwayFromZero); }
        }
        public decimal ImporteSISD
        {
            get { return preciosinimpuestos.Value * cantidad.Value; }
        }
        public decimal ImporteSICD
        {
            get { return preciosinimpuestos.Value * cantidad.Value * (100m - descuento.Value) / 100; }
        }
        public decimal ImporteI1CD
        {
            get { return preciosinimpuestos.Value * impuesto1.Value / 100m * cantidad.Value * (100m - descuento.Value) / 100m; }
        }
        public decimal ImporteI2CD
        {
            get { return preciosinimpuestos.Value * impuesto2.Value / 100m * cantidad.Value * (100m - descuento.Value) / 100m; }
        }
        public decimal ImporteI3CD
        {
            get { return preciosinimpuestos.Value * impuesto3.Value / 100m * cantidad.Value * (100m - descuento.Value) / 100m; }
        }
    }
}
