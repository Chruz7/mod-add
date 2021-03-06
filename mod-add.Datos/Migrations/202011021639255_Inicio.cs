﻿namespace mod_add.Datos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cheques",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TipoAccion = c.Int(nullable: false),
                        TipoPago = c.Int(nullable: false),
                        TotalArticulosEliminados = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FolioAnt = c.Long(nullable: false),
                        TotalArticulosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubtotalAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PropinaAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalConPropinaAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalConCargoAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalConPropinaCargoAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DescuentoImporteAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EfectivoAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TarjetaAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PropinaTarjetaAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ValesAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OtrosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalSinDescuentoAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAlimentosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBebidasAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOtrosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDescuentosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDescuentoAlimentosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDescuentoBebidasAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDescuentoOtrosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDescuentoYCortesiaAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAlimentosSinDescuentosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBebidasSinDescuentosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOtrosSinDescuentosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubtotalConDescuentoAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalImpuestoD1Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalImpuestoD2Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalImpuestoD3Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalImpuesto1Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Desc_Imp_OriginalAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CambioAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CambioRepartidorAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdTurnoAnt = c.Long(nullable: false),
                        folio = c.Long(nullable: false),
                        seriefolio = c.String(),
                        numcheque = c.Decimal(precision: 18, scale: 2),
                        fecha = c.DateTime(),
                        salidarepartidor = c.DateTime(),
                        arriborepartidor = c.DateTime(),
                        cierre = c.DateTime(),
                        mesa = c.String(),
                        nopersonas = c.Decimal(precision: 18, scale: 2),
                        idmesero = c.String(),
                        pagado = c.Boolean(),
                        cancelado = c.Boolean(),
                        impreso = c.Boolean(),
                        impresiones = c.Decimal(precision: 18, scale: 2),
                        cambio = c.Decimal(precision: 18, scale: 2),
                        descuento = c.Decimal(precision: 18, scale: 2),
                        reabiertas = c.Decimal(precision: 18, scale: 2),
                        razoncancelado = c.String(),
                        orden = c.Decimal(precision: 18, scale: 2),
                        facturado = c.Boolean(),
                        idcliente = c.String(),
                        idarearestaurant = c.String(),
                        idempresa = c.String(),
                        tipodeservicio = c.Decimal(precision: 18, scale: 2),
                        idturno = c.Decimal(precision: 18, scale: 2),
                        usuariocancelo = c.String(),
                        comentariodescuento = c.String(),
                        estacion = c.String(),
                        cambiorepartidor = c.Decimal(precision: 18, scale: 2),
                        usuariodescuento = c.String(),
                        fechacancelado = c.DateTime(),
                        idtipodescuento = c.String(),
                        numerotarjeta = c.String(),
                        folionotadeconsumo = c.Decimal(precision: 18, scale: 2),
                        notadeconsumo = c.Boolean(),
                        propinapagada = c.Boolean(),
                        propinafoliomovtocaja = c.Decimal(precision: 18, scale: 2),
                        puntosmonederogenerados = c.Decimal(precision: 18, scale: 2),
                        propinaincluida = c.Decimal(precision: 18, scale: 2),
                        tarjetadescuento = c.String(),
                        porcentajefac = c.Decimal(precision: 18, scale: 2),
                        usuariopago = c.String(),
                        propinamanual = c.Boolean(),
                        observaciones = c.String(),
                        idclientedomicilio = c.String(),
                        iddireccion = c.String(),
                        idclientefacturacion = c.String(),
                        telefonousadodomicilio = c.String(),
                        totalarticulos = c.Decimal(precision: 18, scale: 2),
                        subtotal = c.Decimal(precision: 18, scale: 2),
                        subtotalsinimpuestos = c.Decimal(precision: 18, scale: 2),
                        total = c.Decimal(precision: 18, scale: 2),
                        totalconpropina = c.Decimal(precision: 18, scale: 2),
                        totalsinimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalsindescuentosinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalimpuesto1 = c.Decimal(precision: 18, scale: 2),
                        totalalimentosconimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalbebidasconimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalotrosconimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalalimentossinimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalbebidassinimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalotrossinimpuestos = c.Decimal(precision: 18, scale: 2),
                        totaldescuentossinimpuestos = c.Decimal(precision: 18, scale: 2),
                        totaldescuentosconimpuestos = c.Decimal(precision: 18, scale: 2),
                        totaldescuentoalimentosconimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentobebidasconimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentootrosconimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentoalimentossinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentobebidassinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentootrossinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalcortesiassinimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalcortesiasconimpuestos = c.Decimal(precision: 18, scale: 2),
                        totalcortesiaalimentosconimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalcortesiabebidasconimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalcortesiaotrosconimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalcortesiaalimentossinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalcortesiabebidassinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totalcortesiaotrossinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentoycortesiasinimpuesto = c.Decimal(precision: 18, scale: 2),
                        totaldescuentoycortesiaconimpuesto = c.Decimal(precision: 18, scale: 2),
                        cargo = c.Decimal(precision: 18, scale: 2),
                        totalconcargo = c.Decimal(precision: 18, scale: 2),
                        totalconpropinacargo = c.Decimal(precision: 18, scale: 2),
                        descuentoimporte = c.Decimal(precision: 18, scale: 2),
                        efectivo = c.Decimal(precision: 18, scale: 2),
                        tarjeta = c.Decimal(precision: 18, scale: 2),
                        vales = c.Decimal(precision: 18, scale: 2),
                        otros = c.Decimal(precision: 18, scale: 2),
                        propina = c.Decimal(precision: 18, scale: 2),
                        propinatarjeta = c.Decimal(precision: 18, scale: 2),
                        totalalimentossinimpuestossindescuentos = c.Decimal(precision: 18, scale: 2),
                        totalbebidassinimpuestossindescuentos = c.Decimal(precision: 18, scale: 2),
                        totalotrossinimpuestossindescuentos = c.Decimal(precision: 18, scale: 2),
                        campoadicional1 = c.String(),
                        idreservacion = c.String(),
                        idcomisionista = c.String(),
                        importecomision = c.Decimal(precision: 18, scale: 2),
                        comisionpagada = c.Boolean(),
                        fechapagocomision = c.DateTime(),
                        foliopagocomision = c.Decimal(precision: 18, scale: 2),
                        tipoventarapida = c.Decimal(precision: 18, scale: 2),
                        callcenter = c.Boolean(),
                        idordencompra = c.Long(),
                        totalsindescuento = c.Decimal(precision: 18, scale: 2),
                        totalalimentos = c.Decimal(precision: 18, scale: 2),
                        totalbebidas = c.Decimal(precision: 18, scale: 2),
                        totalotros = c.Decimal(precision: 18, scale: 2),
                        totaldescuentos = c.Decimal(precision: 18, scale: 2),
                        totaldescuentoalimentos = c.Decimal(precision: 18, scale: 2),
                        totaldescuentobebidas = c.Decimal(precision: 18, scale: 2),
                        totaldescuentootros = c.Decimal(precision: 18, scale: 2),
                        totalcortesias = c.Decimal(precision: 18, scale: 2),
                        totalcortesiaalimentos = c.Decimal(precision: 18, scale: 2),
                        totalcortesiabebidas = c.Decimal(precision: 18, scale: 2),
                        totalcortesiaotros = c.Decimal(precision: 18, scale: 2),
                        totaldescuentoycortesia = c.Decimal(precision: 18, scale: 2),
                        totalalimentossindescuentos = c.Decimal(precision: 18, scale: 2),
                        totalbebidassindescuentos = c.Decimal(precision: 18, scale: 2),
                        totalotrossindescuentos = c.Decimal(precision: 18, scale: 2),
                        descuentocriterio = c.Decimal(precision: 18, scale: 2),
                        descuentomonedero = c.Decimal(precision: 18, scale: 2),
                        idmenucomedor = c.String(),
                        subtotalcondescuento = c.Decimal(precision: 18, scale: 2),
                        comisionpax = c.Decimal(precision: 18, scale: 2),
                        procesadointerfaz = c.Boolean(),
                        domicilioprogramado = c.Boolean(),
                        fechadomicilioprogramado = c.DateTime(),
                        enviado = c.Boolean(),
                        ncf = c.String(),
                        numerocuenta = c.String(),
                        codigo_unico_af = c.String(),
                        estatushub = c.Int(),
                        idfoliohub = c.Decimal(precision: 18, scale: 2),
                        EnviadoRW = c.Boolean(),
                        usuarioapertura = c.String(),
                        titulartarjetamonedero = c.String(),
                        saldoanteriormonedero = c.Decimal(precision: 18, scale: 2),
                        autorizacionfolio = c.String(),
                        fechalimiteemision = c.DateTime(),
                        totalimpuestod1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        totalimpuestod2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        totalimpuestod3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        idmotivocancela = c.String(),
                        sistema_envio = c.Int(),
                        idformadepagoDescuento = c.String(),
                        titulartarjetamonederodescuento = c.String(),
                        foliotempcheques = c.Long(),
                        c_iddispositivo = c.Int(),
                        surveycode = c.String(),
                        salerestaurantid = c.String(),
                        timemarktoconfirmed = c.DateTime(),
                        timemarktodelivery = c.DateTime(),
                        timemarktodeliveryarrive = c.DateTime(),
                        esalestatus = c.Int(),
                        statusSR = c.Int(),
                        paymentreference = c.String(),
                        deliverycharge = c.Decimal(precision: 18, scale: 2),
                        comandaimpresa = c.Boolean(),
                        foodorder = c.Int(),
                        cashpaymentwith = c.Decimal(precision: 18, scale: 2),
                        intentoEnvioAF = c.Int(),
                        paymentmethod_id = c.Int(),
                        TKC_Token = c.String(),
                        TKC_Transaction = c.String(),
                        TKC_Authorization = c.String(),
                        TKC_Cupon = c.String(),
                        TKC_ExpirationDate = c.String(),
                        TKC_Recompensa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        campoadicional3 = c.String(),
                        estrateca_CardNumber = c.String(),
                        estrateca_VoucherText = c.String(),
                        campoadicional4 = c.String(),
                        campoadicional5 = c.String(),
                        sacoa_CardNumber = c.String(),
                        sacoa_credits = c.Decimal(nullable: false, precision: 18, scale: 2),
                        estrateca_TypeDisccount = c.String(),
                        estrateca_DiscountCode = c.String(),
                        estrateca_DiscountID = c.String(),
                        estrateca_DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        desc_imp_original = c.Decimal(precision: 18, scale: 2),
                        donativo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        totalcondonativo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        totalconpropinacargodonativo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        orderreference = c.String(),
                        appname = c.String(),
                        paymentproviderid = c.String(),
                        paymentprovider = c.String(),
                        ChangeStatusSRX = c.Boolean(),
                        claveempresav = c.String(),
                        cuentaenuso = c.Boolean(),
                        modificado = c.Decimal(precision: 18, scale: 2),
                        campoadicional2 = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cheques_Detalle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TipoAccion = c.Int(nullable: false),
                        TipoClasificacion = c.Int(nullable: false),
                        Cambiado = c.Boolean(nullable: false),
                        FolioAnt = c.Long(nullable: false),
                        CantidadAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdProductoAnt = c.String(),
                        PrecioAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Impuesto1Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Impuesto2Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Impuesto3Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrecioSinImpuestosAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImpuestoImporte3Ant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrecioCatalogoAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdProductoCompuestoAnt = c.String(),
                        foliodet = c.Long(),
                        movimiento = c.Decimal(precision: 18, scale: 2),
                        comanda = c.String(),
                        cantidad = c.Decimal(precision: 18, scale: 2),
                        idproducto = c.String(),
                        descuento = c.Decimal(precision: 18, scale: 2),
                        precio = c.Decimal(precision: 18, scale: 2),
                        impuesto1 = c.Decimal(precision: 18, scale: 2),
                        impuesto2 = c.Decimal(precision: 18, scale: 2),
                        impuesto3 = c.Decimal(precision: 18, scale: 2),
                        preciosinimpuestos = c.Decimal(precision: 18, scale: 2),
                        tiempo = c.String(),
                        hora = c.DateTime(),
                        modificador = c.Boolean(),
                        mitad = c.Decimal(precision: 18, scale: 2),
                        comentario = c.String(),
                        idestacion = c.String(),
                        usuariodescuento = c.String(),
                        comentariodescuento = c.String(),
                        idtipodescuento = c.String(),
                        horaproduccion = c.DateTime(),
                        idproductocompuesto = c.String(),
                        productocompuestoprincipal = c.Boolean(),
                        preciocatalogo = c.Decimal(precision: 18, scale: 2),
                        marcar = c.Boolean(),
                        idmeseroproducto = c.String(),
                        prioridadproduccion = c.String(),
                        estatuspatin = c.Decimal(precision: 18, scale: 2),
                        idcortesia = c.String(),
                        numerotarjeta = c.String(),
                        estadomonitor = c.Decimal(precision: 18, scale: 2),
                        llavemovto = c.String(),
                        horameserofinalizado = c.DateTime(),
                        meserofinalizado = c.String(),
                        sistema_envio = c.Int(nullable: false),
                        idturno_cierre = c.Long(),
                        procesado = c.Boolean(),
                        promovolumen = c.Boolean(),
                        iddispositivo = c.Int(),
                        productsyncidsr = c.Int(),
                        subtotalsrx = c.Decimal(precision: 18, scale: 2),
                        totalsrx = c.Decimal(precision: 18, scale: 2),
                        idlistaprecio = c.Int(),
                        tipocambio = c.Decimal(precision: 18, scale: 2),
                        idmovtobillar = c.Long(nullable: false),
                        impuestoimporte3 = c.Decimal(precision: 18, scale: 2),
                        estrateca_DiscountCode = c.String(),
                        estrateca_DiscountID = c.String(),
                        estrateca_DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cheques_Pago",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TipoAccion = c.Int(nullable: false),
                        FolioAnt = c.Long(nullable: false),
                        IdFormadePagoAnt = c.String(),
                        ImporteAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PropinaAnt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        folio = c.Long(nullable: false),
                        idformadepago = c.String(),
                        importe = c.Decimal(precision: 18, scale: 2),
                        propina = c.Decimal(precision: 18, scale: 2),
                        tipodecambio = c.Decimal(precision: 18, scale: 2),
                        referencia = c.String(),
                        idturno_cierre = c.Long(),
                        procesado = c.Boolean(),
                        sistema_envio = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Configuracion_Sistema",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModificarVentasReales = c.Boolean(nullable: false),
                        MinProductosCuenta = c.Int(nullable: false),
                        EliminarProductosSeleccionados = c.Boolean(nullable: false),
                        Contrasena = c.String(),
                        ContrasenaAdmin = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Productos_Eliminacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Clave = c.String(),
                        Eliminar = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Productos_Reemplazo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reemplazar = c.Boolean(nullable: false),
                        Clave = c.String(),
                        Porcentaje = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Registro_Licencias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Anio = c.Int(nullable: false),
                        Mes = c.Int(nullable: false),
                        Licencia = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Turnos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TipoAccion = c.Int(nullable: false),
                        IdTurnoAnt = c.Long(nullable: false),
                        EfectivoAnterior = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TarjetaAnterior = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ValesAnterior = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreditoAnterior = c.Decimal(nullable: false, precision: 18, scale: 2),
                        idturnointerno = c.Long(nullable: false),
                        idturno = c.Long(),
                        fondo = c.Decimal(precision: 18, scale: 2),
                        apertura = c.DateTime(),
                        cierre = c.DateTime(),
                        idestacion = c.String(),
                        cajero = c.String(),
                        efectivo = c.Decimal(precision: 18, scale: 2),
                        tarjeta = c.Decimal(precision: 18, scale: 2),
                        vales = c.Decimal(precision: 18, scale: 2),
                        credito = c.Decimal(precision: 18, scale: 2),
                        procesadoweb = c.Boolean(),
                        idempresa = c.String(),
                        enviadoacentral = c.Boolean(),
                        fechaenviado = c.DateTime(),
                        usuarioenvio = c.String(),
                        offline = c.Boolean(),
                        enviadoaf = c.Boolean(),
                        corte_enviado = c.Boolean(),
                        eliminartemporalesencierre = c.Boolean(),
                        idmesero = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Turnos");
            DropTable("dbo.Registro_Licencias");
            DropTable("dbo.Productos_Reemplazo");
            DropTable("dbo.Productos_Eliminacion");
            DropTable("dbo.Configuracion_Sistema");
            DropTable("dbo.Cheques_Pago");
            DropTable("dbo.Cheques_Detalle");
            DropTable("dbo.Cheques");
        }
    }
}
