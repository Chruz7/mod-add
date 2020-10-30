﻿using mod_add.Datos.Enums;
using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using SR.Datos;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace mod_add.Utils
{
    public static class Funciones
    {
        public static List<T> CloneList<T>(List<T> oldList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<T>)formatter.Deserialize(stream);
        }


        public static void RegistrarModificacion(BitacoraModificacion bitacora)
        {
            DatabaseFactory dbf = new DatabaseFactory();
            IBitacoraServicio bitacoraServicio = new BitacoraServicio(dbf);

            bitacoraServicio.Create(bitacora);
        }

        public static Cheque ParseCheque(SR_cheques modelo, TipoAccion tipoAccion = TipoAccion.NINGUNO, TipoPago tipoPago = TipoPago.NINGUNO)
        {
            return new Cheque
            {
                TipoAccion = tipoAccion,
                TipoPago = tipoPago,
                TotalArticulosEliminados = 0,
                FolioAnt = modelo.folio,
                PropinaAnt = modelo.propina.Value,
                PropinaTarjetaAnt = modelo.propinatarjeta.Value,
                TotalArticulosAnt = modelo.totalarticulos.Value,
                SubtotalAnt = modelo.subtotal.Value,
                TotalAnt = modelo.total.Value,
                TotalConPropinaAnt = modelo.totalconpropina.Value,
                TotalConCargoAnt = modelo.totalconcargo.Value,
                TotalConPropinaCargoAnt = modelo.totalconpropinacargo.Value,
                DescuentoImporteAnt = modelo.descuentoimporte.Value,
                EfectivoAnt = modelo.efectivo.Value,
                TarjetaAnt = modelo.tarjeta.Value,
                ValesAnt = modelo.vales.Value,
                OtrosAnt = modelo.otros.Value,
                TotalSinDescuentoAnt = modelo.totalsindescuento.Value,
                TotalAlimentosAnt = modelo.totalalimentos.Value,
                TotalBebidasAnt = modelo.totalbebidas.Value,
                TotalOtrosAnt = modelo.totalotros.Value,
                TotalDescuentosAnt = modelo.totaldescuentos.Value,
                TotalDescuentoAlimentosAnt = modelo.totaldescuentoalimentos.Value,
                TotalDescuentoBebidasAnt = modelo.totaldescuentobebidas.Value,
                TotalDescuentoOtrosAnt = modelo.totaldescuentootros.Value,
                TotalDescuentoYCortesiaAnt = modelo.totaldescuentoycortesia.Value,
                TotalAlimentosSinDescuentosAnt = modelo.totalalimentossindescuentos.Value,
                TotalBebidasSinDescuentosAnt = modelo.totalbebidassindescuentos.Value,
                TotalOtrosSinDescuentosAnt = modelo.totalotrossindescuentos.Value,
                SubtotalConDescuentoAnt = modelo.subtotalcondescuento.Value,
                TotalImpuestoD1Ant = modelo.totalimpuestod1,
                TotalImpuestoD2Ant = modelo.totalimpuestod2,
                TotalImpuestoD3Ant = modelo.totalimpuestod3,
                TotalImpuesto1Ant = modelo.totalimpuesto1.Value,
                Desc_Imp_OriginalAnt = modelo.desc_imp_original.Value,
                CambioAnt = modelo.cambio.Value,
                CambioRepartidorAnt = modelo.cambiorepartidor.Value,
                IdTurnoAnt = (long)modelo.Idturno.Value,

                folio = modelo.folio,
                seriefolio = modelo.seriefolio,
                numcheque = modelo.Numcheque,
                numcheque_f = modelo.numcheque_f,
                fecha = modelo.fecha,
                salidarepartidor = modelo.salidarepartidor,
                arriborepartidor = modelo.arriborepartidor,
                cierre = modelo.cierre,
                mesa = modelo.mesa,
                nopersonas = modelo.Nopersonas,
                nopersonas_f = modelo.nopersonas_f,
                idmesero = modelo.idmesero,
                pagado = modelo.pagado,
                cancelado = modelo.cancelado,
                impreso = modelo.impreso,
                impresiones = modelo.Impresiones,
                impresiones_f = modelo.impresiones_f,
                cambio = modelo.cambio,
                descuento = modelo.descuento,
                reabiertas = modelo.Reabiertas,
                reabiertas_f = modelo.reabiertas_f,
                razoncancelado = modelo.razoncancelado,
                orden = modelo.Orden,
                orden_f = modelo.orden_f,
                facturado = modelo.facturado,
                idcliente = modelo.idcliente,
                idarearestaurant = modelo.idarearestaurant,
                idempresa = modelo.idempresa,
                tipodeservicio = modelo.Tipodeservicio,
                tipodeservicio_f = modelo.tipodeservicio_f,
                idturno = modelo.Idturno,
                idturno_f = modelo.idturno_f,
                usuariocancelo = modelo.usuariocancelo,
                comentariodescuento = modelo.comentariodescuento,
                estacion = modelo.estacion,
                cambiorepartidor = modelo.cambiorepartidor,
                usuariodescuento = modelo.usuariodescuento,
                fechacancelado = modelo.fechacancelado,
                idtipodescuento = modelo.idtipodescuento,
                numerotarjeta = modelo.numerotarjeta,
                folionotadeconsumo = modelo.Folionotadeconsumo,
                folionotadeconsumo_f = modelo.folionotadeconsumo_f,
                notadeconsumo = modelo.notadeconsumo,
                propinapagada = modelo.propinapagada,
                propinafoliomovtocaja = modelo.Propinafoliomovtocaja,
                propinafoliomovtocaja_f = modelo.propinafoliomovtocaja_f,
                puntosmonederogenerados = modelo.puntosmonederogenerados,
                propinaincluida = modelo.propinaincluida,
                tarjetadescuento = modelo.tarjetadescuento,
                porcentajefac = modelo.porcentajefac,
                usuariopago = modelo.usuariopago,
                propinamanual = modelo.propinamanual,
                observaciones = modelo.observaciones,
                idclientedomicilio = modelo.idclientedomicilio,
                iddireccion = modelo.iddireccion,
                idclientefacturacion = modelo.idclientefacturacion,
                telefonousadodomicilio = modelo.telefonousadodomicilio,
                totalarticulos = modelo.totalarticulos,
                subtotal = modelo.subtotal,
                subtotalsinimpuestos = modelo.subtotalsinimpuestos,
                total = modelo.total,
                totalconpropina = modelo.totalconpropina,
                totalsinimpuestos = modelo.totalsinimpuestos,
                totalsindescuentosinimpuesto = modelo.totalsindescuentosinimpuesto,
                totalimpuesto1 = modelo.totalimpuesto1,
                totalalimentosconimpuestos = modelo.totalalimentosconimpuestos,
                totalbebidasconimpuestos = modelo.totalbebidasconimpuestos,
                totalotrosconimpuestos = modelo.totalotrosconimpuestos,
                totalalimentossinimpuestos = modelo.totalalimentossinimpuestos,
                totalbebidassinimpuestos = modelo.totalbebidassinimpuestos,
                totalotrossinimpuestos = modelo.totalotrossinimpuestos,
                totaldescuentossinimpuestos = modelo.totaldescuentossinimpuestos,
                totaldescuentosconimpuestos = modelo.totaldescuentosconimpuestos,
                totaldescuentoalimentosconimpuesto = modelo.totaldescuentoalimentosconimpuesto,
                totaldescuentobebidasconimpuesto = modelo.totaldescuentobebidasconimpuesto,
                totaldescuentootrosconimpuesto = modelo.totaldescuentootrosconimpuesto,
                totaldescuentoalimentossinimpuesto = modelo.totaldescuentoalimentossinimpuesto,
                totaldescuentobebidassinimpuesto = modelo.totaldescuentobebidassinimpuesto,
                totaldescuentootrossinimpuesto = modelo.totaldescuentootrossinimpuesto,
                totalcortesiassinimpuestos = modelo.totalcortesiassinimpuestos,
                totalcortesiasconimpuestos = modelo.totalcortesiasconimpuestos,
                totalcortesiaalimentosconimpuesto = modelo.totalcortesiaalimentosconimpuesto,
                totalcortesiabebidasconimpuesto = modelo.totalcortesiabebidasconimpuesto,
                totalcortesiaotrosconimpuesto = modelo.totalcortesiaotrosconimpuesto,
                totalcortesiaalimentossinimpuesto = modelo.totalcortesiaalimentossinimpuesto,
                totalcortesiabebidassinimpuesto = modelo.totalcortesiabebidassinimpuesto,
                totalcortesiaotrossinimpuesto = modelo.totalcortesiaotrossinimpuesto,
                totaldescuentoycortesiasinimpuesto = modelo.totaldescuentoycortesiasinimpuesto,
                totaldescuentoycortesiaconimpuesto = modelo.totaldescuentoycortesiaconimpuesto,
                cargo = modelo.cargo,
                totalconcargo = modelo.totalconcargo,
                totalconpropinacargo = modelo.totalconpropinacargo,
                descuentoimporte = modelo.descuentoimporte,
                efectivo = modelo.efectivo,
                tarjeta = modelo.tarjeta,
                vales = modelo.vales,
                otros = modelo.otros,
                propina = modelo.propina,
                propinatarjeta = modelo.propinatarjeta,
                totalalimentossinimpuestossindescuentos = modelo.totalalimentossinimpuestossindescuentos,
                totalbebidassinimpuestossindescuentos = modelo.totalbebidassinimpuestossindescuentos,
                totalotrossinimpuestossindescuentos = modelo.totalotrossinimpuestossindescuentos,
                campoadicional1 = modelo.campoadicional1,
                idreservacion = modelo.idreservacion,
                idcomisionista = modelo.idcomisionista,
                importecomision = modelo.importecomision,
                comisionpagada = modelo.comisionpagada,
                fechapagocomision = modelo.fechapagocomision,
                foliopagocomision = modelo.foliopagocomision,
                tipoventarapida = modelo.tipoventarapida,
                callcenter = modelo.callcenter,
                idordencompra = modelo.idordencompra,
                totalsindescuento = modelo.totalsindescuento,
                totalalimentos = modelo.totalalimentos,
                totalbebidas = modelo.totalbebidas,
                totalotros = modelo.totalotros,
                totaldescuentos = modelo.totaldescuentos,
                totaldescuentoalimentos = modelo.totaldescuentoalimentos,
                totaldescuentobebidas = modelo.totaldescuentobebidas,
                totaldescuentootros = modelo.totaldescuentootros,
                totalcortesias = modelo.totalcortesias,
                totalcortesiaalimentos = modelo.totalcortesiaalimentos,
                totalcortesiabebidas = modelo.totalcortesiabebidas,
                totalcortesiaotros = modelo.totalcortesiaotros,
                totaldescuentoycortesia = modelo.totaldescuentoycortesia,
                totalalimentossindescuentos = modelo.totalalimentossindescuentos,
                totalbebidassindescuentos = modelo.totalbebidassindescuentos,
                totalotrossindescuentos = modelo.totalotrossindescuentos,
                descuentocriterio = modelo.descuentocriterio,
                descuentomonedero = modelo.descuentomonedero,
                idmenucomedor = modelo.idmenucomedor,
                subtotalcondescuento = modelo.subtotalcondescuento,
                comisionpax = modelo.comisionpax,
                procesadointerfaz = modelo.procesadointerfaz,
                domicilioprogramado = modelo.domicilioprogramado,
                fechadomicilioprogramado = modelo.fechadomicilioprogramado,
                enviado = modelo.enviado,
                ncf = modelo.ncf,
                numerocuenta = modelo.numerocuenta,
                codigo_unico_af = modelo.codigo_unico_af,
                estatushub = modelo.estatushub,
                idfoliohub = modelo.idfoliohub,
                EnviadoRW = modelo.EnviadoRW,
                usuarioapertura = modelo.usuarioapertura,
                titulartarjetamonedero = modelo.titulartarjetamonedero,
                saldoanteriormonedero = modelo.saldoanteriormonedero,
                autorizacionfolio = modelo.autorizacionfolio,
                fechalimiteemision = modelo.fechalimiteemision,
                totalimpuestod1 = modelo.totalimpuestod1,
                totalimpuestod2 = modelo.totalimpuestod2,
                totalimpuestod3 = modelo.totalimpuestod3,
                idmotivocancela = modelo.idmotivocancela,
                sistema_envio = modelo.sistema_envio,
                idformadepagoDescuento = modelo.idformadepagoDescuento,
                titulartarjetamonederodescuento = modelo.titulartarjetamonederodescuento,
                foliotempcheques = modelo.foliotempcheques,
                c_iddispositivo = modelo.c_iddispositivo,
                surveycode = modelo.surveycode,
                salerestaurantid = modelo.salerestaurantid,
                timemarktoconfirmed = modelo.timemarktoconfirmed,
                timemarktodelivery = modelo.timemarktodelivery,
                timemarktodeliveryarrive = modelo.timemarktodeliveryarrive,
                esalestatus = modelo.esalestatus,
                statusSR = modelo.statusSR,
                paymentreference = modelo.paymentreference,
                deliverycharge = modelo.deliverycharge,
                comandaimpresa = modelo.comandaimpresa,
                foodorder = modelo.foodorder,
                cashpaymentwith = modelo.cashpaymentwith,
                intentoEnvioAF = modelo.intentoEnvioAF,
                paymentmethod_id = modelo.paymentmethod_id,
                TKC_Token = modelo.TKC_Token,
                TKC_Transaction = modelo.TKC_Transaction,
                TKC_Authorization = modelo.TKC_Authorization,
                TKC_Cupon = modelo.TKC_Cupon,
                TKC_ExpirationDate = modelo.TKC_ExpirationDate,
                TKC_Recompensa = modelo.TKC_Recompensa,
                campoadicional3 = modelo.campoadicional3,
                estrateca_CardNumber = modelo.estrateca_CardNumber,
                estrateca_VoucherText = modelo.estrateca_VoucherText,
                campoadicional4 = modelo.campoadicional4,
                campoadicional5 = modelo.campoadicional5,
                sacoa_CardNumber = modelo.sacoa_CardNumber,
                sacoa_credits = modelo.sacoa_credits,
                estrateca_TypeDisccount = modelo.estrateca_TypeDisccount,
                estrateca_DiscountCode = modelo.estrateca_DiscountCode,
                estrateca_DiscountID = modelo.estrateca_DiscountID,
                estrateca_DiscountAmount = modelo.estrateca_DiscountAmount,
                desc_imp_original = modelo.desc_imp_original,
                donativo = modelo.donativo,
                totalcondonativo = modelo.totalcondonativo,
                totalconpropinacargodonativo = modelo.totalconpropinacargodonativo,
                orderreference = modelo.orderreference,
                appname = modelo.appname,
                paymentproviderid = modelo.paymentproviderid,
                paymentprovider = modelo.paymentprovider,
                ChangeStatusSRX = modelo.ChangeStatusSRX,
                claveempresav = modelo.claveempresav,
                cuentaenuso = modelo.cuentaenuso,
                modificado = modelo.modificado,
                campoadicional2 = modelo.campoadicional2,
            };
        }

        public static SR_cheques ParseSR_cheques(Cheque modelo)
        {
            return new SR_cheques
            {
                folio = modelo.folio,
                seriefolio = modelo.seriefolio,
                numcheque = modelo.numcheque,
                numcheque_f = modelo.numcheque_f,
                fecha = modelo.fecha,
                salidarepartidor = modelo.salidarepartidor,
                arriborepartidor = modelo.arriborepartidor,
                cierre = modelo.cierre,
                mesa = modelo.mesa,
                nopersonas = modelo.nopersonas,
                nopersonas_f = modelo.nopersonas_f,
                idmesero = modelo.idmesero,
                pagado = modelo.pagado,
                cancelado = modelo.cancelado,
                impreso = modelo.impreso,
                impresiones = modelo.impresiones,
                impresiones_f = modelo.impresiones_f,
                cambio = modelo.cambio,
                descuento = modelo.descuento,
                reabiertas = modelo.reabiertas,
                reabiertas_f = modelo.reabiertas_f,
                razoncancelado = modelo.razoncancelado,
                orden = modelo.orden,
                orden_f = modelo.orden_f,
                facturado = modelo.facturado,
                idcliente = modelo.idcliente,
                idarearestaurant = modelo.idarearestaurant,
                idempresa = modelo.idempresa,
                tipodeservicio = modelo.tipodeservicio,
                tipodeservicio_f = modelo.tipodeservicio_f,
                idturno = modelo.idturno,
                idturno_f = modelo.idturno_f,
                usuariocancelo = modelo.usuariocancelo,
                comentariodescuento = modelo.comentariodescuento,
                estacion = modelo.estacion,
                cambiorepartidor = modelo.cambiorepartidor,
                usuariodescuento = modelo.usuariodescuento,
                fechacancelado = modelo.fechacancelado,
                idtipodescuento = modelo.idtipodescuento,
                numerotarjeta = modelo.numerotarjeta,
                folionotadeconsumo = modelo.folionotadeconsumo,
                folionotadeconsumo_f = modelo.folionotadeconsumo_f,
                notadeconsumo = modelo.notadeconsumo,
                propinapagada = modelo.propinapagada,
                propinafoliomovtocaja = modelo.propinafoliomovtocaja,
                propinafoliomovtocaja_f = modelo.propinafoliomovtocaja_f,
                puntosmonederogenerados = modelo.puntosmonederogenerados,
                propinaincluida = modelo.propinaincluida,
                tarjetadescuento = modelo.tarjetadescuento,
                porcentajefac = modelo.porcentajefac,
                usuariopago = modelo.usuariopago,
                propinamanual = modelo.propinamanual,
                observaciones = modelo.observaciones,
                idclientedomicilio = modelo.idclientedomicilio,
                iddireccion = modelo.iddireccion,
                idclientefacturacion = modelo.idclientefacturacion,
                telefonousadodomicilio = modelo.telefonousadodomicilio,
                totalarticulos = modelo.totalarticulos,
                subtotal = modelo.subtotal,
                subtotalsinimpuestos = modelo.subtotalsinimpuestos,
                total = modelo.total,
                totalconpropina = modelo.totalconpropina,
                totalsinimpuestos = modelo.totalsinimpuestos,
                totalsindescuentosinimpuesto = modelo.totalsindescuentosinimpuesto,
                totalimpuesto1 = modelo.totalimpuesto1,
                totalalimentosconimpuestos = modelo.totalalimentosconimpuestos,
                totalbebidasconimpuestos = modelo.totalbebidasconimpuestos,
                totalotrosconimpuestos = modelo.totalotrosconimpuestos,
                totalalimentossinimpuestos = modelo.totalalimentossinimpuestos,
                totalbebidassinimpuestos = modelo.totalbebidassinimpuestos,
                totalotrossinimpuestos = modelo.totalotrossinimpuestos,
                totaldescuentossinimpuestos = modelo.totaldescuentossinimpuestos,
                totaldescuentosconimpuestos = modelo.totaldescuentosconimpuestos,
                totaldescuentoalimentosconimpuesto = modelo.totaldescuentoalimentosconimpuesto,
                totaldescuentobebidasconimpuesto = modelo.totaldescuentobebidasconimpuesto,
                totaldescuentootrosconimpuesto = modelo.totaldescuentootrosconimpuesto,
                totaldescuentoalimentossinimpuesto = modelo.totaldescuentoalimentossinimpuesto,
                totaldescuentobebidassinimpuesto = modelo.totaldescuentobebidassinimpuesto,
                totaldescuentootrossinimpuesto = modelo.totaldescuentootrossinimpuesto,
                totalcortesiassinimpuestos = modelo.totalcortesiassinimpuestos,
                totalcortesiasconimpuestos = modelo.totalcortesiasconimpuestos,
                totalcortesiaalimentosconimpuesto = modelo.totalcortesiaalimentosconimpuesto,
                totalcortesiabebidasconimpuesto = modelo.totalcortesiabebidasconimpuesto,
                totalcortesiaotrosconimpuesto = modelo.totalcortesiaotrosconimpuesto,
                totalcortesiaalimentossinimpuesto = modelo.totalcortesiaalimentossinimpuesto,
                totalcortesiabebidassinimpuesto = modelo.totalcortesiabebidassinimpuesto,
                totalcortesiaotrossinimpuesto = modelo.totalcortesiaotrossinimpuesto,
                totaldescuentoycortesiasinimpuesto = modelo.totaldescuentoycortesiasinimpuesto,
                totaldescuentoycortesiaconimpuesto = modelo.totaldescuentoycortesiaconimpuesto,
                cargo = modelo.cargo,
                totalconcargo = modelo.totalconcargo,
                totalconpropinacargo = modelo.totalconpropinacargo,
                descuentoimporte = modelo.descuentoimporte,
                efectivo = modelo.efectivo,
                tarjeta = modelo.tarjeta,
                vales = modelo.vales,
                otros = modelo.otros,
                propina = modelo.propina,
                propinatarjeta = modelo.propinatarjeta,
                totalalimentossinimpuestossindescuentos = modelo.totalalimentossinimpuestossindescuentos,
                totalbebidassinimpuestossindescuentos = modelo.totalbebidassinimpuestossindescuentos,
                totalotrossinimpuestossindescuentos = modelo.totalotrossinimpuestossindescuentos,
                campoadicional1 = modelo.campoadicional1,
                idreservacion = modelo.idreservacion,
                idcomisionista = modelo.idcomisionista,
                importecomision = modelo.importecomision,
                comisionpagada = modelo.comisionpagada,
                fechapagocomision = modelo.fechapagocomision,
                foliopagocomision = modelo.foliopagocomision,
                tipoventarapida = modelo.tipoventarapida,
                callcenter = modelo.callcenter,
                idordencompra = modelo.idordencompra,
                totalsindescuento = modelo.totalsindescuento,
                totalalimentos = modelo.totalalimentos,
                totalbebidas = modelo.totalbebidas,
                totalotros = modelo.totalotros,
                totaldescuentos = modelo.totaldescuentos,
                totaldescuentoalimentos = modelo.totaldescuentoalimentos,
                totaldescuentobebidas = modelo.totaldescuentobebidas,
                totaldescuentootros = modelo.totaldescuentootros,
                totalcortesias = modelo.totalcortesias,
                totalcortesiaalimentos = modelo.totalcortesiaalimentos,
                totalcortesiabebidas = modelo.totalcortesiabebidas,
                totalcortesiaotros = modelo.totalcortesiaotros,
                totaldescuentoycortesia = modelo.totaldescuentoycortesia,
                totalalimentossindescuentos = modelo.totalalimentossindescuentos,
                totalbebidassindescuentos = modelo.totalbebidassindescuentos,
                totalotrossindescuentos = modelo.totalotrossindescuentos,
                descuentocriterio = modelo.descuentocriterio,
                descuentomonedero = modelo.descuentomonedero,
                idmenucomedor = modelo.idmenucomedor,
                subtotalcondescuento = modelo.subtotalcondescuento,
                comisionpax = modelo.comisionpax,
                procesadointerfaz = modelo.procesadointerfaz,
                domicilioprogramado = modelo.domicilioprogramado,
                fechadomicilioprogramado = modelo.fechadomicilioprogramado,
                enviado = modelo.enviado,
                ncf = modelo.ncf,
                numerocuenta = modelo.numerocuenta,
                codigo_unico_af = modelo.codigo_unico_af,
                estatushub = modelo.estatushub,
                idfoliohub = modelo.idfoliohub,
                EnviadoRW = modelo.EnviadoRW,
                usuarioapertura = modelo.usuarioapertura,
                titulartarjetamonedero = modelo.titulartarjetamonedero,
                saldoanteriormonedero = modelo.saldoanteriormonedero,
                autorizacionfolio = modelo.autorizacionfolio,
                fechalimiteemision = modelo.fechalimiteemision,
                totalimpuestod1 = modelo.totalimpuestod1,
                totalimpuestod2 = modelo.totalimpuestod2,
                totalimpuestod3 = modelo.totalimpuestod3,
                idmotivocancela = modelo.idmotivocancela,
                sistema_envio = modelo.sistema_envio,
                idformadepagoDescuento = modelo.idformadepagoDescuento,
                titulartarjetamonederodescuento = modelo.titulartarjetamonederodescuento,
                foliotempcheques = modelo.foliotempcheques,
                c_iddispositivo = modelo.c_iddispositivo,
                surveycode = modelo.surveycode,
                salerestaurantid = modelo.salerestaurantid,
                timemarktoconfirmed = modelo.timemarktoconfirmed,
                timemarktodelivery = modelo.timemarktodelivery,
                timemarktodeliveryarrive = modelo.timemarktodeliveryarrive,
                esalestatus = modelo.esalestatus,
                statusSR = modelo.statusSR,
                paymentreference = modelo.paymentreference,
                deliverycharge = modelo.deliverycharge,
                comandaimpresa = modelo.comandaimpresa,
                foodorder = modelo.foodorder,
                cashpaymentwith = modelo.cashpaymentwith,
                intentoEnvioAF = modelo.intentoEnvioAF,
                paymentmethod_id = modelo.paymentmethod_id,
                TKC_Token = modelo.TKC_Token,
                TKC_Transaction = modelo.TKC_Transaction,
                TKC_Authorization = modelo.TKC_Authorization,
                TKC_Cupon = modelo.TKC_Cupon,
                TKC_ExpirationDate = modelo.TKC_ExpirationDate,
                TKC_Recompensa = modelo.TKC_Recompensa,
                campoadicional3 = modelo.campoadicional3,
                estrateca_CardNumber = modelo.estrateca_CardNumber,
                estrateca_VoucherText = modelo.estrateca_VoucherText,
                campoadicional4 = modelo.campoadicional4,
                campoadicional5 = modelo.campoadicional5,
                sacoa_CardNumber = modelo.sacoa_CardNumber,
                sacoa_credits = modelo.sacoa_credits,
                estrateca_TypeDisccount = modelo.estrateca_TypeDisccount,
                estrateca_DiscountCode = modelo.estrateca_DiscountCode,
                estrateca_DiscountID = modelo.estrateca_DiscountID,
                estrateca_DiscountAmount = modelo.estrateca_DiscountAmount,
                desc_imp_original = modelo.desc_imp_original,
                donativo = modelo.donativo,
                totalcondonativo = modelo.totalcondonativo,
                totalconpropinacargodonativo = modelo.totalconpropinacargodonativo,
                orderreference = modelo.orderreference,
                appname = modelo.appname,
                paymentproviderid = modelo.paymentproviderid,
                paymentprovider = modelo.paymentprovider,
                ChangeStatusSRX = modelo.ChangeStatusSRX,
                claveempresav = modelo.claveempresav,
                cuentaenuso = modelo.cuentaenuso,
                modificado = modelo.modificado,
                campoadicional2 = modelo.campoadicional2,
            };
        }

        public static ChequeDetalle ParseChequeDetalle(SR_cheqdet modelo, TipoAccion tipoAccion = TipoAccion.NINGUNO, TipoClasificacion tipoClasificacion = TipoClasificacion.NINGUNO)
        {
            return new ChequeDetalle
            {
                TipoAccion = tipoAccion,
                TipoClasificacion = tipoClasificacion,
                Cambiado = false,
                FolioAnt = modelo.foliodet.Value,
                CantidadAnt = modelo.cantidad.Value,
                IdProductoCompuestoAnt = modelo.idproductocompuesto,
                IdProductoAnt = modelo.idproducto,
                PrecioAnt = modelo.precio.Value,
                Impuesto1Ant = modelo.impuesto1.Value,
                Impuesto2Ant = modelo.impuesto2.Value,
                Impuesto3Ant = modelo.impuesto3.Value,
                PrecioSinImpuestosAnt = modelo.preciosinimpuestos.Value,
                PrecioCatalogoAnt = modelo.preciocatalogo.Value,
                ImpuestoImporte3Ant = modelo.impuestoimporte3.Value,

                foliodet = modelo.foliodet,
                movimiento = modelo.movimiento,
                comanda = modelo.comanda,
                cantidad = modelo.cantidad,
                idproducto = modelo.idproducto,
                descuento = modelo.descuento,
                precio = modelo.precio,
                impuesto1 = modelo.impuesto1,
                impuesto2 = modelo.impuesto2,
                impuesto3 = modelo.impuesto3,
                preciosinimpuestos = modelo.preciosinimpuestos,
                tiempo = modelo.tiempo,
                hora = modelo.hora,
                modificador = modelo.modificador,
                mitad = modelo.mitad,
                comentario = modelo.comentario,
                idestacion = modelo.idestacion,
                usuariodescuento = modelo.usuariodescuento,
                comentariodescuento = modelo.comentariodescuento,
                idtipodescuento = modelo.idtipodescuento,
                horaproduccion = modelo.horaproduccion,
                idproductocompuesto = modelo.idproductocompuesto,
                productocompuestoprincipal = modelo.productocompuestoprincipal,
                preciocatalogo = modelo.preciocatalogo,
                marcar = modelo.marcar,
                idmeseroproducto = modelo.idmeseroproducto,
                prioridadproduccion = modelo.prioridadproduccion,
                estatuspatin = modelo.estatuspatin,
                idcortesia = modelo.idcortesia,
                numerotarjeta = modelo.numerotarjeta,
                estadomonitor = modelo.estadomonitor,
                llavemovto = modelo.llavemovto,
                horameserofinalizado = modelo.horameserofinalizado,
                meserofinalizado = modelo.meserofinalizado,
                sistema_envio = modelo.sistema_envio,
                idturno_cierre = modelo.idturno_cierre,
                procesado = modelo.procesado,
                promovolumen = modelo.promovolumen,
                iddispositivo = modelo.iddispositivo,
                productsyncidsr = modelo.productsyncidsr,
                subtotalsrx = modelo.subtotalsrx,
                totalsrx = modelo.totalsrx,
                idlistaprecio = modelo.idlistaprecio,
                tipocambio = modelo.tipocambio,
                idmovtobillar = modelo.idmovtobillar,
                impuestoimporte3 = modelo.impuestoimporte3,
                estrateca_DiscountCode = modelo.estrateca_DiscountCode,
                estrateca_DiscountID = modelo.estrateca_DiscountID,
                estrateca_DiscountAmount = modelo.estrateca_DiscountAmount,
            };
        }

        public static SR_cheqdet ParseSR_cheqdet(ChequeDetalle modelo)
        {
            return new SR_cheqdet
            {
                foliodet = modelo.foliodet,
                movimiento = modelo.movimiento,
                comanda = modelo.comanda,
                cantidad = modelo.cantidad,
                idproducto = modelo.idproducto,
                descuento = modelo.descuento,
                precio = modelo.precio,
                impuesto1 = modelo.impuesto1,
                impuesto2 = modelo.impuesto2,
                impuesto3 = modelo.impuesto3,
                preciosinimpuestos = modelo.preciosinimpuestos,
                tiempo = modelo.tiempo,
                hora = modelo.hora,
                modificador = modelo.modificador,
                mitad = modelo.mitad,
                comentario = modelo.comentario,
                idestacion = modelo.idestacion,
                usuariodescuento = modelo.usuariodescuento,
                comentariodescuento = modelo.comentariodescuento,
                idtipodescuento = modelo.idtipodescuento,
                horaproduccion = modelo.horaproduccion,
                idproductocompuesto = modelo.idproductocompuesto,
                productocompuestoprincipal = modelo.productocompuestoprincipal,
                preciocatalogo = modelo.preciocatalogo,
                marcar = modelo.marcar,
                idmeseroproducto = modelo.idmeseroproducto,
                prioridadproduccion = modelo.prioridadproduccion,
                estatuspatin = modelo.estatuspatin,
                idcortesia = modelo.idcortesia,
                numerotarjeta = modelo.numerotarjeta,
                estadomonitor = modelo.estadomonitor,
                llavemovto = modelo.llavemovto,
                horameserofinalizado = modelo.horameserofinalizado,
                meserofinalizado = modelo.meserofinalizado,
                sistema_envio = modelo.sistema_envio,
                idturno_cierre = modelo.idturno_cierre,
                procesado = modelo.procesado,
                promovolumen = modelo.promovolumen,
                iddispositivo = modelo.iddispositivo,
                productsyncidsr = modelo.productsyncidsr,
                subtotalsrx = modelo.subtotalsrx,
                totalsrx = modelo.totalsrx,
                idlistaprecio = modelo.idlistaprecio,
                tipocambio = modelo.tipocambio,
                idmovtobillar = modelo.idmovtobillar,
                impuestoimporte3 = modelo.impuestoimporte3,
                estrateca_DiscountCode = modelo.estrateca_DiscountCode,
                estrateca_DiscountID = modelo.estrateca_DiscountID,
                estrateca_DiscountAmount = modelo.estrateca_DiscountAmount,
            };
        }

        public static ChequePago ParseChequePago(SR_chequespagos modelo, TipoAccion tipoAccion = TipoAccion.NINGUNO)
        {
            return new ChequePago
            {
                TipoAccion = tipoAccion,
                FolioAnt = modelo.folio,
                ImporteAnt = modelo.importe.Value,
                PropinaAnt = modelo.propina.Value,

                folio = modelo.folio,
                idformadepago = modelo.idformadepago,
                importe = modelo.importe,
                propina = modelo.propina,
                tipodecambio = modelo.tipodecambio,
                referencia = modelo.referencia,
                idturno_cierre = modelo.idturno_cierre,
                procesado = modelo.procesado,
                sistema_envio = modelo.sistema_envio,
            };
        }

        public static SR_chequespagos ParseSR_chequespagos(ChequePago modelo)
        {
            return new SR_chequespagos
            {
                folio = modelo.folio,
                idformadepago = modelo.idformadepago,
                importe = modelo.importe,
                propina = modelo.propina,
                tipodecambio = modelo.tipodecambio,
                referencia = modelo.referencia,
                idturno_cierre = modelo.idturno_cierre,
                procesado = modelo.procesado,
                sistema_envio = modelo.sistema_envio,
            };
        }

        public static Turno ParseTurno(SR_turnos modelo, TipoAccion tipoAccion)
        {
            return new Turno
            {
                TipoAccion = tipoAccion,
                IdTurnoAnt = modelo.idturno.Value,
                EfectivoAnterior = modelo.efectivo.Value,
                TarjetaAnterior = modelo.tarjeta.Value,
                ValesAnterior = modelo.vales.Value,
                CreditoAnterior = modelo.credito.Value,

                idturnointerno = modelo.idturnointerno,
                idturno = modelo.idturno,
                fondo = modelo.fondo,
                apertura = modelo.apertura,
                cierre = modelo.cierre,
                idestacion = modelo.idestacion,
                cajero = modelo.cajero,
                efectivo = modelo.efectivo,
                tarjeta = modelo.tarjeta,
                vales = modelo.vales,
                credito = modelo.credito,
                procesadoweb = modelo.procesadoweb,
                idempresa = modelo.idempresa,
                enviadoacentral = modelo.enviadoacentral,
                fechaenviado = modelo.fechaenviado,
                usuarioenvio = modelo.usuarioenvio,
                offline = modelo.offline,
                enviadoaf = modelo.enviadoaf,
                corte_enviado = modelo.corte_enviado,
                eliminartemporalesencierre = modelo.eliminartemporalesencierre,
                idmesero = modelo.idmesero,
            };
        }

        public static SR_turnos ParseSR_turnos(Turno modelo)
        {
            return new SR_turnos
            {
                idturnointerno = modelo.idturnointerno,
                idturno = modelo.idturno,
                fondo = modelo.fondo,
                apertura = modelo.apertura,
                cierre = modelo.cierre,
                idestacion = modelo.idestacion,
                cajero = modelo.cajero,
                efectivo = modelo.efectivo,
                tarjeta = modelo.tarjeta,
                vales = modelo.vales,
                credito = modelo.credito,
                procesadoweb = modelo.procesadoweb,
                idempresa = modelo.idempresa,
                enviadoacentral = modelo.enviadoacentral,
                fechaenviado = modelo.fechaenviado,
                usuarioenvio = modelo.usuarioenvio,
                offline = modelo.offline,
                enviadoaf = modelo.enviadoaf,
                corte_enviado = modelo.corte_enviado,
                eliminartemporalesencierre = modelo.eliminartemporalesencierre,
                idmesero = modelo.idmesero,
            };
        }
    }
}
