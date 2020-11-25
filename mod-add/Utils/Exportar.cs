using mod_add.Enums;
using SpreadsheetLight;
using SpreadsheetLight.Charts;
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
        private SLDocument SLDoc { get; set; }
        public Exportar()
        {
            PathBitacora = ConfiguracionLocalServicio.ReadSetting("PATH-BITACORA");
            PathDetalladoVertical = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-VERTICAL");
            PathDetalladoHorizontal = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-HORIZONTAL");

            SLDoc = new SLDocument();
        }

        public TipoRespuesta BicatoraExcel(List<SR_bitacorafiscal> bitacora)
        {
            if (bitacora.Count == 0) return TipoRespuesta.SIN_REGISTROS;
            DateTime fecha = DateTime.Now;
            string fechaString = fecha.ToString("yyyyMMddHHmmss");
            string pathArchivo = Path.Combine("PathBitacora", $"bitacora-{fechaString}.xlsx");

            try
            {
                //SLStyle styleColumn = SLDoc.CreateStyle();
                //styleColumn.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
                //styleColumn.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
                //styleColumn.SetWrapText(true);

                //SLStyle styleColumn2 = SLDoc.CreateStyle();
                //styleColumn2.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right);
                //styleColumn2.SetWrapText(true);

                //SLDoc.SetColumnStyle(1, styleColumn);
                //SLDoc.SetColumnStyle(2, styleColumn);
                //SLDoc.SetColumnStyle(3, styleColumn);
                //SLDoc.SetColumnStyle(4, styleColumn);
                //SLDoc.SetColumnStyle(5, styleColumn);
                //SLDoc.SetColumnStyle(9, styleColumn);

                //styleColumn.FormatCode = "#,##0.00";
                //SLDoc.SetColumnStyle(8, styleColumn);

                //styleColumn2.FormatCode = "#,##0.0000";
                //SLDoc.SetColumnStyle(6, styleColumn2);
                //SLDoc.SetColumnStyle(7, styleColumn2);

                //SLDoc.SetCellStyle(1, 6, styleColumn);
                //SLDoc.SetCellStyle(1, 7, styleColumn);

                SLDoc.SetColumnWidth(1, 24);
                SLDoc.SetColumnWidth(2, 24);
                SLDoc.SetColumnWidth(3, 24);
                SLDoc.SetColumnWidth(4, 15);
                SLDoc.SetColumnWidth(5, 15);
                SLDoc.SetColumnWidth(6, 15);
                SLDoc.SetColumnWidth(7, 15);
                SLDoc.SetColumnWidth(8, 15);
                SLDoc.SetColumnWidth(9, 15);

                SLDoc.SetCellValue(1, 1, "FECHA DE PROCESO");
                SLDoc.SetCellValue(1, 2, "FECHA INICIAL (VENTA)");
                SLDoc.SetCellValue(1, 3, "FECHA FINAL (VENTA)");
                SLDoc.SetCellValue(1, 4, $"TOTAL CUENTAS");
                SLDoc.SetCellValue(1, 5, "CUENTAS MODIFICADAS");
                SLDoc.SetCellValue(1, 6, "IMPORTE ANTERIOR");
                SLDoc.SetCellValue(1, 7, "IMPORTE NUEVO");
                SLDoc.SetCellValue(1, 8, "DIFERENCIA %");
                SLDoc.SetCellValue(1, 9, "TIPO DE ELIMINACION");
                //sl.SetCellValue(1, 10, "MODO DE ELIMINACION");

                SLStyle styleDateTime = SLDoc.CreateStyle();
                styleDateTime.FormatCode = "dd/MM/yyyy HH:mm";

                int i = 1;
                foreach (var resgitro in bitacora)
                {
                    i++;
                    SetDateTime(i, 1, resgitro.fecha, styleDateTime);
                    SetDateTime(i, 2, resgitro.fechainicial, styleDateTime);
                    SetDateTime(i, 3, resgitro.fechafinal, styleDateTime);
                    SLDoc.SetCellValue(i, 4, resgitro.cuentastotal ?? 0);
                    SLDoc.SetCellValue(i, 5, resgitro.cuentasmodificadas ?? 0);
                    SLDoc.SetCellValue(i, 6, resgitro.importeanterior ?? 0);
                    SLDoc.SetCellValue(i, 7, resgitro.importenuevo ?? 0);
                    SLDoc.SetCellValue(i, 8, resgitro.diferencia ?? 0);
                    SLDoc.SetCellValue(i, 9, resgitro.tipo);
                    //SLDoc.SetCellValue(i, 10, resgitro);
                }
                SLDoc.SaveAs(pathArchivo);

                return TipoRespuesta.HECHO;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                return TipoRespuesta.ERROR;
            }
        }

        public void DetallesCuentas(TipoReporte tipoReporte, TipoCorte tipoCorte, DateTime fechaInicial, DateTime fechaFinal, List<SR_cheques> cheques)
        {
            try
            {
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
                    "enviadorw",
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
                    "idformadepagodescuento",
                    "titulartarjetamonederodescuento",
                    "surveycode",
                    "tkc_authorization",
                    "tkc_cupon",
                    "tkc_expirationdate",
                    "tkc_recompensa",
                    "campoadicional2",
                    "campoadicional3",
                    "estrateca_cardnumber",
                    "campoadicional4",
                    "campoadicional5",
                    "sacoa_cardnumber",
                    "sacoa_credits",
                    "estrateca_typedisccount",
                    "estrateca_discountcode",
                    "estrateca_discountid",
                    "estrateca_discountamount",
                    "desc_imp_original",
                    "donativo",
                    "totalcondonativo",
                    "totalconpropinacargodonativo",
                    "appname",
                    "paymentproviderid",
                    "paymentprovider",
                    "changestatussrx",
                    "datedownload",
                    "empaquetado",
                    "status_domicilio",
                    "asignacion",
                    "enviopagado",
                    "sl_cupon_descuento",
                    "sl_tipo_cupon",
                    "sl_importe_descuento",
                    "tuki_cardnumber",
                    "tuki_accumulatedpoints",
                    "tuki_currentpoints",
                    "sl_num_cupones",
                    "totalimportedescuentos",
                };

                int icolumna = 0;
                foreach (var columna in columnas)
                {
                    icolumna++;
                    SLDoc.SetCellValue(1, icolumna, columna);
                }

                int ifila = 1;

                SLStyle styleDateTime = SLDoc.CreateStyle();
                styleDateTime.FormatCode = "dd/MM/yyyy HH:mm";

                foreach (var cheque in cheques)
                {
                    ifila++;

                    SLDoc.SetCellValue(ifila, 1, cheque.folio);
                    SLDoc.SetCellValue(ifila, 2, cheque.seriefolio);
                    SLDoc.SetCellValue(ifila, 3, (cheque.numcheque ?? 0));
                    SetDateTime(ifila, 4, cheque.fecha, styleDateTime);
                    SetDateTime(ifila, 5, cheque.salidarepartidor, styleDateTime);
                    SetDateTime(ifila, 6, cheque.arriborepartidor, styleDateTime);
                    SetDateTime(ifila, 7, cheque.cierre, styleDateTime);
                    SLDoc.SetCellValue(ifila, 8, cheque.mesa);
                    SLDoc.SetCellValue(ifila, 9, (cheque.nopersonas ?? 0));
                    SLDoc.SetCellValue(ifila, 10, cheque.idmesero);
                    SetBoolean(ifila, 11, cheque.pagado);
                    SetBoolean(ifila, 12, cheque.cancelado);
                    SetBoolean(ifila, 13, cheque.impreso);
                    SLDoc.SetCellValue(ifila, 14, (cheque.impresiones ?? 0));
                    SLDoc.SetCellValue(ifila, 15, (cheque.cambio ?? 0));
                    SLDoc.SetCellValue(ifila, 16, (cheque.descuento ?? 0));
                    SLDoc.SetCellValue(ifila, 17, (cheque.reabiertas ?? 0));
                    SLDoc.SetCellValue(ifila, 18, cheque.razoncancelado);
                    SLDoc.SetCellValue(ifila, 19, (cheque.orden ?? 0));
                    SetBoolean(ifila, 20, cheque.facturado);
                    SLDoc.SetCellValue(ifila, 21, cheque.idcliente);
                    SLDoc.SetCellValue(ifila, 22, cheque.idarearestaurant);
                    SLDoc.SetCellValue(ifila, 23, cheque.idempresa);
                    SLDoc.SetCellValue(ifila, 24, (cheque.tipodeservicio ?? 0));
                    SLDoc.SetCellValue(ifila, 25, (cheque.idturno ?? 0));
                    SLDoc.SetCellValue(ifila, 26, cheque.usuariocancelo);
                    SLDoc.SetCellValue(ifila, 27, cheque.comentariodescuento);
                    SLDoc.SetCellValue(ifila, 28, cheque.estacion);
                    SLDoc.SetCellValue(ifila, 29, (cheque.cambiorepartidor ?? 0));
                    SLDoc.SetCellValue(ifila, 30, cheque.usuariodescuento);
                    SetDateTime(ifila, 31, cheque.fechacancelado, styleDateTime);
                    SLDoc.SetCellValue(ifila, 32, cheque.idtipodescuento);
                    SLDoc.SetCellValue(ifila, 33, cheque.numerotarjeta);
                    SLDoc.SetCellValue(ifila, 34, (cheque.folionotadeconsumo ?? 0));
                    SetBoolean(ifila, 35, cheque.notadeconsumo);
                    SetBoolean(ifila, 36, cheque.propinapagada);
                    SLDoc.SetCellValue(ifila, 37, (cheque.propinafoliomovtocaja ?? 0));
                    SLDoc.SetCellValue(ifila, 38, (cheque.puntosmonederogenerados ?? 0));
                    SLDoc.SetCellValue(ifila, 39, (cheque.propinaincluida ?? 0));
                    SLDoc.SetCellValue(ifila, 40, cheque.tarjetadescuento);
                    SLDoc.SetCellValue(ifila, 41, (cheque.porcentajefac ?? 0));
                    SLDoc.SetCellValue(ifila, 42, cheque.usuariopago);
                    SetBoolean(ifila, 43, cheque.propinamanual);
                    SLDoc.SetCellValue(ifila, 44, cheque.observaciones);
                    SLDoc.SetCellValue(ifila, 45, cheque.idclientedomicilio);
                    SLDoc.SetCellValue(ifila, 46, cheque.iddireccion);
                    SLDoc.SetCellValue(ifila, 47, cheque.idclientefacturacion);
                    SLDoc.SetCellValue(ifila, 48, cheque.telefonousadodomicilio);
                    SLDoc.SetCellValue(ifila, 49, (cheque.totalarticulos ?? 0));
                    SLDoc.SetCellValue(ifila, 50, (cheque.subtotal ?? 0));
                    SLDoc.SetCellValue(ifila, 51, (cheque.subtotalsinimpuestos ?? 0));
                    SLDoc.SetCellValue(ifila, 52, (cheque.total ?? 0));
                    SLDoc.SetCellValue(ifila, 53, (cheque.totalconpropina ?? 0));
                    SLDoc.SetCellValue(ifila, 54, (cheque.totalimpuesto1 ?? 0));
                    SLDoc.SetCellValue(ifila, 55, (cheque.cargo ?? 0));
                    SLDoc.SetCellValue(ifila, 56, (cheque.totalconcargo ?? 0));
                    SLDoc.SetCellValue(ifila, 57, (cheque.totalconpropinacargo ?? 0));
                    SLDoc.SetCellValue(ifila, 58, (cheque.descuentoimporte ?? 0));
                    SLDoc.SetCellValue(ifila, 59, (cheque.efectivo ?? 0));
                    SLDoc.SetCellValue(ifila, 60, (cheque.tarjeta ?? 0));
                    SLDoc.SetCellValue(ifila, 61, (cheque.vales ?? 0));
                    SLDoc.SetCellValue(ifila, 62, (cheque.otros ?? 0));
                    SLDoc.SetCellValue(ifila, 63, (cheque.propina ?? 0));
                    SLDoc.SetCellValue(ifila, 64, (cheque.propinatarjeta ?? 0));
                    SLDoc.SetCellValue(ifila, 65, cheque.campoadicional1);
                    SLDoc.SetCellValue(ifila, 66, cheque.idreservacion);
                    SLDoc.SetCellValue(ifila, 67, cheque.idcomisionista);
                    SLDoc.SetCellValue(ifila, 68, (cheque.importecomision ?? 0));
                    SetBoolean(ifila, 69, cheque.comisionpagada);
                    SetDateTime(ifila, 70, cheque.fechapagocomision, styleDateTime);
                    SLDoc.SetCellValue(ifila, 71, (cheque.foliopagocomision ?? 0));
                    SLDoc.SetCellValue(ifila, 72, (cheque.tipoventarapida ?? 0));
                    SetBoolean(ifila, 73, cheque.callcenter);
                    SLDoc.SetCellValue(ifila, 74, (cheque.idordencompra ?? 0));
                    SLDoc.SetCellValue(ifila, 75, (cheque.totalsindescuento ?? 0));
                    SLDoc.SetCellValue(ifila, 76, (cheque.totalalimentos ?? 0));
                    SLDoc.SetCellValue(ifila, 77, (cheque.totalbebidas ?? 0));
                    SLDoc.SetCellValue(ifila, 78, (cheque.totalotros ?? 0));
                    SLDoc.SetCellValue(ifila, 79, (cheque.totaldescuentos ?? 0));
                    SLDoc.SetCellValue(ifila, 80, (cheque.totaldescuentoalimentos ?? 0));
                    SLDoc.SetCellValue(ifila, 81, (cheque.totaldescuentobebidas ?? 0));
                    SLDoc.SetCellValue(ifila, 82, (cheque.totaldescuentootros ?? 0));
                    SLDoc.SetCellValue(ifila, 83, (cheque.totalcortesias ?? 0));
                    SLDoc.SetCellValue(ifila, 84, (cheque.totalcortesiaalimentos ?? 0));
                    SLDoc.SetCellValue(ifila, 85, (cheque.totalcortesiabebidas ?? 0));
                    SLDoc.SetCellValue(ifila, 86, (cheque.totalcortesiaotros ?? 0));
                    SLDoc.SetCellValue(ifila, 87, (cheque.totaldescuentoycortesia ?? 0));
                    SLDoc.SetCellValue(ifila, 88, (cheque.totalalimentossindescuentos ?? 0));
                    SLDoc.SetCellValue(ifila, 89, (cheque.totalbebidassindescuentos ?? 0));
                    SLDoc.SetCellValue(ifila, 90, (cheque.totalotrossindescuentos ?? 0));
                    SLDoc.SetCellValue(ifila, 91, (cheque.descuentocriterio ?? 0));
                    SLDoc.SetCellValue(ifila, 92, (cheque.descuentomonedero ?? 0));
                    SLDoc.SetCellValue(ifila, 93, cheque.idmenucomedor);
                    SLDoc.SetCellValue(ifila, 94, (cheque.subtotalcondescuento ?? 0));
                    SLDoc.SetCellValue(ifila, 95, (cheque.comisionpax ?? 0));
                    SetBoolean(ifila, 96, cheque.procesadointerfaz);
                    SetBoolean(ifila, 97, cheque.domicilioprogramado);
                    SetDateTime(ifila, 98, cheque.fechadomicilioprogramado, styleDateTime);
                    SLDoc.SetCellValue(ifila, 99, cheque.ncf);
                    SLDoc.SetCellValue(ifila, 100, cheque.numerocuenta);
                    SLDoc.SetCellValue(ifila, 101, cheque.codigo_unico_af);
                    SetBoolean(ifila, 102, cheque.EnviadoRW);
                    SLDoc.SetCellValue(ifila, 103, cheque.usuarioapertura);
                    SLDoc.SetCellValue(ifila, 104, cheque.titulartarjetamonedero);
                    SLDoc.SetCellValue(ifila, 105, (cheque.saldoanteriormonedero ?? 0));
                    SLDoc.SetCellValue(ifila, 106, cheque.autorizacionfolio);
                    SetDateTime(ifila, 107, cheque.fechalimiteemision, styleDateTime);
                    SLDoc.SetCellValue(ifila, 108, cheque.totalimpuestod1);
                    SLDoc.SetCellValue(ifila, 109, cheque.totalimpuestod2);
                    SLDoc.SetCellValue(ifila, 110, cheque.totalimpuestod3);
                    SLDoc.SetCellValue(ifila, 111, cheque.idmotivocancela);
                    SLDoc.SetCellValue(ifila, 112, cheque.idformadepagoDescuento);
                    SLDoc.SetCellValue(ifila, 113, cheque.titulartarjetamonederodescuento);
                    SLDoc.SetCellValue(ifila, 114, cheque.surveycode);
                    SLDoc.SetCellValue(ifila, 115, cheque.TKC_Authorization);
                    SLDoc.SetCellValue(ifila, 116, cheque.TKC_Cupon);
                    SLDoc.SetCellValue(ifila, 117, cheque.TKC_ExpirationDate);
                    SLDoc.SetCellValue(ifila, 118, cheque.TKC_Recompensa);
                    SLDoc.SetCellValue(ifila, 119, cheque.campoadicional3);
                    SLDoc.SetCellValue(ifila, 120, cheque.estrateca_CardNumber);
                    SLDoc.SetCellValue(ifila, 121, cheque.campoadicional4);
                    SLDoc.SetCellValue(ifila, 122, cheque.campoadicional5);
                    SLDoc.SetCellValue(ifila, 123, cheque.sacoa_CardNumber);
                    SLDoc.SetCellValue(ifila, 124, cheque.sacoa_credits);
                    SLDoc.SetCellValue(ifila, 125, cheque.estrateca_TypeDisccount);
                    SLDoc.SetCellValue(ifila, 126, cheque.estrateca_DiscountCode);
                    SLDoc.SetCellValue(ifila, 127, cheque.estrateca_DiscountID);
                    SLDoc.SetCellValue(ifila, 128, cheque.estrateca_DiscountAmount);
                    SLDoc.SetCellValue(ifila, 129, (cheque.desc_imp_original ?? 0));
                    SLDoc.SetCellValue(ifila, 130, cheque.donativo);
                    SLDoc.SetCellValue(ifila, 131, cheque.totalcondonativo);
                    SLDoc.SetCellValue(ifila, 132, cheque.totalconpropinacargodonativo);
                    SLDoc.SetCellValue(ifila, 133, cheque.appname);
                    SLDoc.SetCellValue(ifila, 134, cheque.paymentproviderid);
                    SLDoc.SetCellValue(ifila, 135, cheque.paymentprovider);
                    SetBoolean(ifila, 136, cheque.ChangeStatusSRX);
                    SLDoc.SetCellValue(ifila, 137, cheque.claveempresav);
                    SetBoolean(ifila, 138, cheque.cuentaenuso);
                    SLDoc.SetCellValue(ifila, 139, (cheque.modificado ?? 0));
                    SLDoc.SetCellValue(ifila, 140, cheque.campoadicional2);
                    SetDateTime(ifila, 141, cheque.DateDownload, styleDateTime);
                    SetDateTime(ifila, 142, cheque.empaquetado, styleDateTime);
                    SLDoc.SetCellValue(ifila, 143, (cheque.status_domicilio ?? 0));
                    SetDateTime(ifila, 144, cheque.asignacion, styleDateTime);
                    SetBoolean(ifila, 145, cheque.enviopagado);
                    SLDoc.SetCellValue(ifila, 146, cheque.sl_cupon_descuento);
                    SLDoc.SetCellValue(ifila, 147, cheque.sl_tipo_cupon);
                    SLDoc.SetCellValue(ifila, 148, (cheque.sl_importe_descuento ?? 0));
                    SLDoc.SetCellValue(ifila, 149, cheque.TUKI_CardNumber);
                    SLDoc.SetCellValue(ifila, 150, (cheque.TUKI_AccumulatedPoints ?? 0));
                    SLDoc.SetCellValue(ifila, 151, (cheque.TUKI_CurrentPoints ?? 0));
                    SLDoc.SetCellValue(ifila, 152, (cheque.sl_num_cupones ?? 0));
                    SLDoc.SetCellValue(ifila, 153, (cheque.totalsindescuento ?? 0));
                }

                string path = "";
                string textoFecha = "";

                if (tipoCorte == TipoCorte.TURNO)
                    textoFecha = fechaInicial.ToString("yyyy-MM-dd");
                else if (tipoCorte == TipoCorte.PERIODO)
                    textoFecha = fechaInicial.ToString("yyyy-MM-dd") + "_al_" + fechaFinal.ToString("yyyy-MM-dd");

                if (tipoReporte == TipoReporte.DETALLADO_VERTICAL)
                    path = PathDetalladoVertical;
                else if (tipoReporte == TipoReporte.DETALLADO_HORIZONTAL)
                    path = PathDetalladoHorizontal;

                string pathArchivo = Path.Combine(PathDetalladoVertical, $"corte_de_cajax_{textoFecha}.xlsx");

                SLDoc.SaveAs(path);

                Process.Start(path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        private void SetBoolean(int row, int column, bool? value, string textTrue = "VERDADERO", string textFalse = "FALSO")
        {
            SLDoc.SetCellValue(row, column, (value ?? false) ? textTrue : textFalse);
        }

        private void SetBoolean(int row, int column, bool value, string textTrue = "VERDADERO", string textFalse = "FALSO")
        {
            SLDoc.SetCellValue(row, column, value ? textTrue : textFalse);
        }

        private void SetDateTime(int row, int column, DateTime? datetime, SLStyle style, string textDefault = "  -   -  : :")
        {
            if (datetime.HasValue)
            {
                SLDoc.SetCellStyle(row, column, style);
                SLDoc.SetCellValue(row, column, datetime.Value);
            }
            else
            {
                SLDoc.SetCellValue(row, column, textDefault);
            }
        }

        private void SetDateTime(int row, int column, DateTime datetime, SLStyle style)
        {
            SLDoc.SetCellStyle(row, column, style);
            SLDoc.SetCellValue(row, column, datetime);
        }
    }
}
