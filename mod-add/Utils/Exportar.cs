using mod_add.Datos.Modelos;
using mod_add.Enums;
using SpreadsheetLight;
using SR.Datos;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace mod_add.Utils
{
    public class Exportar
    {
        public string PathBitacora { get; set; }
        public string PathDetalladoVertical { get; set; }
        public string PathDetalladoHorizontal { get; set; }
        public Exportar()
        {
            PathBitacora = ConfiguracionLocalServicio.ReadSetting("PATH-BITACORA");
            PathDetalladoVertical = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-VERTICAL");
            PathDetalladoHorizontal = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-HORIZONTAL");
        }

        public TipoRespuesta BicatoraExcel(List<SR_bitacorafiscal> bitacora)
        {
            if (bitacora.Count == 0) return TipoRespuesta.SIN_REGISTROS;
            DateTime fecha = DateTime.Now;
            string fechaString = fecha.ToString("yyyyMMddHHmmss");
            string pathArchivo = Path.Combine("PathBitacora", $"bitacora-{fechaString}.xlsx");

            try
            {
                SLDocument sl = new SLDocument();

                SLStyle styleColumn = sl.CreateStyle();
                styleColumn.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
                styleColumn.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
                styleColumn.SetWrapText(true);

                SLStyle styleColumn2 = sl.CreateStyle();
                styleColumn2.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right);
                styleColumn2.SetWrapText(true);

                sl.SetColumnStyle(1, styleColumn);
                sl.SetColumnStyle(2, styleColumn);
                sl.SetColumnStyle(3, styleColumn);
                sl.SetColumnStyle(4, styleColumn);
                sl.SetColumnStyle(5, styleColumn);
                sl.SetColumnStyle(9, styleColumn);

                styleColumn.FormatCode = "#,##0.00";
                sl.SetColumnStyle(8, styleColumn);

                styleColumn2.FormatCode = "#,##0.0000";
                sl.SetColumnStyle(6, styleColumn2);
                sl.SetColumnStyle(7, styleColumn2);

                sl.SetCellStyle(1, 6, styleColumn);
                sl.SetCellStyle(1, 7, styleColumn);

                sl.SetColumnWidth(1, 24);
                sl.SetColumnWidth(2, 24);
                sl.SetColumnWidth(3, 24);
                sl.SetColumnWidth(4, 15);
                sl.SetColumnWidth(5, 15);
                sl.SetColumnWidth(6, 15);
                sl.SetColumnWidth(7, 15);
                sl.SetColumnWidth(8, 15);
                sl.SetColumnWidth(9, 15);

                sl.SetCellValue(1, 1, "FECHA DE PROCESO");
                sl.SetCellValue(1, 2, "FECHA INICIAL (VENTA)");
                sl.SetCellValue(1, 3, "FECHA FINAL (VENTA)");
                sl.SetCellValue(1, 4, $"TOTAL CUENTAS");
                sl.SetCellValue(1, 5, "CUENTAS MODIFICADAS");
                sl.SetCellValue(1, 6, "IMPORTE ANTERIOR");
                sl.SetCellValue(1, 7, "IMPORTE NUEVO");
                sl.SetCellValue(1, 8, "DIFERENCIA %");
                sl.SetCellValue(1, 9, "TIPO DE ELIMINACION");
                //sl.SetCellValue(1, 10, "MODO DE ELIMINACION");

                int i = 2;
                foreach (var resgitro in bitacora)
                {
                    sl.SetCellValue(i, 1, resgitro.fecha.Value.ToString());
                    sl.SetCellValue(i, 2, resgitro.fechainicial.Value.ToString());
                    sl.SetCellValue(i, 3, resgitro.fechafinal.Value.ToString());
                    sl.SetCellValue(i, 4, resgitro.cuentastotal.Value);
                    sl.SetCellValue(i, 5, resgitro.cuentasmodificadas.Value);
                    sl.SetCellValue(i, 6, resgitro.importeanterior.Value);
                    sl.SetCellValue(i, 7, resgitro.importenuevo.Value);
                    sl.SetCellValue(i, 8, resgitro.diferencia.Value);
                    sl.SetCellValue(i, 9, resgitro.tipo);
                    //sl.SetCellValue(i, 10, resgitro.tipo);
                    i++;
                }
                sl.SaveAs(pathArchivo);

                return TipoRespuesta.HECHO;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                return TipoRespuesta.ERROR;
            }
        }

        public void DetallesCuentas(TipoReporte tipoReporte, DateTime fechaInicial, DateTime fechaFinal, List<SR_cheques> cheques)
        {
            try
            {
                SLDocument sl = new SLDocument();

                List<string> columnas = new List<string>
                {
                    "folio",
                    "seriefolio",
                    "numcheque",
                    "fecha",
                    "salidarepartidor",
                    "arriborepartidor",
                    "cierre",
                    "mesa",
                    "nopersonas",
                    "idmesero",
                    "pagado",
                    "cancelado",
                    "impreso",
                    "impresiones",
                    "cambio",
                    "descuento",
                    "reabiertas",
                    "razoncancelado",
                    "orden",
                    "facturado",
                    "idcliente",
                    "idarearestaurant",
                    "claveempresav",
                    "tipodeservicio",
                    "idturno",
                    "usuariocancelo",
                    "comentariodescuento",
                    "estacion",
                    "cambiorepartidor",
                    "usuariodescuento",
                    "fechacancelado",
                    "idtipodescuento",
                    "numerotarjeta",
                    "folionotadeconsumo",
                    "notadeconsumo",
                    "propinapagada",
                    "propinafoliomovtocaja",
                    "puntosmonederogenerados",
                    "propinaincluida",
                    "tarjetadescuento",
                    "porcentajefac",
                    "propinamanual",
                    "usuariopago",
                    "idclientefacturacion",
                    "cuentaenuso",
                    "observaciones",
                    "idclientedomicilio",
                    "iddireccion",
                    "telefonousadodomicilio",
                    "totalarticulos",
                    "subtotal",
                    "subtotalsinimpuestos",
                    "total",
                    "totalconpropina",
                    "totalimpuesto1",
                    "cargo",
                    "totalconcargo",
                    "totalconpropinacargo",
                    "descuentoimporte",
                    "efectivo",
                    "tarjeta",
                    "vales",
                    "otros",
                    "propina",
                    "propinatarjeta",
                    "campoadicional1",
                    "idreservacion",
                    "idcomisionista",
                    "importecomision",
                    "comisionpagada",
                    "fechapagocomision",
                    "foliopagocomision",
                    "tipoventarapida",
                    "callcenter",
                    "idordencompra",
                    "idempresa",
                    "totalsindescuento",
                    "totalalimentos",
                    "totalbebidas",
                    "totalotros",
                    "totaldescuentos",
                    "totaldescuentoalimentos",
                    "totaldescuentobebidas",
                    "totaldescuentootros",
                    "totalcortesias",
                    "totalcortesiaalimentos",
                    "totalcortesiabebidas",
                    "totalcortesiaotros",
                    "totaldescuentoycortesia",
                    "totalalimentossindescuentos",
                    "totalbebidassindescuentos",
                    "totalotrossindescuentos",
                    "descuentocriterio",
                    "descuentomonedero",
                    "idmenucomedor",
                    "subtotalcondescuento",
                    "comisionpax",
                    "procesadointerfaz",
                    "domicilioprogramado",
                    "fechadomicilioprogramado",
                    "numerocuenta",
                    "codigo_unico_af",
                    "modificado",
                    "EnviadoRW",
                    "usuarioapertura",
                    "autorizacionfolio",
                    "fechalimiteemision",
                    "totalimpuestod1",
                    "totalimpuestod2",
                    "totalimpuestod3",
                    "idmotivocancela",
                    "titulartarjetamonedero",
                    "saldoanteriormonedero",
                    "ncf",
                    "idformadepagoDescuento",
                    "titulartarjetamonederodescuento",
                    "surveycode",
                    "TKC_Authorization",
                    "TKC_Cupon",
                    "TKC_ExpirationDate",
                    "TKC_Recompensa",
                    "campoadicional2",
                    "campoadicional3",
                    "estrateca_CardNumber",
                    "estrateca_VoucherText",
                    "campoadicional4",
                    "campoadicional5",
                    "sacoa_CardNumber",
                    "sacoa_credits",
                    "estrateca_TypeDisccount",
                    "estrateca_DiscountCode",
                    "estrateca_DiscountID",
                    "estrateca_DiscountAmount",
                    "desc_imp_original",
                    "donativo",
                    "totalcondonativo",
                    "totalconpropinacargodonativo",
                    "orderreference",
                    "appname",
                    "paymentproviderid",
                    "paymentprovider",
                    "ChangeStatusSRX",
                };

                int icolumna = 0;
                foreach (var columna in columnas)
                {
                    icolumna++;
                    sl.SetCellValue(1, icolumna, columna);
                }

                int ifila = 1;
                foreach (var cheque in cheques)
                {
                    ifila++;

                    sl.SetCellValue(ifila, 1, cheque.folio);
                    sl.SetCellValue(ifila, 2, cheque.seriefolio);
                    sl.SetCellValue(ifila, 3, (long)(cheque.numcheque ?? 0));
                    sl.SetCellValue(ifila, 4, FechaToString(cheque.fecha));
                    sl.SetCellValue(ifila, 5, FechaToString(cheque.salidarepartidor));
                    sl.SetCellValue(ifila, 6, FechaToString(cheque.arriborepartidor));
                    sl.SetCellValue(ifila, 7, FechaToString(cheque.cierre));
                    sl.SetCellValue(ifila, 8, cheque.mesa);
                    sl.SetCellValue(ifila, 9, (int)(cheque.nopersonas ?? 0));
                    sl.SetCellValue(ifila, 10, cheque.idmesero);
                    sl.SetCellValue(ifila, 11, BooleanToString(cheque.pagado));
                    sl.SetCellValue(ifila, 12, BooleanToString(cheque.cancelado));
                    sl.SetCellValue(ifila, 13, BooleanToString(cheque.impreso));
                    sl.SetCellValue(ifila, 14, (int)(cheque.impresiones ?? 0));
                    sl.SetCellValue(ifila, 15, (cheque.cambio ?? 0));
                    sl.SetCellValue(ifila, 16, (cheque.descuento ?? 0));
                    sl.SetCellValue(ifila, 17, (cheque.reabiertas ?? 0));
                    sl.SetCellValue(ifila, 18, cheque.razoncancelado);
                    sl.SetCellValue(ifila, 19, (cheque.orden ?? 0));
                    sl.SetCellValue(ifila, 20, BooleanToString(cheque.facturado));
                    sl.SetCellValue(ifila, 21, cheque.idcliente);
                    sl.SetCellValue(ifila, 22, cheque.idarearestaurant);
                    sl.SetCellValue(ifila, 23, cheque.claveempresav);
                    sl.SetCellValue(ifila, 24, (int)(cheque.tipodeservicio ?? 0));
                    sl.SetCellValue(ifila, 25, (long)(cheque.idturno ?? 0));
                    sl.SetCellValue(ifila, 26, cheque.usuariocancelo);
                    sl.SetCellValue(ifila, 27, cheque.comentariodescuento);
                    sl.SetCellValue(ifila, 28, cheque.estacion);
                    sl.SetCellValue(ifila, 29, (cheque.cambiorepartidor ?? 0));
                    sl.SetCellValue(ifila, 30, cheque.usuariodescuento);
                    sl.SetCellValue(ifila, 31, FechaToString(cheque.fechacancelado));
                    sl.SetCellValue(ifila, 32, cheque.idtipodescuento);
                    sl.SetCellValue(ifila, 33, cheque.numerotarjeta);
                    sl.SetCellValue(ifila, 34, (long)(cheque.folionotadeconsumo ?? 0));
                    sl.SetCellValue(ifila, 35, BooleanToString(cheque.notadeconsumo));
                    sl.SetCellValue(ifila, 36, BooleanToString(cheque.propinapagada));
                    sl.SetCellValue(ifila, 37, (long)(cheque.propinafoliomovtocaja ?? 0));
                    sl.SetCellValue(ifila, 38, (decimal)(cheque.puntosmonederogenerados ?? 0));
                    sl.SetCellValue(ifila, 39, (int)(cheque.propinaincluida ?? 0));
                    sl.SetCellValue(ifila, 40, cheque.tarjetadescuento);
                    sl.SetCellValue(ifila, 41, (decimal)(cheque.porcentajefac ?? 0));
                    sl.SetCellValue(ifila, 42, BooleanToString(cheque.propinamanual));
                    sl.SetCellValue(ifila, 43, cheque.usuariopago);
                    sl.SetCellValue(ifila, 44, cheque.idclientefacturacion);
                    sl.SetCellValue(ifila, 45, BooleanToString(cheque.cuentaenuso));
                    sl.SetCellValue(ifila, 46, cheque.observaciones);
                    sl.SetCellValue(ifila, 47, cheque.idclientedomicilio);
                    sl.SetCellValue(ifila, 48, cheque.iddireccion);
                    sl.SetCellValue(ifila, 49, cheque.telefonousadodomicilio);
                    sl.SetCellValue(ifila, 50, (decimal)(cheque.totalarticulos ?? 0));
                    sl.SetCellValue(ifila, 51, (decimal)(cheque.subtotal ?? 0));
                    sl.SetCellValue(ifila, 52, (decimal)(cheque.subtotalsinimpuestos ?? 0));
                    sl.SetCellValue(ifila, 53, (decimal)(cheque.total ?? 0));
                    sl.SetCellValue(ifila, 54, (decimal)(cheque.totalconpropina ?? 0));
                    sl.SetCellValue(ifila, 55, (decimal)(cheque.totalimpuesto1 ?? 0));
                    sl.SetCellValue(ifila, 56, (decimal)(cheque.cargo ?? 0));
                    sl.SetCellValue(ifila, 57, (decimal)(cheque.totalconcargo ?? 0));
                    sl.SetCellValue(ifila, 58, (decimal)(cheque.totalconpropinacargo ?? 0));
                    sl.SetCellValue(ifila, 59, (decimal)(cheque.descuentoimporte ?? 0));
                    sl.SetCellValue(ifila, 60, (decimal)(cheque.efectivo ?? 0));
                    sl.SetCellValue(ifila, 61, (decimal)(cheque.tarjeta ?? 0));
                    sl.SetCellValue(ifila, 62, (decimal)(cheque.vales ?? 0));
                    sl.SetCellValue(ifila, 63, (decimal)(cheque.otros ?? 0));
                    sl.SetCellValue(ifila, 64, (decimal)(cheque.propina ?? 0));
                    sl.SetCellValue(ifila, 65, (decimal)(cheque.propinatarjeta ?? 0));
                    sl.SetCellValue(ifila, 66, cheque.campoadicional1);
                    sl.SetCellValue(ifila, 67, cheque.idreservacion);
                    sl.SetCellValue(ifila, 68, cheque.idcomisionista);
                    sl.SetCellValue(ifila, 69, (decimal)(cheque.importecomision ?? 0));
                    sl.SetCellValue(ifila, 70, BooleanToString(cheque.comisionpagada));
                    sl.SetCellValue(ifila, 71, FechaToString(cheque.fechapagocomision));
                    sl.SetCellValue(ifila, 72, (int)(cheque.foliopagocomision ?? 0));
                    sl.SetCellValue(ifila, 73, (int)(cheque.tipoventarapida ?? 0));
                    sl.SetCellValue(ifila, 74, BooleanToString(cheque.callcenter));
                    sl.SetCellValue(ifila, 75, (long)(cheque.idordencompra ?? 0));
                    sl.SetCellValue(ifila, 76, cheque.idempresa);
                    sl.SetCellValue(ifila, 77, (decimal)(cheque.totalsindescuento ?? 0));
                    sl.SetCellValue(ifila, 78, (decimal)(cheque.totalalimentos ?? 0));
                    sl.SetCellValue(ifila, 79, (decimal)(cheque.totalbebidas ?? 0));
                    sl.SetCellValue(ifila, 80, (decimal)(cheque.totalotros ?? 0));
                    sl.SetCellValue(ifila, 81, (decimal)(cheque.totaldescuentos ?? 0));
                    sl.SetCellValue(ifila, 82, (decimal)(cheque.totaldescuentoalimentos ?? 0));
                    sl.SetCellValue(ifila, 83, (decimal)(cheque.totaldescuentobebidas ?? 0));
                    sl.SetCellValue(ifila, 84, (decimal)(cheque.totaldescuentootros ?? 0));
                    sl.SetCellValue(ifila, 85, (decimal)(cheque.totalcortesias ?? 0));
                    sl.SetCellValue(ifila, 86, (decimal)(cheque.totalcortesiaalimentos ?? 0));
                    sl.SetCellValue(ifila, 87, (decimal)(cheque.totalcortesiabebidas ?? 0));
                    sl.SetCellValue(ifila, 88, (decimal)(cheque.totalcortesiaotros ?? 0));
                    sl.SetCellValue(ifila, 89, (decimal)(cheque.totaldescuentoycortesia ?? 0));
                    sl.SetCellValue(ifila, 90, (decimal)(cheque.totalalimentossindescuentos ?? 0));
                    sl.SetCellValue(ifila, 91, (decimal)(cheque.totalbebidassindescuentos ?? 0));
                    sl.SetCellValue(ifila, 92, (decimal)(cheque.totalotrossindescuentos ?? 0));
                    sl.SetCellValue(ifila, 93, (decimal)(cheque.descuentocriterio ?? 0));
                    sl.SetCellValue(ifila, 94, (decimal)(cheque.descuentomonedero ?? 0));
                    sl.SetCellValue(ifila, 95, cheque.idmenucomedor);
                    sl.SetCellValue(ifila, 96, (decimal)(cheque.subtotalcondescuento ?? 0));
                    sl.SetCellValue(ifila, 97, (decimal)(cheque.comisionpax ?? 0));
                    sl.SetCellValue(ifila, 98, BooleanToString(cheque.procesadointerfaz));
                    sl.SetCellValue(ifila, 99, BooleanToString(cheque.domicilioprogramado));
                    sl.SetCellValue(ifila, 100, FechaToString(cheque.fechadomicilioprogramado));
                    sl.SetCellValue(ifila, 101, cheque.numerocuenta);
                    sl.SetCellValue(ifila, 102, cheque.codigo_unico_af);
                    sl.SetCellValue(ifila, 103, (int)(cheque.modificado ?? 0));
                    sl.SetCellValue(ifila, 104, BooleanToString(cheque.EnviadoRW));
                    sl.SetCellValue(ifila, 105, cheque.usuarioapertura);
                    sl.SetCellValue(ifila, 106, cheque.autorizacionfolio);
                    sl.SetCellValue(ifila, 107, FechaToString(cheque.fechalimiteemision));
                    sl.SetCellValue(ifila, 108, cheque.totalimpuestod1);
                    sl.SetCellValue(ifila, 109, cheque.totalimpuestod2);
                    sl.SetCellValue(ifila, 110, cheque.totalimpuestod3);
                    sl.SetCellValue(ifila, 111, cheque.idmotivocancela);
                    sl.SetCellValue(ifila, 112, cheque.titulartarjetamonedero);
                    sl.SetCellValue(ifila, 113, (decimal)(cheque.saldoanteriormonedero ?? 0));
                    sl.SetCellValue(ifila, 114, cheque.ncf);
                    sl.SetCellValue(ifila, 115, cheque.idformadepagoDescuento);
                    sl.SetCellValue(ifila, 116, cheque.titulartarjetamonederodescuento);
                    sl.SetCellValue(ifila, 117, cheque.surveycode);
                    sl.SetCellValue(ifila, 118, cheque.TKC_Authorization);
                    sl.SetCellValue(ifila, 119, cheque.TKC_Cupon);
                    sl.SetCellValue(ifila, 120, cheque.TKC_ExpirationDate);
                    sl.SetCellValue(ifila, 121, cheque.TKC_Recompensa);
                    sl.SetCellValue(ifila, 122, cheque.campoadicional2);
                    sl.SetCellValue(ifila, 123, cheque.campoadicional3);
                    sl.SetCellValue(ifila, 124, cheque.estrateca_CardNumber);
                    sl.SetCellValue(ifila, 125, cheque.estrateca_VoucherText);
                    sl.SetCellValue(ifila, 126, cheque.campoadicional4);
                    sl.SetCellValue(ifila, 127, cheque.campoadicional5);
                    sl.SetCellValue(ifila, 128, cheque.sacoa_CardNumber);
                    sl.SetCellValue(ifila, 129, cheque.sacoa_credits);
                    sl.SetCellValue(ifila, 130, cheque.estrateca_TypeDisccount);
                    sl.SetCellValue(ifila, 131, cheque.estrateca_DiscountCode);
                    sl.SetCellValue(ifila, 132, cheque.estrateca_DiscountID);
                    sl.SetCellValue(ifila, 133, cheque.estrateca_DiscountAmount);
                    sl.SetCellValue(ifila, 134, (decimal)(cheque.desc_imp_original ?? 0));
                    sl.SetCellValue(ifila, 135, cheque.donativo);
                    sl.SetCellValue(ifila, 136, cheque.totalcondonativo);
                    sl.SetCellValue(ifila, 137, cheque.totalconpropinacargodonativo);
                    sl.SetCellValue(ifila, 138, cheque.orderreference);
                    sl.SetCellValue(ifila, 139, cheque.appname);
                    sl.SetCellValue(ifila, 140, cheque.paymentproviderid);
                    sl.SetCellValue(ifila, 141, cheque.paymentprovider);
                    sl.SetCellValue(ifila, 142, BooleanToString(cheque.ChangeStatusSRX));
                    //sl.SetCellValue(ifila, 143, cheque.DateDownload);
                    //sl.SetCellValue(ifila, 144, cheque.empaquetado);
                    //sl.SetCellValue(ifila, 145, cheque.status_domicilio);
                    //sl.SetCellValue(ifila, 146, cheque.asignacion);
                    //sl.SetCellValue(ifila, 147, cheque.enviopagado);
                    //sl.SetCellValue(ifila, 148, cheque.sl_cupon_descuento);
                    //sl.SetCellValue(ifila, 149, cheque.sl_tipo_cupon);
                    //sl.SetCellValue(ifila, 150, cheque.sl_importe_descuento);
                    //sl.SetCellValue(ifila, 151, cheque.TUKI_CardNumber);
                    //sl.SetCellValue(ifila, 152, cheque.TUKI_AccumulatedPoints);
                    //sl.SetCellValue(ifila, 153, cheque.TUKI_CurrentPoints);
                    //sl.SetCellValue(ifila, 154, cheque.sl_num_cupones);
                }

                string pathArchivo = "";

                string textoFecha = "";

                if (fechaInicial.Date == fechaFinal.Date)
                    textoFecha = fechaInicial.ToString("yyyy-MM-dd");
                else
                    textoFecha = fechaInicial.ToString("yyyy-MM-dd") + "_al_" + fechaFinal.ToString("yyyy-MM-dd");

                if (tipoReporte == TipoReporte.DETALLADO_VERTICAL)
                {
                    pathArchivo = Path.Combine(PathDetalladoVertical, $"corte_de_cajax_{textoFecha}.xlsx");
                }
                else if (tipoReporte == TipoReporte.DETALLADO_HORIZONTAL)
                {
                    pathArchivo = Path.Combine(PathDetalladoHorizontal, $"corte_de_cajax_{textoFecha}.xlsx");
                }
                    
                sl.SaveAs(pathArchivo);

                Process.Start(pathArchivo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        private string BooleanToString(bool? valor)
        {
            return (valor ?? false) ? "VERDADERO" : "FALSO";
        }

        private string BooleanToString(bool valor)
        {
            return valor ? "VERDADERO" : "FALSO";
        }

        private string FechaToString(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy HH:mm") : "  -   -  : :";
        }
    }
}
