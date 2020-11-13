using System;
using System.Collections.Generic;

namespace mod_add.Modelos
{
    public class ChequeReporte
    {
        public ChequeReporte()
        {
            ChequesPagos = new List<ChequePagoReporte>();
            Totales = false;
        }

        public long folio { get; set; }
        //public string seriefolio { get; set; }
        public decimal? numcheque { get; set; }
        public DateTime? fecha { get; set; }
        //public DateTime? salidarepartidor { get; set; }
        //public DateTime? arriborepartidor { get; set; }
        //public DateTime? cierre { get; set; }
        public string mesa { get; set; }
        public decimal? nopersonas { get; set; }
        //public string idmesero { get; set; }
        //public bool? pagado { get; set; }
        public bool? cancelado { get; set; }
        //public bool? impreso { get; set; }
        public decimal? impresiones { get; set; }
        //public decimal? cambio { get; set; }
        public decimal? descuento { get; set; }
        public decimal? reabiertas { get; set; }
        //public string razoncancelado { get; set; }
        //public decimal? orden { get; set; }
        //public bool? facturado { get; set; }
        //public string idcliente { get; set; }
        //public string idarearestaurant { get; set; }
        //public string idempresa { get; set; }
        public decimal? tipodeservicio { get; set; }
        public decimal? idturno { get; set; }
        //public string usuariocancelo { get; set; }
        //public string comentariodescuento { get; set; }
        //public string estacion { get; set; }
        //public decimal? cambiorepartidor { get; set; }
        //public string usuariodescuento { get; set; }
        //public DateTime? fechacancelado { get; set; }
        public string idtipodescuento { get; set; }
        //public string numerotarjeta { get; set; }
        public decimal? folionotadeconsumo { get; set; }
        //public bool? notadeconsumo { get; set; }
        //public bool? propinapagada { get; set; }
        //public decimal? propinafoliomovtocaja { get; set; }
        //public decimal? puntosmonederogenerados { get; set; }
        //public decimal? propinaincluida { get; set; }
        //public string tarjetadescuento { get; set; }
        //public decimal? porcentajefac { get; set; }
        //public string usuariopago { get; set; }
        //public bool? propinamanual { get; set; }
        //public string observaciones { get; set; }
        //public string idclientedomicilio { get; set; }
        //public string iddireccion { get; set; }
        //public string idclientefacturacion { get; set; }
        //public string telefonousadodomicilio { get; set; }
        //public decimal? totalarticulos { get; set; }
        //public decimal? subtotal { get; set; }
        //public decimal? subtotalsinimpuestos { get; set; }
        public decimal? total { get; set; }
        //public decimal? totalconpropina { get; set; }
        //public decimal? totalsinimpuestos { get; set; }
        //public decimal? totalsindescuentosinimpuesto { get; set; }
        //public decimal? totalimpuesto1 { get; set; }
        //public decimal? totalalimentosconimpuestos { get; set; }
        //public decimal? totalbebidasconimpuestos { get; set; }
        //public decimal? totalotrosconimpuestos { get; set; }
        //public decimal? totalalimentossinimpuestos { get; set; }
        //public decimal? totalbebidassinimpuestos { get; set; }
        //public decimal? totalotrossinimpuestos { get; set; }
        //public decimal? totaldescuentossinimpuestos { get; set; }
        //public decimal? totaldescuentosconimpuestos { get; set; }
        //public decimal? totaldescuentoalimentosconimpuesto { get; set; }
        //public decimal? totaldescuentobebidasconimpuesto { get; set; }
        //public decimal? totaldescuentootrosconimpuesto { get; set; }
        //public decimal? totaldescuentoalimentossinimpuesto { get; set; }
        //public decimal? totaldescuentobebidassinimpuesto { get; set; }
        //public decimal? totaldescuentootrossinimpuesto { get; set; }
        //public decimal? totalcortesiassinimpuestos { get; set; }
        //public decimal? totalcortesiasconimpuestos { get; set; }
        //public decimal? totalcortesiaalimentosconimpuesto { get; set; }
        //public decimal? totalcortesiabebidasconimpuesto { get; set; }
        //public decimal? totalcortesiaotrosconimpuesto { get; set; }
        //public decimal? totalcortesiaalimentossinimpuesto { get; set; }
        //public decimal? totalcortesiabebidassinimpuesto { get; set; }
        //public decimal? totalcortesiaotrossinimpuesto { get; set; }
        //public decimal? totaldescuentoycortesiasinimpuesto { get; set; }
        //public decimal? totaldescuentoycortesiaconimpuesto { get; set; }
        public decimal? cargo { get; set; }
        //public decimal? totalconcargo { get; set; }
        //public decimal? totalconpropinacargo { get; set; }
        public decimal? descuentoimporte { get; set; }
        public decimal? efectivo { get; set; }
        public decimal? tarjeta { get; set; }
        public decimal? vales { get; set; }
        public decimal? otros { get; set; }
        public decimal? propina { get; set; }
        //public decimal? propinatarjeta { get; set; }
        //public decimal? totalalimentossinimpuestossindescuentos { get; set; }
        //public decimal? totalbebidassinimpuestossindescuentos { get; set; }
        //public decimal? totalotrossinimpuestossindescuentos { get; set; }
        //public string campoadicional1 { get; set; }
        //public string idreservacion { get; set; }
        //public string idcomisionista { get; set; }
        //public decimal? importecomision { get; set; }
        //public bool? comisionpagada { get; set; }
        //public DateTime? fechapagocomision { get; set; }
        //public decimal? foliopagocomision { get; set; }
        public decimal? tipoventarapida { get; set; }
        //public bool? callcenter { get; set; }
        //public long? idordencompra { get; set; }
        public decimal? totalsindescuento { get; set; }
        //public decimal? totalalimentos { get; set; }
        //public decimal? totalbebidas { get; set; }
        //public decimal? totalotros { get; set; }
        public decimal? totaldescuentos { get; set; }
        public decimal? totaldescuentoalimentos { get; set; }
        public decimal? totaldescuentobebidas { get; set; }
        public decimal? totaldescuentootros { get; set; }
        public decimal? totalcortesias { get; set; }
        public decimal? totalcortesiaalimentos { get; set; }
        public decimal? totalcortesiabebidas { get; set; }
        public decimal? totalcortesiaotros { get; set; }
        public decimal? totaldescuentoycortesia { get; set; }
        public decimal? totalalimentossindescuentos { get; set; }
        public decimal? totalbebidassindescuentos { get; set; }
        public decimal? totalotrossindescuentos { get; set; }
        //public decimal? descuentocriterio { get; set; }
        public decimal? descuentomonedero { get; set; }
        //public string idmenucomedor { get; set; }
        public decimal? subtotalcondescuento { get; set; }
        //public decimal? comisionpax { get; set; }
        //public bool? procesadointerfaz { get; set; }
        //public bool? domicilioprogramado { get; set; }
        //public DateTime? fechadomicilioprogramado { get; set; }
        //public bool? enviado { get; set; }
        //public string ncf { get; set; }
        //public string numerocuenta { get; set; }
        //public string codigo_unico_af { get; set; }
        //public int? estatushub { get; set; }
        //public decimal? idfoliohub { get; set; }
        //public bool? EnviadoRW { get; set; }
        //public string usuarioapertura { get; set; }
        //public string titulartarjetamonedero { get; set; }
        //public decimal? saldoanteriormonedero { get; set; }
        //public string autorizacionfolio { get; set; }
        //public DateTime? fechalimiteemision { get; set; }
        //public decimal totalimpuestod1 { get; set; }
        //public decimal totalimpuestod2 { get; set; }
        //public decimal totalimpuestod3 { get; set; }
        //public string idmotivocancela { get; set; }
        //public int? sistema_envio { get; set; }
        //public string idformadepagoDescuento { get; set; }
        //public string titulartarjetamonederodescuento { get; set; }
        //public long? foliotempcheques { get; set; }
        //public int? c_iddispositivo { get; set; }
        //public string surveycode { get; set; }
        //public string salerestaurantid { get; set; }
        //public DateTime? timemarktoconfirmed { get; set; }
        //public DateTime? timemarktodelivery { get; set; }
        //public DateTime? timemarktodeliveryarrive { get; set; }
        //public int? esalestatus { get; set; }
        //public int? statusSR { get; set; }
        //public string paymentreference { get; set; }
        //public decimal? deliverycharge { get; set; }
        //public bool? comandaimpresa { get; set; }
        //public int? foodorder { get; set; }
        //public decimal? cashpaymentwith { get; set; }
        //public int? intentoEnvioAF { get; set; }
        //public int? paymentmethod_id { get; set; }
        //public string TKC_Token { get; set; }
        //public string TKC_Transaction { get; set; }
        //public string TKC_Authorization { get; set; }
        //public string TKC_Cupon { get; set; }
        //public string TKC_ExpirationDate { get; set; }
        //public decimal TKC_Recompensa { get; set; }
        //public string campoadicional3 { get; set; }
        //public string estrateca_CardNumber { get; set; }
        //public string estrateca_VoucherText { get; set; }
        //public string campoadicional4 { get; set; }
        //public string campoadicional5 { get; set; }
        //public string sacoa_CardNumber { get; set; }
        //public decimal sacoa_credits { get; set; }
        //public string estrateca_TypeDisccount { get; set; }
        //public string estrateca_DiscountCode { get; set; }
        //public string estrateca_DiscountID { get; set; }
        //public decimal estrateca_DiscountAmount { get; set; }
        //public decimal? desc_imp_original { get; set; }
        //public decimal donativo { get; set; }
        //public decimal totalcondonativo { get; set; }
        //public decimal totalconpropinacargodonativo { get; set; }
        //public string orderreference { get; set; }
        //public string appname { get; set; }
        //public string paymentproviderid { get; set; }
        //public string paymentprovider { get; set; }
        //public bool? ChangeStatusSRX { get; set; }
        //public string claveempresav { get; set; }
        //public bool? cuentaenuso { get; set; }
        //public decimal? modificado { get; set; }
        //public string campoadicional2 { get; set; }
        public string DescripcionTipoDescuento { get; set; }

        public bool Totales { get; set; }

        public List<ChequePagoReporte> ChequesPagos { get; set; }

        public string Snumcheque { get { return $"{numcheque}"; } }
        public string Snumcheque2 { get { return $"{numcheque}".PadLeft(8, '0'); } }
        public string Sfolionotadeconsumo { get { return (folionotadeconsumo ?? 0) > 0 ? $"{folionotadeconsumo}" : ""; } }
        public string Sfecha { get { return fecha.HasValue ? fecha.ToString() : ""; } }
        public string Simpresiones { get { return (impresiones ?? 0) > 1 ? $"{(int)impresiones}" : ""; } }
        public string Sreabiertas { get { return (reabiertas ?? 0) > 0 ? $"{(int)reabiertas}" : ""; } }
        public string Sdescuento { get { return  (descuento ?? 0) > 0 ? string.Format("{0:C}", descuento) : ""; } }
        public string Stotaldescuentoycortesia { get { return (totaldescuentoycortesia ?? 0) > 0 || Totales ? string.Format("{0:C}", totaldescuentoycortesia ?? 0) : ""; } }
        public string Spropina { get { return (propina ?? 0) > 0 || Totales ? string.Format("{0:C}", propina ?? 0) : ""; } }
        public string Simporte { get { return (descuento ?? 0) < 100 || Totales ? string.Format("{0:C}", total ?? 0) : "CORTESIA"; } }
        public string Scargo { get { return (cargo ?? 0) > 0 || Totales ? string.Format("{0:C}", cargo ?? 0) : ""; } }
        public string Sefectivo { get { return (efectivo ?? 0) > 0 || Totales ? string.Format("{0:C}", efectivo ?? 0) : ""; } }
        public string Starjeta { get { return (tarjeta ?? 0) > 0 || Totales ? string.Format("{0:C}", tarjeta ?? 0) : ""; } }
        public string Svales { get { return (vales ?? 0) > 0 || Totales ? string.Format("{0:C}", vales ?? 0) : ""; } }
        public string Sotros { get { return (otros ?? 0) > 0 || Totales ? string.Format("{0:C}", otros ?? 0) : ""; } }
    }
}
