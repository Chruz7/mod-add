using HandlebarsDotNet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using mod_add.Enums;
using mod_add.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace mod_add.Utils
{
    public class GenerarReporte
    {
        public string PathEjecuccion { get; set; }
        public string PathDetalladoVertical { get; set; }
        public string PathDetalladoHorizontal { get; set; }
        public string PathDetalladoFormasPago { get; set; }

        public string NombreComercial { get; set; }
        public string RazonSocial { get; set; }
        public string DireccionFiscal { get; set; }
        public string CodigoPostal { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string RFC { get; set; }
        public string Telefono { get; set; }
        public string DireccionSucursal { get; set; }
        public string CiudadSucursal { get; set; }
        public string EstadoSucursal { get; set; }

        private readonly SRLibrary.Utils.Print Impresion;

        public GenerarReporte()
        {
            Impresion = new SRLibrary.Utils.Print();
            PathEjecuccion = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            PathDetalladoVertical = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-VERTICAL");
            PathDetalladoHorizontal = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-HORIZONTAL");
            PathDetalladoFormasPago = ConfiguracionLocalServicio.ReadSetting("PATH-DETALLADO-FORMAS-PAGO");

            NombreComercial = ConfiguracionLocalServicio.ReadSetting("NOMBRE-COMERCIAL");
            RazonSocial = ConfiguracionLocalServicio.ReadSetting("RAZON-SOCIAL");
            DireccionFiscal = ConfiguracionLocalServicio.ReadSetting("DIRECCION-FISCAL");
            CodigoPostal = ConfiguracionLocalServicio.ReadSetting("CODIGO-POSTAL");
            Ciudad = ConfiguracionLocalServicio.ReadSetting("CIUDAD");
            Estado = ConfiguracionLocalServicio.ReadSetting("ESTADO");
            Pais = ConfiguracionLocalServicio.ReadSetting("PAIS");
            RFC = ConfiguracionLocalServicio.ReadSetting("RFC");
            Telefono = ConfiguracionLocalServicio.ReadSetting("TELEFONO");
            DireccionSucursal = ConfiguracionLocalServicio.ReadSetting("DIRECCION-SUCURSAL");
            CiudadSucursal = ConfiguracionLocalServicio.ReadSetting("CIUDAD-SUCURSAL");
            EstadoSucursal = ConfiguracionLocalServicio.ReadSetting("ESTADO-SUCURSAL");
        }

        public void DetalladoFormasPago(ReporteCorte reporte, TipoDestino tipoDestino)
        {
            try
            {
                DateTime fecha = DateTime.Now;

                var rutaHtml = Path.Combine(PathEjecuccion, @".\plantillas\corte-detallado-formas-de-pago.html");
                var rutaCss = Path.Combine(PathEjecuccion, @".\plantillas\estilos.css");

                string html = @File.ReadAllText(rutaHtml);
                string css = @File.ReadAllText(rutaCss);

                #region Cheques
                string source = "{{#CHEQUESREPORTE}}<tr class=\"fila\">{{>CHEQUEREPORTE}}</tr>{{/CHEQUESREPORTE}}";

                string partialSource =
                    "<td class=\"info text-center\">{{Snumcheque}}</td> " +
                    "<td class=\"info text-center\">{{Sfecha}}</td>" +
                    "<td class=\"info text-center\">{{Simpresiones}}</td>" +
                    "<td class=\"info text-center\">{{Sreabiertas}}</td>" +
                    "<td class=\"info text-center\">{{Sdescuento}}</td>" +
                    "<td class=\"info text-right\">{{Spropina}}</td>" +
                    "<td class=\"info text-right\">{{Simporte}}</td>" +
                    "<td class=\"info text-right\">{{Scargo}}</td>" +
                    "<td class=\"info text-right\">{{Sefectivo}}</td>" +
                    "<td class=\"info text-right\">{{Starjeta}}</td>" +
                    "<td class=\"info text-right\">{{Svales}}</td>" +
                    "<td class=\"info text-right\">{{Sotros}}</td>";

                Handlebars.RegisterTemplate("CHEQUEREPORTE", partialSource);

                var template1 = Handlebars.Compile(source);

                var data1 = new
                {
                    reporte.ChequesReporte
                };

                var chequesReporteTemplate = template1(data1);
                #endregion

                #region Pago ventas
                source = "{{#PAGOS}}<tr class=\"fila\">{{>PAGO}}</tr>{{/PAGOS}}";
                partialSource =
                    "<td>{{descripcion}}</td>" +
                    "<td>{{SImporte}}</td>";

                Handlebars.RegisterTemplate("PAGO", partialSource);

                var template2 = Handlebars.Compile(source);

                var data2 = new
                {
                    reporte.Pagos
                };

                var pagoVentasTemplate = template2(data2);
                #endregion

                #region Pago propinas
                var pagoPropinas = reporte.Pagos.Where(x => x.propina > 0).ToList();

                source = "{{#PAGOPROPINAS}}<tr class=\"fila\">{{> PAGOPROPINA}}</tr>{{/PAGOPROPINAS}}";
                partialSource =
                    "<td>{{descripcion}}</td>" +
                    "<td>{{SPropina}}</td>";

                Handlebars.RegisterTemplate("PAGOPROPINA", partialSource);

                var template3 = Handlebars.Compile(source);

                var data3 = new
                {
                    pagoPropinas
                };

                var pagoPropinasTemplate = template3(data3);
                #endregion

                #region Turnos
                source = "{{#TURNOS}}<tr class=\"fila\">{{> TURNO}}</tr>{{/TURNOS}}";
                partialSource =
                    "<td>{{idestacion}}</td>" +
                    "<td>{{Sapertura}}</td>" +
                    "<td>{{Scierre}}</td>" +
                    "<td>{{STotal}}</td>" +
                    "<td>{{SCargo}}</td>" +
                    "<td>{{Sefectivo}}</td>" +
                    "<td>{{Starjeta}}</td>" +
                    "<td>{{Svales}}</td>" +
                    "<td>{{SPropina}}</td>" +
                    "<td>{{Sotros}}</td>";

                Handlebars.RegisterTemplate("TURNO", partialSource);

                var template4 = Handlebars.Compile(source);

                var data4 = new
                {
                    reporte.Turnos
                };

                var turnosTemplate = template4(data4);
                #endregion

                #region Ventas Rapidas
                source = "{{#VENTASRAPIDAS}}<tr class=\"fila\">{{> VENTARAPIDA}}</tr>{{/VENTASRAPIDAS}}";
                partialSource =
                    "<td>{{Descripcion}}</td>" +
                    "<td class\"text-right\">{{STotal}}</td>";

                Handlebars.RegisterTemplate("VENTARAPIDA", partialSource);

                var template5 = Handlebars.Compile(source);

                var data5 = new
                {
                    reporte.VentasRapidas
                };

                var ventasrapidasTemplate = template5(data5);
                #endregion

                #region Totales cheques
                source = "{{#TOTALESCHEQUESREPORTE}}<tr class=\"fila\">{{> TOTALCHEQUEREPORTE}}</tr>{{/TOTALESCHEQUESREPORTE}}";
                partialSource =
                    "<td></td> " +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td>{{Spropina}}</td>" +
                    "<td>{{Simporte}}</td>" +
                    "<td>{{Scargo}}</td>" +
                    "<td>{{Sefectivo}}</td>" +
                    "<td>{{Starjeta}}</td>" +
                    "<td>{{Svales}}</td>" +
                    "<td>{{Scredito}}</td>";

                Handlebars.RegisterTemplate("TOTALCHEQUEREPORTE", partialSource);

                var template6 = Handlebars.Compile(source);

                var data6 = new
                {
                    reporte.TotalesChequesReporte
                };

                var totaleschequesreporteTemplate = template6(data6);
                #endregion

                var HtmlTemplate = html;
                var HtmlInstance = HtmlTemplate
                    .Replace("[[FECHA]]", fecha.ToShortDateString()) //HEADER PAGINA
                    .Replace("[[HORA]]", fecha.ToString("hh:mm:ss tt").ToUpper())
                    .Replace("[[NOMBRE-COMERCIAL]]", NombreComercial)
                    .Replace("[[RAZON-SOCIAL]]", RazonSocial)
                    .Replace("[[RFC]]", RFC)
                    .Replace("[[DIRECCION-FISCAL]]", DireccionFiscal)
                    .Replace("[[CIUDAD]]", Ciudad)
                    .Replace("[[ESTADO]]", Estado)
                    .Replace("[[PAIS]]", Pais)
                    .Replace("[[CODIGO-POSTAL]]", CodigoPostal)
                    .Replace("[[TELEFONO]]", Telefono)
                    .Replace("[[DIRECCION-SUCURSAL]]", DireccionSucursal)
                    .Replace("[[CIUDAD-SUCURSAL]]", CiudadSucursal)
                    .Replace("[[ESTADO-SUCURSAL]]", EstadoSucursal)
                    .Replace("[[TITULO-CORTE-Z]]", reporte.TituloCorte)
                    .Replace("[[FECHA-CORTE-INICIO]]", reporte.FechaCorteInicio.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    .Replace("[[FECHA-CORTE-CIERRE]]", reporte.FechaCorteCierre.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    .Replace("[[FOLIO-CORTE]]", reporte.FolioCorte.ToString())

                    .Replace("[[CHEQUESREPORTE]]", chequesReporteTemplate) // CUENTAS

                    .Replace("[[TOTALESCHEQUESREPORTE]]", totaleschequesreporteTemplate) // TOTALES CUENTAS

                    .Replace($"[[{nameof(reporte.CuentasNormales).ToUpper()}]]", $"{reporte.CuentasNormales}") //CUENTAS
                    .Replace($"[[{nameof(reporte.CuentasCanceladas).ToUpper()}]]", $"{reporte.CuentasCanceladas}")
                    .Replace($"[[{nameof(reporte.CuentasConDescuento).ToUpper()}]]", $"{reporte.CuentasConDescuento}")
                    .Replace($"[[{nameof(reporte.CuentasConDescuentoImporte).ToUpper()}]]", string.Format("{0:C}", reporte.CuentasConDescuentoImporte))
                    .Replace($"[[{nameof(reporte.CuentasConCortesia).ToUpper()}]]", $"{reporte.CuentasConCortesia}")
                    .Replace($"[[{nameof(reporte.CuentasConCortesiaImporte).ToUpper()}]]", string.Format("{0:C}", reporte.CuentasConCortesiaImporte))
                    .Replace($"[[{nameof(reporte.CuentaPromedio).ToUpper()}]]", string.Format("{0:C}", reporte.CuentaPromedio))
                    .Replace($"[[{nameof(reporte.Comensales).ToUpper()}]]", $"{reporte.Comensales}")
                    .Replace($"[[{nameof(reporte.ConsumoPromedio).ToUpper()}]]", string.Format("{0:C}", reporte.ConsumoPromedio))
                    .Replace($"[[{nameof(reporte.Propinas).ToUpper()}]]", string.Format("{0:C}", reporte.Propinas))
                    .Replace($"[[{nameof(reporte.Cargos).ToUpper()}]]", string.Format("{0:C}", reporte.Cargos))
                    .Replace($"[[{nameof(reporte.DescuentoMonedero).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoMonedero))
                    .Replace($"[[{nameof(reporte.FolioInicial).ToUpper()}]]", $"{reporte.FolioInicial}")
                    .Replace($"[[{nameof(reporte.FolioFinal).ToUpper()}]]", $"{reporte.FolioFinal}")

                    .Replace("[[TOTALPROPINA]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.propina))) //TOTALES EN CUENTAS
                    .Replace("[[TOTALIMPORTE]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.total)))
                    .Replace("[[TOTALCARGO]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.cargo)))
                    .Replace("[[TOTALEFECTIVO]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.efectivo)))
                    .Replace("[[TOTALTARJETA]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.tarjeta)))
                    .Replace("[[TOTALVALES]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.vales)))
                    .Replace("[[TOTALOTROS]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.otros)))

                    .Replace($"[[{nameof(reporte.VentaFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.VentaFacturada)) // FACTURAS
                    .Replace($"[[{nameof(reporte.PropinaFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.PropinaFacturada))
                    .Replace($"[[{nameof(reporte.Facturado).ToUpper()}]]", string.Format("{0:C}", reporte.Facturado))
                    .Replace($"[[{nameof(reporte.VentaNoFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.VentaNoFacturada))

                    .Replace($"[[{nameof(reporte.EfectivoInicial).ToUpper()}]]", string.Format("{0:C}", reporte.EfectivoInicial)) // CAJA
                    .Replace($"[[{nameof(reporte.Efectivo).ToUpper()}]]", string.Format("{0:C}", reporte.Efectivo))
                    .Replace($"[[{nameof(reporte.Tarjeta).ToUpper()}]]", string.Format("{0:C}", reporte.Tarjeta))
                    .Replace($"[[{nameof(reporte.Vales).ToUpper()}]]", string.Format("{0:C}", reporte.Vales))
                    .Replace($"[[{nameof(reporte.Otros).ToUpper()}]]", string.Format("{0:C}", reporte.Otros))
                    .Replace($"[[{nameof(reporte.DepositosEfectivo).ToUpper()}]]", string.Format("{0:C}", reporte.DepositosEfectivo))
                    .Replace($"[[{nameof(reporte.RetirosEfectivo).ToUpper()}]]", string.Format("{0:C}", reporte.RetirosEfectivo))
                    .Replace($"[[{nameof(reporte.PropinasPagadas).ToUpper()}]]", string.Format("{0:C}", reporte.PropinasPagadas))
                    .Replace($"[[{nameof(reporte.SaldoFinal).ToUpper()}]]", string.Format("{0:C}", reporte.SaldoFinal))
                    .Replace($"[[{nameof(reporte.Dolares).ToUpper()}]]", string.Format("{0:C}", reporte.Dolares))
                    .Replace($"[[{nameof(reporte.TotalDeclarado).ToUpper()}]]", string.Format("{0:C}", reporte.TotalDeclarado))
                    .Replace("[[SOBRANTE]]", reporte.SobranteOFaltante > 0 ? string.Format("{0:C}", reporte.SobranteOFaltante) : "")
                    .Replace("[[FALTANTE]]", reporte.SobranteOFaltante < 0 ? string.Format("{0:C}", reporte.SobranteOFaltante * -1) : "")

                    .Replace("[[PAGOS]]", pagoVentasTemplate) // FORMA DE PAGO VENTAS

                    .Replace("[[PAGOPROPINAS]]", pagoPropinasTemplate) // FORMA DE PAGO PROPINAS
                    .Replace($"[[{nameof(reporte.TotalFormasPagoPropinas).ToUpper()}]]", string.Format("{0:C}", reporte.TotalFormasPagoPropinas))

                    .Replace($"[[{nameof(reporte.CortesiaAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.CortesiaAlimentos)) // CORTESIAS Y DESCUENTOS
                    .Replace($"[[{nameof(reporte.CortesiaBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.CortesiaBebidas))
                    .Replace($"[[{nameof(reporte.CortesiaOtros).ToUpper()}]]", string.Format("{0:C}", reporte.CortesiaOtros))
                    .Replace($"[[{nameof(reporte.TotalCortesias).ToUpper()}]]", string.Format("{0:C}", reporte.TotalCortesias))
                    .Replace($"[[{nameof(reporte.DescuentoAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoAlimentos))
                    .Replace($"[[{nameof(reporte.DescuentoBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoBebidas))
                    .Replace($"[[{nameof(reporte.DescuentoOtros).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoOtros))
                    .Replace($"[[{nameof(reporte.TotalDescuentos).ToUpper()}]]", string.Format("{0:C}", reporte.TotalDescuentos))
                    .Replace($"[[{nameof(reporte.Descuentos).ToUpper()}]]", string.Format("{0:C}", reporte.Descuentos))

                    .Replace($"[[{nameof(reporte.PAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.PAlimentos)) //VANTAS POR TIPO DE PRODUCTO
                    .Replace($"[[{nameof(reporte.PPorcentajeAlimentos).ToUpper()}]]", $"{reporte.PPorcentajeAlimentos}")
                    .Replace($"[[{nameof(reporte.PBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.PBebidas))
                    .Replace($"[[{nameof(reporte.PPorcentajeBebidas).ToUpper()}]]", $"{reporte.PPorcentajeBebidas}")
                    .Replace($"[[{nameof(reporte.POtros).ToUpper()}]]", string.Format("{0:C}", reporte.POtros))
                    .Replace($"[[{nameof(reporte.PPorcentajeOtros).ToUpper()}]]", $"{reporte.PPorcentajeOtros}")

                    .Replace($"[[{nameof(reporte.Comedor).ToUpper()}]]", string.Format("{0:C}", reporte.Comedor)) //VENTAS POR TIPO DE SERVICIO
                    .Replace($"[[{nameof(reporte.ComedorPorcentaje).ToUpper()}]]", $"{reporte.ComedorPorcentaje}")
                    .Replace($"[[{nameof(reporte.Domicilio).ToUpper()}]]", string.Format("{0:C}", reporte.Domicilio))
                    .Replace($"[[{nameof(reporte.DomicilioPorcentaje).ToUpper()}]]", $"{reporte.DomicilioPorcentaje}")
                    .Replace($"[[{nameof(reporte.Rapido).ToUpper()}]]", string.Format("{0:C}", reporte.Rapido))
                    .Replace($"[[{nameof(reporte.RapidoPorcentaje).ToUpper()}]]", $"{reporte.RapidoPorcentaje}")

                    .Replace($"[[{nameof(reporte.Subtotal).ToUpper()}]]", string.Format("{0:C}", reporte.Subtotal))
                    .Replace($"[[{nameof(reporte.Descuentos).ToUpper()}]]", string.Format("{0:C}", reporte.Descuentos))
                    .Replace($"[[{nameof(reporte.VentaNeta).ToUpper()}]]", string.Format("{0:C}", reporte.VentaNeta))
                    .Replace($"[[{nameof(reporte.ImpuestoTotal).ToUpper()}]]", string.Format("{0:C}", reporte.ImpuestoTotal))
                    .Replace($"[[{nameof(reporte.VentasConImpuesto).ToUpper()}]]", string.Format("{0:C}", reporte.VentasConImpuesto))

                    .Replace("[[VENTASRAPIDAS]]", ventasrapidasTemplate) // TURNOS

                    .Replace("[[TURNOS]]", turnosTemplate) // TURNOS
                    ;

                StringReader sr = new StringReader(HtmlInstance.ToString());

                Document pdfDoc = new Document(PageSize.LETTER, 20f, 20f, 30f, 30f);
                pdfDoc.SetPageSize(PageSize.A4.Rotate());

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                    pdfDoc.Open();

                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css)))
                    {
                        using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(HtmlInstance)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
                        }
                    }

                    pdfDoc.Close();
                    var nombre_archivo = Path.Combine(PathDetalladoFormasPago, $"{reporte.FolioCorte}-detallado-formas-de-pago.pdf");
                    byte[] bytes = AddPageNumbers(memoryStream.ToArray(), false);
                    memoryStream.Close();
                    var fs = new FileStream(nombre_archivo, FileMode.Create, FileAccess.Write);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();

                    if (tipoDestino == TipoDestino.EXPORTAR)
                    {
                        Process.Start("file://" + nombre_archivo);
                    }
                    else if (tipoDestino == TipoDestino.IMPRESION)
                    {
                        Impresion.Print_File(nombre_archivo, "", 0, 8, 8);

                        //File.Delete(nombre_archivo);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void DetalladoHorizontal(ReporteCorte reporte, TipoDestino tipoDestino)
        {
            try
            {
                DateTime fecha = DateTime.Now;

                var rutaHtml = Path.Combine(PathEjecuccion, @".\plantillas\corte-detallado-horizontal.html");
                var rutaCss = Path.Combine(PathEjecuccion, @".\plantillas\estilos.css");

                string html = @File.ReadAllText(rutaHtml);
                string css = @File.ReadAllText(rutaCss);

                #region Cheques
                string source = "{{#CHEQUESREPORTE}}<tr class=\"fila\">{{>CHEQUEREPORTE}}</tr>{{/CHEQUESREPORTE}}";

                string partialSource =
                    "<td class=\"info text-center\">{{Snumcheque2}}</td>" +
                    "<td class=\"info text-center\">{{Sfolionotadeconsumo}}</td>" +
                    "<td class=\"info text-center\">{{Sfecha}}</td>" +
                    "<td class=\"info text-center\">{{Simpresiones}}</td>" +
                    "<td class=\"info text-center\">{{Sreabiertas}}</td>" +
                    "<td class=\"info\">{{mesa}}</td>" +
                    "<td class=\"info text-center\">{{idtipodescuento}}</td>" +
                    "<td class=\"info text-center\">{{Sdescuento}}</td>" +
                    "<td class=\"info text-center\">{{Stotaldescuentoycortesia}}</td>" +
                    "<td class=\"info text-right\">{{Spropina}}</td>" +
                    "<td class=\"info text-right\">{{Simporte}}</td>" +
                    "<td class=\"info text-right\">{{Scargo}}</td>" +
                    "<td class=\"info text-right\">{{Sefectivo}}</td>" +
                    "<td class=\"info text-right\">{{Starjeta}}</td>" +
                    "<td class=\"info text-right\">{{Svales}}</td>" +
                    "<td class=\"info text-right\">{{Sotros}}</td>";

                Handlebars.RegisterTemplate("CHEQUEREPORTE", partialSource);

                var template1 = Handlebars.Compile(source);

                var data1 = new
                {
                    reporte.ChequesReporte
                };

                var chequesReporteTemplate = template1(data1);
                #endregion

                #region Pago ventas
                source = "{{#PAGOS}}<tr class=\"fila\">{{>PAGO}}</tr>{{/PAGOS}}";
                partialSource =
                    "<td>{{descripcion}}</td>" +
                    "<td>{{SImporte}}</td>";

                Handlebars.RegisterTemplate("PAGO", partialSource);

                var template2 = Handlebars.Compile(source);

                var data2 = new
                {
                    reporte.Pagos
                };

                var pagoVentasTemplate = template2(data2);
                #endregion

                #region Pago propinas
                var pagoPropinas = reporte.Pagos.Where(x => x.propina > 0).ToList();

                source = "{{#PAGOPROPINAS}}<tr class=\"fila\">{{> PAGOPROPINA}}</tr>{{/PAGOPROPINAS}}";
                partialSource =
                    "<td>{{descripcion}}</td>" +
                    "<td>{{SPropina}}</td>";

                Handlebars.RegisterTemplate("PAGOPROPINA", partialSource);

                var template3 = Handlebars.Compile(source);

                var data3 = new
                {
                    pagoPropinas
                };

                var pagoPropinasTemplate = template3(data3);
                #endregion

                #region Turnos
                source = "{{#TURNOS}}<tr class=\"fila\">{{> TURNO}}</tr>{{/TURNOS}}";
                partialSource =
                    "<td>{{idestacion}}</td>" +
                    "<td>{{cajero}}</td>" +
                    "<td>{{Sapertura}}</td>" +
                    "<td>{{Scierre}}</td>" +
                    "<td>{{STotal}}</td>" +
                    "<td>{{SCargo}}</td>" +
                    "<td>{{Sefectivo}}</td>" +
                    "<td>{{Starjeta}}</td>" +
                    "<td>{{Svales}}</td>" +
                    "<td>{{SPropina}}</td>" +
                    "<td>{{Scredito}}</td>" +
                    "<td>{{idturno}}</td>";

                Handlebars.RegisterTemplate("TURNO", partialSource);

                var template4 = Handlebars.Compile(source);

                var data4 = new
                {
                    reporte.Turnos
                };

                var turnosTemplate = template4(data4);
                #endregion

                #region Ventas Rapidas
                source = "{{#VENTASRAPIDAS}}<tr class=\"fila\">{{> VENTARAPIDA}}</tr>{{/VENTASRAPIDAS}}";
                partialSource =
                    "<td>{{Descripcion}}</td>" +
                    "<td class\"text-right\">{{STotal}}</td>";

                Handlebars.RegisterTemplate("VENTARAPIDA", partialSource);

                var template5 = Handlebars.Compile(source);

                var data5 = new
                {
                    reporte.VentasRapidas
                };

                var ventasrapidasTemplate = template5(data5);
                #endregion

                #region Totales cheques
                source = "{{#TOTALESCHEQUESREPORTE}}<tr class=\"fila\">{{> TOTALCHEQUEREPORTE}}</tr>{{/TOTALESCHEQUESREPORTE}}";
                partialSource =
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td class=\"text-right\">{{Stotaldescuentoycortesia}}</td>" +
                    "<td class=\"text-right\">{{Spropina}}</td>" +
                    "<td class=\"text-right\">{{Simporte}}</td>" +
                    "<td class=\"text-right\">{{Scargo}}</td>" +
                    "<td class=\"text-right\">{{Sefectivo}}</td>" +
                    "<td class=\"text-right\">{{Starjeta}}</td>" +
                    "<td class=\"text-right\">{{Svales}}</td>" +
                    "<td class=\"text-right\">{{Sotros}}</td>";

                Handlebars.RegisterTemplate("TOTALCHEQUEREPORTE", partialSource);

                var template6 = Handlebars.Compile(source);

                var data6 = new
                {
                    reporte.TotalesChequesReporte
                };

                var totaleschequesreporteTemplate = template6(data6);
                #endregion

                var HtmlTemplate = html;
                var HtmlInstance = HtmlTemplate
                    .Replace("[[FECHA]]", fecha.ToShortDateString()) //HEADER PAGINA
                    .Replace("[[HORA]]", fecha.ToString("hh:mm:ss tt").ToUpper())
                    .Replace("[[NOMBRE-COMERCIAL]]", NombreComercial)
                    .Replace("[[RAZON-SOCIAL]]", RazonSocial)
                    .Replace("[[RFC]]", RFC)
                    .Replace("[[DIRECCION-FISCAL]]", DireccionFiscal)
                    .Replace("[[CIUDAD]]", Ciudad)
                    .Replace("[[ESTADO]]", Estado)
                    .Replace("[[PAIS]]", Pais)
                    .Replace("[[CODIGO-POSTAL]]", CodigoPostal)
                    .Replace("[[TELEFONO]]", Telefono)
                    .Replace("[[DIRECCION-SUCURSAL]]", DireccionSucursal)
                    .Replace("[[CIUDAD-SUCURSAL]]", CiudadSucursal)
                    .Replace("[[ESTADO-SUCURSAL]]", EstadoSucursal)
                    .Replace("[[TITULO-CORTE-Z]]", reporte.TituloCorte)
                    .Replace("[[FECHA-CORTE-INICIO]]", reporte.FechaCorteInicio.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    .Replace("[[FECHA-CORTE-CIERRE]]", reporte.FechaCorteCierre.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    .Replace("[[FOLIO-CORTE]]", reporte.FolioCorte.ToString())

                    .Replace("[[CHEQUESREPORTE]]", chequesReporteTemplate) // CUENTAS

                    .Replace("[[TOTALESCHEQUESREPORTE]]", totaleschequesreporteTemplate) // TOTALES CUENTAS

                    .Replace($"[[{nameof(reporte.CuentasNormales).ToUpper()}]]", $"{reporte.CuentasNormales}") //CUENTAS
                    .Replace($"[[{nameof(reporte.CuentasCanceladas).ToUpper()}]]", $"{reporte.CuentasCanceladas}")
                    .Replace($"[[{nameof(reporte.CuentasConDescuento).ToUpper()}]]", $"{reporte.CuentasConDescuento}")
                    .Replace($"[[{nameof(reporte.CuentasConDescuentoImporte).ToUpper()}]]", string.Format("{0:C}", reporte.CuentasConDescuentoImporte))
                    .Replace($"[[{nameof(reporte.CuentasConCortesia).ToUpper()}]]", $"{reporte.CuentasConCortesia}")
                    .Replace($"[[{nameof(reporte.CuentasConCortesiaImporte).ToUpper()}]]", string.Format("{0:C}", reporte.CuentasConCortesiaImporte))
                    .Replace($"[[{nameof(reporte.CuentaPromedio).ToUpper()}]]", string.Format("{0:C}", reporte.CuentaPromedio))
                    .Replace($"[[{nameof(reporte.Comensales).ToUpper()}]]", $"{reporte.Comensales}")
                    .Replace($"[[{nameof(reporte.ConsumoPromedio).ToUpper()}]]", string.Format("{0:C}", reporte.ConsumoPromedio))
                    .Replace($"[[{nameof(reporte.Propinas).ToUpper()}]]", string.Format("{0:C}", reporte.Propinas))
                    .Replace($"[[{nameof(reporte.Cargos).ToUpper()}]]", string.Format("{0:C}", reporte.Cargos))
                    .Replace($"[[{nameof(reporte.DescuentoMonedero).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoMonedero))
                    .Replace($"[[{nameof(reporte.FolioInicial).ToUpper()}]]", $"{reporte.FolioInicial}")
                    .Replace($"[[{nameof(reporte.FolioFinal).ToUpper()}]]", $"{reporte.FolioFinal}")

                    .Replace("[[TOTALPROPINA]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.propina))) //TOTALES EN CUENTAS
                    .Replace("[[TOTALIMPORTE]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.total)))
                    .Replace("[[TOTALCARGO]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.cargo)))
                    .Replace("[[TOTALEFECTIVO]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.efectivo)))
                    .Replace("[[TOTALTARJETA]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.tarjeta)))
                    .Replace("[[TOTALVALES]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.vales)))
                    .Replace("[[TOTALOTROS]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.otros)))

                    .Replace($"[[{nameof(reporte.VentaFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.VentaFacturada)) // FACTURAS
                    .Replace($"[[{nameof(reporte.PropinaFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.PropinaFacturada))
                    .Replace($"[[{nameof(reporte.Facturado).ToUpper()}]]", string.Format("{0:C}", reporte.Facturado))
                    .Replace($"[[{nameof(reporte.VentaNoFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.VentaNoFacturada))

                    .Replace($"[[{nameof(reporte.EfectivoInicial).ToUpper()}]]", string.Format("{0:C}", reporte.EfectivoInicial)) // CAJA
                    .Replace($"[[{nameof(reporte.Efectivo).ToUpper()}]]", string.Format("{0:C}", reporte.Efectivo))
                    .Replace($"[[{nameof(reporte.Tarjeta).ToUpper()}]]", string.Format("{0:C}", reporte.Tarjeta))
                    .Replace($"[[{nameof(reporte.Vales).ToUpper()}]]", string.Format("{0:C}", reporte.Vales))
                    .Replace($"[[{nameof(reporte.Otros).ToUpper()}]]", string.Format("{0:C}", reporte.Otros))
                    .Replace($"[[{nameof(reporte.DepositosEfectivo).ToUpper()}]]", string.Format("{0:C}", reporte.DepositosEfectivo))
                    .Replace($"[[{nameof(reporte.RetirosEfectivo).ToUpper()}]]", string.Format("{0:C}", reporte.RetirosEfectivo))
                    .Replace($"[[{nameof(reporte.PropinasPagadas).ToUpper()}]]", string.Format("{0:C}", reporte.PropinasPagadas))
                    .Replace($"[[{nameof(reporte.SaldoFinal).ToUpper()}]]", string.Format("{0:C}", reporte.SaldoFinal))
                    .Replace($"[[{nameof(reporte.Dolares).ToUpper()}]]", string.Format("{0:C}", reporte.Dolares))
                    .Replace($"[[{nameof(reporte.TotalDeclarado).ToUpper()}]]", string.Format("{0:C}", reporte.TotalDeclarado))
                    .Replace("[[SOBRANTE]]", reporte.SobranteOFaltante > 0 ? string.Format("{0:C}", reporte.SobranteOFaltante) : "")
                    .Replace("[[FALTANTE]]", reporte.SobranteOFaltante < 0 ? string.Format("{0:C}", reporte.SobranteOFaltante * -1) : "")

                    .Replace("[[PAGOS]]", pagoVentasTemplate) // FORMA DE PAGO VENTAS

                    .Replace("[[PAGOPROPINAS]]", pagoPropinasTemplate) // FORMA DE PAGO PROPINAS
                    .Replace($"[[{nameof(reporte.TotalFormasPagoPropinas).ToUpper()}]]", string.Format("{0:C}", reporte.TotalFormasPagoPropinas))

                    .Replace($"[[{nameof(reporte.CortesiaAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.CortesiaAlimentos)) // CORTESIAS Y DESCUENTOS
                    .Replace($"[[{nameof(reporte.CortesiaBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.CortesiaBebidas))
                    .Replace($"[[{nameof(reporte.CortesiaOtros).ToUpper()}]]", string.Format("{0:C}", reporte.CortesiaOtros))
                    .Replace($"[[{nameof(reporte.TotalCortesias).ToUpper()}]]", string.Format("{0:C}", reporte.TotalCortesias))
                    .Replace($"[[{nameof(reporte.DescuentoAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoAlimentos))
                    .Replace($"[[{nameof(reporte.DescuentoBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoBebidas))
                    .Replace($"[[{nameof(reporte.DescuentoOtros).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoOtros))
                    .Replace($"[[{nameof(reporte.TotalDescuentos).ToUpper()}]]", string.Format("{0:C}", reporte.TotalDescuentos))
                    .Replace($"[[{nameof(reporte.Descuentos).ToUpper()}]]", string.Format("{0:C}", reporte.Descuentos))

                    .Replace($"[[{nameof(reporte.PAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.PAlimentos)) //VANTAS POR TIPO DE PRODUCTO
                    .Replace($"[[{nameof(reporte.PPorcentajeAlimentos).ToUpper()}]]", $"{reporte.PPorcentajeAlimentos}")
                    .Replace($"[[{nameof(reporte.PBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.PBebidas))
                    .Replace($"[[{nameof(reporte.PPorcentajeBebidas).ToUpper()}]]", $"{reporte.PPorcentajeBebidas}")
                    .Replace($"[[{nameof(reporte.POtros).ToUpper()}]]", string.Format("{0:C}", reporte.POtros))
                    .Replace($"[[{nameof(reporte.PPorcentajeOtros).ToUpper()}]]", $"{reporte.PPorcentajeOtros}")

                    .Replace($"[[{nameof(reporte.Comedor).ToUpper()}]]", string.Format("{0:C}", reporte.Comedor)) //VENTAS POR TIPO DE SERVICIO
                    .Replace($"[[{nameof(reporte.ComedorPorcentaje).ToUpper()}]]", $"{reporte.ComedorPorcentaje}")
                    .Replace($"[[{nameof(reporte.Domicilio).ToUpper()}]]", string.Format("{0:C}", reporte.Domicilio))
                    .Replace($"[[{nameof(reporte.DomicilioPorcentaje).ToUpper()}]]", $"{reporte.DomicilioPorcentaje}")
                    .Replace($"[[{nameof(reporte.Rapido).ToUpper()}]]", string.Format("{0:C}", reporte.Rapido))
                    .Replace($"[[{nameof(reporte.RapidoPorcentaje).ToUpper()}]]", $"{reporte.RapidoPorcentaje}")

                    .Replace($"[[{nameof(reporte.Subtotal).ToUpper()}]]", string.Format("{0:C}", reporte.Subtotal))
                    .Replace($"[[{nameof(reporte.Descuentos).ToUpper()}]]", string.Format("{0:C}", reporte.Descuentos))
                    .Replace($"[[{nameof(reporte.VentaNeta).ToUpper()}]]", string.Format("{0:C}", reporte.VentaNeta))
                    .Replace($"[[{nameof(reporte.ImpuestoTotal).ToUpper()}]]", string.Format("{0:C}", reporte.ImpuestoTotal))
                    .Replace($"[[{nameof(reporte.VentasConImpuesto).ToUpper()}]]", string.Format("{0:C}", reporte.VentasConImpuesto))

                    .Replace("[[VENTASRAPIDAS]]", ventasrapidasTemplate) // TURNOS

                    .Replace("[[TURNOS]]", turnosTemplate) // TURNOS
                    ;

                StringReader sr = new StringReader(HtmlInstance.ToString());

                Document pdfDoc = new Document(PageSize.LETTER, 20f, 20f, 30f, 30f);
                pdfDoc.SetPageSize(PageSize.A4.Rotate());

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();

                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css)))
                    {
                        using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(HtmlInstance)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
                        }
                    }

                    pdfDoc.Close();
                    var nombre_archivo = Path.Combine(PathDetalladoHorizontal, $"{reporte.FolioCorte}-detallado-horizontal.pdf");
                    byte[] bytes = AddPageNumbers(memoryStream.ToArray(), false);
                    memoryStream.Close();
                    var fs = new FileStream(nombre_archivo, FileMode.Create, FileAccess.Write);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();

                    if (tipoDestino == TipoDestino.EXPORTAR)
                    {
                        Process.Start("file://" + nombre_archivo);
                    }
                    else if (tipoDestino == TipoDestino.IMPRESION)
                    {
                        Impresion.Print_File(nombre_archivo, "", 0, 8, 8);

                        //File.Delete(nombre_archivo);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public static byte[] AddPageNumbers(byte[] pdf, bool orientacionVertical = true)
        {
            MemoryStream ms = new MemoryStream();
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(pdf);
            // we retrieve the total number of pages
            int n = reader.NumberOfPages;
            // we retrieve the size of the first page
            Rectangle psize = reader.GetPageSizeWithRotation(1);

            // step 1: creation of a document-object
            Document document = new Document(reader.GetPageSizeWithRotation(1));

            //if (orientacionVertical)
            //{
            //    document = new Document(psize, 30, 30, 20, 20);
            //}
            //else
            //{
            //    document = new Document(psize, 20, 20, 30, 30);
            //    //document.SetPageSize(PageSize.A4.Rotate());
            //}

            // step 2: we create a writer that listens to the document
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            // step 3: we open the document

            document.Open();
            // step 4: we add content
            PdfContentByte cb = writer.DirectContent;

            int p = 0;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                document.NewPage();
                p++;

                PdfImportedPage importedPage = writer.GetImportedPage(reader, page);
                //cb.AddTemplate(importedPage, 0, 0);

                int rotation = reader.GetPageRotation(page);

                if (rotation == 90 || rotation == 270)
                {
                    cb.AddTemplate(importedPage, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(page).Height);
                }
                else
                {
                    cb.AddTemplate(importedPage, 1f, 0, 0, 1f, 0, 0);
                }

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, $"PAG. {p}", psize.Width - (orientacionVertical ? 50 : 40), orientacionVertical ? 20 : 30, 0);
                cb.EndText();
            }
            // step 5: we close the document
            document.Close();
            return ms.ToArray();
        }

        public void DetalladoVertical(ReporteCorte reporte, TipoDestino tipoDestino)
        {
            try
            {
                DateTime fecha = DateTime.Now;

                var rutaHtml = Path.Combine(PathEjecuccion, @".\plantillas\corte-detallado-vertical.html");
                var rutaCss = Path.Combine(PathEjecuccion, @".\plantillas\estilos.css");

                string html = @File.ReadAllText(rutaHtml);
                string css = @File.ReadAllText(rutaCss);

                #region Cheques
                string source = "{{#CHEQUESREPORTE}}<tr class=\"fila\">{{>CHEQUEREPORTE}}</tr>{{/CHEQUESREPORTE}}";

                string partialSource =
                    "<td class=\"info text-center\">{{Snumcheque}}</td>" +
                    "<td class=\"info text-center\">{{Sfecha}}</td>" +
                    "<td class=\"info text-center\">{{Simpresiones}}</td>" +
                    "<td class=\"info text-center\">{{Sreabiertas}}</td>" +
                    "<td class=\"info text-center\">{{Sdescuento}}</td>" +
                    "<td class=\"info text-right\">{{Spropina}}</td>" +
                    "<td class=\"info text-right\">{{Simporte}}</td>" +
                    "<td class=\"info text-right\">{{Scargo}}</td>" +
                    "<td class=\"info text-right\">{{Sefectivo}}</td>" +
                    "<td class=\"info text-right\">{{Starjeta}}</td>" +
                    "<td class=\"info text-right\">{{Svales}}</td>" +
                    "<td class=\"info text-right\">{{Sotros}}</td>";

                Handlebars.RegisterTemplate("CHEQUEREPORTE", partialSource);

                var template1 = Handlebars.Compile(source);

                var data1 = new
                {
                    reporte.ChequesReporte
                };

                var chequesReporteTemplate = template1(data1);
                #endregion

                #region Pago ventas
                source = "{{#PAGOS}}<tr class=\"fila\">{{>PAGO}}</tr>{{/PAGOS}}";
                partialSource =
                    "<td>{{descripcion}}</td>" +
                    "<td>{{SImporte}}</td>";

                Handlebars.RegisterTemplate("PAGO", partialSource);

                var template2 = Handlebars.Compile(source);

                var data2 = new
                {
                    reporte.Pagos
                };

                var pagoVentasTemplate = template2(data2);
                #endregion

                #region Pago propinas
                var pagoPropinas = reporte.Pagos.Where(x => x.propina > 0).ToList();

                source = "{{#PAGOPROPINAS}}<tr class=\"fila\">{{> PAGOPROPINA}}</tr>{{/PAGOPROPINAS}}";
                partialSource =
                    "<td>{{descripcion}}</td>" +
                    "<td>{{SPropina}}</td>";

                Handlebars.RegisterTemplate("PAGOPROPINA", partialSource);

                var template3 = Handlebars.Compile(source);

                var data3 = new
                {
                    pagoPropinas
                };

                var pagoPropinasTemplate = template3(data3);
                #endregion

                #region Turnos
                source = "{{#TURNOS}}<tr class=\"fila\">{{> TURNO}}</tr>{{/TURNOS}}";
                partialSource =
                    "<td>{{idestacion}}</td>" +
                    "<td>{{Sapertura}}</td>" +
                    "<td>{{Scierre}}</td>" +
                    "<td>{{STotal}}</td>" +
                    "<td>{{SCargo}}</td>" +
                    "<td>{{Sefectivo}}</td>" +
                    "<td>{{Starjeta}}</td>" +
                    "<td>{{Svales}}</td>" +
                    "<td>{{SPropina}}</td>" +
                    "<td>{{Scredito}}</td>";

                Handlebars.RegisterTemplate("TURNO", partialSource);

                var template4 = Handlebars.Compile(source);

                var data4 = new
                {
                    reporte.Turnos
                };

                var turnosTemplate = template4(data4);
                #endregion

                #region Ventas Rapidas
                source = "{{#VENTASRAPIDAS}}<tr class=\"fila\">{{> VENTARAPIDA}}</tr>{{/VENTASRAPIDAS}}";
                partialSource =
                    "<td>{{Descripcion}}</td>" +
                    "<td class\"text-right\">{{STotal}}</td>";

                Handlebars.RegisterTemplate("VENTARAPIDA", partialSource);

                var template5 = Handlebars.Compile(source);

                var data5 = new
                {
                    reporte.VentasRapidas
                };

                var ventasrapidasTemplate = template5(data5);
                #endregion

                #region Totales cheques
                source = "{{#TOTALESCHEQUESREPORTE}}<tr class=\"fila\">{{> TOTALCHEQUEREPORTE}}</tr>{{/TOTALESCHEQUESREPORTE}}";
                partialSource =
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td></td>" +
                    "<td class=\"text-right\">{{Spropina}}</td>" +
                    "<td class=\"text-right\">{{Simporte}}</td>" +
                    "<td class=\"text-right\">{{Scargo}}</td>" +
                    "<td class=\"text-right\">{{Sefectivo}}</td>" +
                    "<td class=\"text-right\">{{Starjeta}}</td>" +
                    "<td class=\"text-right\">{{Svales}}</td>" +
                    "<td class=\"text-right\">{{Sotros}}</td>";

                Handlebars.RegisterTemplate("TOTALCHEQUEREPORTE", partialSource);

                var template6 = Handlebars.Compile(source);

                var data6 = new
                {
                    reporte.TotalesChequesReporte
                };

                var totaleschequesreporteTemplate = template6(data6);
                #endregion

                var HtmlTemplate = html;
                var HtmlInstance = HtmlTemplate
                    .Replace("[[FECHA]]", fecha.ToShortDateString()) //HEADER PAGINA
                    .Replace("[[HORA]]", fecha.ToString("hh:mm:ss tt").ToUpper())
                    .Replace("[[NOMBRE-COMERCIAL]]", NombreComercial)
                    .Replace("[[RAZON-SOCIAL]]", RazonSocial)
                    .Replace("[[RFC]]", RFC)
                    .Replace("[[DIRECCION-FISCAL]]", DireccionFiscal)
                    .Replace("[[CIUDAD]]", Ciudad)
                    .Replace("[[ESTADO]]", Estado)
                    .Replace("[[PAIS]]", Pais)
                    .Replace("[[CODIGO-POSTAL]]", CodigoPostal)
                    .Replace("[[TELEFONO]]", Telefono)
                    .Replace("[[DIRECCION-SUCURSAL]]", DireccionSucursal)
                    .Replace("[[CIUDAD-SUCURSAL]]", CiudadSucursal)
                    .Replace("[[ESTADO-SUCURSAL]]", EstadoSucursal)
                    .Replace("[[TITULO-CORTE-Z]]", reporte.TituloCorte)
                    .Replace("[[FECHA-CORTE-INICIO]]", reporte.FechaCorteInicio.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    .Replace("[[FECHA-CORTE-CIERRE]]", reporte.FechaCorteCierre.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    .Replace("[[FOLIO-CORTE]]", reporte.FolioCorte.ToString())

                    .Replace("[[CHEQUESREPORTE]]", chequesReporteTemplate) // CUENTAS

                    .Replace("[[TOTALESCHEQUESREPORTE]]", totaleschequesreporteTemplate) // TOTALES CUENTAS

                    .Replace($"[[{nameof(reporte.CuentasNormales).ToUpper()}]]", $"{reporte.CuentasNormales}") //CUENTAS
                    .Replace($"[[{nameof(reporte.CuentasCanceladas).ToUpper()}]]", $"{reporte.CuentasCanceladas}")
                    .Replace($"[[{nameof(reporte.CuentasConDescuento).ToUpper()}]]", $"{reporte.CuentasConDescuento}")
                    .Replace($"[[{nameof(reporte.CuentasConDescuentoImporte).ToUpper()}]]", string.Format("{0:C}", reporte.CuentasConDescuentoImporte))
                    .Replace($"[[{nameof(reporte.CuentasConCortesia).ToUpper()}]]", $"{reporte.CuentasConCortesia}")
                    .Replace($"[[{nameof(reporte.CuentasConCortesiaImporte).ToUpper()}]]", string.Format("{0:C}", reporte.CuentasConCortesiaImporte))
                    .Replace($"[[{nameof(reporte.CuentaPromedio).ToUpper()}]]", string.Format("{0:C}", reporte.CuentaPromedio))
                    .Replace($"[[{nameof(reporte.Comensales).ToUpper()}]]", $"{reporte.Comensales}")
                    .Replace($"[[{nameof(reporte.ConsumoPromedio).ToUpper()}]]", string.Format("{0:C}", reporte.ConsumoPromedio))
                    .Replace($"[[{nameof(reporte.Propinas).ToUpper()}]]", string.Format("{0:C}", reporte.Propinas))
                    .Replace($"[[{nameof(reporte.Cargos).ToUpper()}]]", string.Format("{0:C}", reporte.Cargos))
                    .Replace($"[[{nameof(reporte.DescuentoMonedero).ToUpper()}]]", string.Format("{0:C}", reporte.DescuentoMonedero))
                    .Replace($"[[{nameof(reporte.FolioInicial).ToUpper()}]]", $"{reporte.FolioInicial}")
                    .Replace($"[[{nameof(reporte.FolioFinal).ToUpper()}]]", $"{reporte.FolioFinal}")

                    .Replace("[[TOTALPROPINA]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.propina))) //TOTALES EN CUENTAS
                    .Replace("[[TOTALIMPORTE]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.total)))
                    .Replace("[[TOTALCARGO]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.cargo)))
                    .Replace("[[TOTALEFECTIVO]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.efectivo)))
                    .Replace("[[TOTALTARJETA]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.tarjeta)))
                    .Replace("[[TOTALVALES]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.vales)))
                    .Replace("[[TOTALOTROS]]", string.Format("{0:C}", reporte.ChequesReporte.Sum(x => x.otros)))

                    .Replace($"[[{nameof(reporte.VentaFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.VentaFacturada)) // FACTURAS
                    .Replace($"[[{nameof(reporte.PropinaFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.PropinaFacturada))
                    .Replace($"[[{nameof(reporte.Facturado).ToUpper()}]]", string.Format("{0:C}", reporte.Facturado))
                    .Replace($"[[{nameof(reporte.VentaNoFacturada).ToUpper()}]]", string.Format("{0:C}", reporte.VentaNoFacturada))

                    .Replace($"[[{nameof(reporte.EfectivoInicial).ToUpper()}]]", string.Format("{0:C}", reporte.EfectivoInicial)) // CAJA
                    .Replace($"[[{nameof(reporte.Efectivo).ToUpper()}]]", string.Format("{0:C}", reporte.Efectivo))
                    .Replace($"[[{nameof(reporte.Tarjeta).ToUpper()}]]", string.Format("{0:C}", reporte.Tarjeta))
                    .Replace($"[[{nameof(reporte.Vales).ToUpper()}]]", string.Format("{0:C}", reporte.Vales))
                    .Replace($"[[{nameof(reporte.Otros).ToUpper()}]]", string.Format("{0:C}", reporte.Otros))
                    .Replace($"[[{nameof(reporte.DepositosEfectivo).ToUpper()}]]", string.Format("{0:C}", reporte.DepositosEfectivo))
                    .Replace($"[[{nameof(reporte.RetirosEfectivo).ToUpper()}]]", string.Format("{0:C}", reporte.RetirosEfectivo))
                    .Replace($"[[{nameof(reporte.PropinasPagadas).ToUpper()}]]", string.Format("{0:C}", reporte.PropinasPagadas))
                    .Replace($"[[{nameof(reporte.SaldoFinal).ToUpper()}]]", string.Format("{0:C}", reporte.SaldoFinal))
                    .Replace($"[[{nameof(reporte.Dolares).ToUpper()}]]", string.Format("{0:C}", reporte.Dolares))
                    .Replace($"[[{nameof(reporte.TotalDeclarado).ToUpper()}]]", string.Format("{0:C}", reporte.TotalDeclarado))
                    .Replace("[[SOBRANTE]]", reporte.SobranteOFaltante > 0 ? string.Format("{0:C}", reporte.SobranteOFaltante) : "")
                    .Replace("[[FALTANTE]]", reporte.SobranteOFaltante < 0 ? string.Format("{0:C}", reporte.SobranteOFaltante * -1) : "")

                    .Replace("[[PAGOS]]", pagoVentasTemplate) // FORMA DE PAGO VENTAS

                    .Replace("[[PAGOPROPINAS]]", pagoPropinasTemplate) // FORMA DE PAGO PROPINAS

                    .Replace($"[[{nameof(reporte.TotalFormasPagoPropinas).ToUpper()}]]", string.Format("{0:C}", reporte.TotalFormasPagoPropinas))

                    .Replace($"[[{nameof(reporte.PAlimentos).ToUpper()}]]", string.Format("{0:C}", reporte.PAlimentos)) //VANTAS POR TIPO DE PRODUCTO
                    .Replace($"[[{nameof(reporte.PPorcentajeAlimentos).ToUpper()}]]", $"{reporte.PPorcentajeAlimentos}")
                    .Replace($"[[{nameof(reporte.PBebidas).ToUpper()}]]", string.Format("{0:C}", reporte.PBebidas))
                    .Replace($"[[{nameof(reporte.PPorcentajeBebidas).ToUpper()}]]", $"{reporte.PPorcentajeBebidas}")
                    .Replace($"[[{nameof(reporte.POtros).ToUpper()}]]", string.Format("{0:C}", reporte.POtros))
                    .Replace($"[[{nameof(reporte.PPorcentajeOtros).ToUpper()}]]", $"{reporte.PPorcentajeOtros}")

                    .Replace($"[[{nameof(reporte.Comedor).ToUpper()}]]", string.Format("{0:C}", reporte.Comedor)) //VENTAS POR TIPO DE SERVICIO
                    .Replace($"[[{nameof(reporte.ComedorPorcentaje).ToUpper()}]]", $"{reporte.ComedorPorcentaje}")
                    .Replace($"[[{nameof(reporte.Domicilio).ToUpper()}]]", string.Format("{0:C}", reporte.Domicilio))
                    .Replace($"[[{nameof(reporte.DomicilioPorcentaje).ToUpper()}]]", $"{reporte.DomicilioPorcentaje}")
                    .Replace($"[[{nameof(reporte.Rapido).ToUpper()}]]", string.Format("{0:C}", reporte.Rapido))
                    .Replace($"[[{nameof(reporte.RapidoPorcentaje).ToUpper()}]]", $"{reporte.RapidoPorcentaje}")

                    .Replace($"[[{nameof(reporte.Subtotal).ToUpper()}]]", string.Format("{0:C}", reporte.Subtotal))
                    .Replace($"[[{nameof(reporte.Descuentos).ToUpper()}]]", string.Format("{0:C}", reporte.Descuentos))
                    .Replace($"[[{nameof(reporte.VentaNeta).ToUpper()}]]", string.Format("{0:C}", reporte.VentaNeta))
                    .Replace($"[[{nameof(reporte.ImpuestoTotal).ToUpper()}]]", string.Format("{0:C}", reporte.ImpuestoTotal))
                    .Replace($"[[{nameof(reporte.VentasConImpuesto).ToUpper()}]]", string.Format("{0:C}", reporte.VentasConImpuesto))

                    .Replace("[[VENTASRAPIDAS]]", ventasrapidasTemplate) // TURNOS

                    .Replace("[[TURNOS]]", turnosTemplate) // TURNOS
                    ;

                StringReader sr = new StringReader(HtmlInstance.ToString());

                Document pdfDoc = new Document(PageSize.LETTER, 30f, 30f, 20f, 20f);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                    pdfDoc.Open();

                    using (var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css)))
                    {
                        using (var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(HtmlInstance)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
                        }
                    }

                    pdfDoc.Close();
                    var nombre_archivo = Path.Combine(PathDetalladoVertical, $"{reporte.FolioCorte}-detallado-vertical.pdf");
                    byte[] bytes = AddPageNumbers(memoryStream.ToArray());
                    memoryStream.Close();
                    var fs = new FileStream(nombre_archivo, FileMode.Create, FileAccess.Write);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();

                    if (tipoDestino == TipoDestino.EXPORTAR)
                    {
                        Process.Start("file://" + nombre_archivo);
                    }
                    else if (tipoDestino == TipoDestino.IMPRESION)
                    {
                        Impresion.Print_File(nombre_archivo, "", 0, 8, 8);

                        //File.Delete(nombre_archivo);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void Resumido(ReporteCorte reporte, TipoDestino tipoDestino)
        {
            List<string> lineas = new List<string>();
            string filePathCorte = Path.Combine(PathEjecuccion, "corte.txt");
            string filePathTarjeta = Path.Combine(PathEjecuccion, "tarjeta.txt");

            List<string> multilineas = new List<string>();

            multilineas.AddRange(Multilinia(NombreComercial));
            multilineas.AddRange(Multilinia(RazonSocial));

            multilineas.AddRange(Multilinia($"RFC: {RFC}"));

            multilineas.AddRange(Multilinia($"{DireccionFiscal} {Ciudad} {Estado} {Pais}  CP  {CodigoPostal}"));

            foreach (var linea in multilineas)
            {
                lineas.Add(linea);
            }

            multilineas.Clear();
            multilineas = Multilinia($"{DireccionSucursal} {CiudadSucursal} {EstadoSucursal}");

            lineas.Add("");
            foreach (var linea in multilineas)
            {
                lineas.Add(linea);
            }

            lineas.Add("");
            lineas.Add($"{reporte.TituloCorte}");
            lineas.Add(string.Format("DEL {0}", reporte.FechaCorteInicio.ToString("dd/MM/yyyy")));
            lineas.Add("");
            lineas.Add($"FOLIO {reporte.TituloCorte}: {reporte.FolioCorte}");
            lineas.Add(Relleno('='));
            lineas.Add(Centrado("CAJA"));

            if (reporte.ConsiderarFondoInicial)
                lineas.Add(Relleno(40, new Division("+ EFECTIVO INICIAL:", 20), new Division(string.Format("{0:C}", reporte.EfectivoInicial))));

            lineas.Add(Relleno(40, new Division("+ EFECTIVO:", 20), new Division(string.Format("{0:C}", reporte.Efectivo))));
            lineas.Add(Relleno(40, new Division("+ TARJETA:", 20), new Division(string.Format("{0:C}", reporte.Tarjeta))));
            lineas.Add(Relleno(40, new Division("+ VALES:", 20), new Division(string.Format("{0:C}", reporte.Vales))));
            lineas.Add(Relleno(40, new Division("+ OTROS:", 20), new Division(string.Format("{0:C}", reporte.Otros))));

            if (!reporte.NoConsiderarDepositosRetiros)
            {
                lineas.Add(Relleno(40, new Division("+ DEPÓSITOS EFECTIVO:", 20), new Division(string.Format("{0:C}", reporte.DepositosEfectivo))));
                lineas.Add(Relleno(40, new Division("- RETIROS EFECTIVO:", 20), new Division(string.Format("{0:C}", reporte.RetirosEfectivo))));
            }

            if (!reporte.NoConsiderarPropinas)
                lineas.Add(Relleno(40, new Division("- PROPINAS PAGADAS:", 20), new Division(string.Format("{0:C}", reporte.PropinasPagadas))));


            lineas.Add(Relleno(40, new Division("", 20), new Division("____________")));
            lineas.Add(Relleno(40, new Division("= SALDO FINAL:", 20), new Division(string.Format("{0:C}", reporte.SaldoFinal))));
            lineas.Add(Relleno(40, new Division("  EFECTIVO FINAL:", 20), new Division(string.Format("{0:C}", reporte.EfectivoFinal))));
            lineas.Add("");
            lineas.Add(Centrado("FORMA DE PAGO VENTAS"));
            lineas.Add("");

            foreach (var pago in reporte.Pagos)
            {
                lineas.Add(Relleno(40, new Division(pago.descripcion, 24), new Division(string.Format("{0:C}", pago.importe))));
            }

            lineas.Add(Relleno('-'));
            lineas.Add(string.Format("TOTAL FORMAS DE PAGO VENTAS: {0:C}", reporte.TotalFormasPagoVentas));

            if (!reporte.NoConsiderarPropinas)
            {
                lineas.Add("");
                lineas.Add("");
                lineas.Add("");
                lineas.Add("");
                lineas.Add("");
                lineas.Add(Centrado("FORMA DE PAGO PROPINA"));

                var pagosPropina = reporte.Pagos.Where(x => x.propina > 0).ToList();
                foreach (var pago in pagosPropina)
                {
                    lineas.Add(Relleno(40, new Division(pago.descripcion, 24), new Division(string.Format("{0:C}", pago.propina))));
                }

                lineas.Add(Relleno('-'));
                lineas.Add(string.Format("TOTAL FORMAS DE PAGO PROPINA: {0:C}", reporte.TotalFormasPagoPropinas));
            }

            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add(Relleno('='));
            lineas.Add(Centrado("VENTA (NO INCLUYE IMPUESTOS)"));
            lineas.Add(Centrado("POR TIPO DE PRODUCTO"));
            lineas.Add(
                Relleno(40, 
                new Division("ALIMENTOS:", 17), 
                new Division(string.Format("{0:C}", reporte.PAlimentos), 15), 
                new Division($"({reporte.PPorcentajeAlimentos}%) {reporte.PCantidadAlimentos}"))
            );
            lineas.Add(
                Relleno(40,
                new Division("BEBIDAS:", 17),
                new Division(string.Format("{0:C}", reporte.PBebidas), 15),
                new Division($"({reporte.PPorcentajeBebidas}%) {reporte.PCantidadBebidas}"))
            );
            lineas.Add(
                Relleno(40,
                new Division("OTROS:", 17),
                new Division(string.Format("{0:C}", reporte.POtros), 15),
                new Division($"({reporte.PPorcentajeOtros}%) {reporte.PCantidadOtros}"))
            );
            lineas.Add(Centrado("POR TIPO DE SERVICIO"));
            lineas.Add(
                Relleno(40,
                new Division("COMEDOR:", 17),
                new Division(string.Format("{0:C}", reporte.Comedor), 15),
                new Division($"({reporte.ComedorPorcentaje} %)"))
            );
            lineas.Add(
                Relleno(40,
                new Division("DOMICILIO:", 17),
                new Division(string.Format("{0:C}", reporte.Domicilio), 15),
                new Division($"({reporte.DomicilioPorcentaje} %)"))
            );
            lineas.Add(
                Relleno(40,
                new Division("RAPIDO:", 17),
                new Division(string.Format("{0:C}", reporte.Rapido), 15),
                new Division($"({reporte.Rapido} %)"))
            );
            lineas.Add(Relleno(40, new Division("", 17), new Division("______________")));
            lineas.Add(Relleno(40, new Division(" SUBTOTAL    :", 17), new Division(string.Format("{0:C}", reporte.Subtotal))));
            lineas.Add(Relleno(40, new Division("-DESCUENTOS  :", 17), new Division(string.Format("{0:C}", reporte.Descuentos))));
            lineas.Add(Relleno(40, new Division(" VENTA NETA  :", 17), new Division(string.Format("{0:C}", reporte.VentaNeta))));
            lineas.Add(Relleno(40, new Division("", 17), new Division("______________")));

            foreach (var impuestoVenta in reporte.ImpuestosVentas)
            {
                lineas.Add(Relleno(40, 
                    new Division("VENTA", 8), 
                    new Division($"{impuestoVenta.porcentaje}%:".PadLeft(5, ' '), 9), 
                    new Division(string.Format("{0:C}", impuestoVenta.venta))));
                lineas.Add(Relleno(40,
                    new Division("IMPUESTO", 8),
                    new Division($"{impuestoVenta.porcentaje}%:".PadLeft(5, ' '), 9),
                    new Division(string.Format("{0:C}", impuestoVenta.impuesto))));
            }

            lineas.Add("");
            lineas.Add(Relleno(40, new Division("IMPUESTOS TOTAL:", 17), new Division(string.Format("{0:C}", reporte.ImpuestoTotal))));
            lineas.Add(Relleno(40, new Division("", 17), new Division("______________")));
            lineas.Add(Relleno(40, new Division("VENTAS CON IMP.:", 17), new Division(string.Format("{0:C}", reporte.VentasConImpuesto))));
            lineas.Add(Relleno(40, new Division("", 17), new Division("==============")));
            lineas.Add("");
            lineas.Add(Relleno(40, new Division("VENTA FACTURADA:", 18), new Division(string.Format("{0:C}", reporte.VentaFacturada))));
            lineas.Add(Relleno(40, new Division("PROPINA FACTURADA:", 18), new Division(string.Format("{0:C}", reporte.PropinaFacturada))));
            lineas.Add(Relleno(40, new Division("", 18), new Division("______________")));
            lineas.Add(Relleno(40, new Division("FACTURADO:", 18), new Division(string.Format("{0:C}", reporte.Facturado))));
            lineas.Add(Relleno(40, new Division("VENTA NO FACTURADA:", 18), new Division(string.Format("{0:C}", reporte.VentaNoFacturada))));
            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add("");
            lineas.Add(Relleno('='));
            lineas.Add(Relleno(40, new Division("CUENTAS NORMALES", 22), new Division(":", 2), new Division($"{reporte.CuentasNormales}")));
            lineas.Add(Relleno(40, new Division("CUENTAS CANCELADAS", 22), new Division(":", 2), new Division($"{reporte.CuentasCanceladas}")));
            lineas.Add(Relleno(40, new Division("CUENTAS CON DESCUENTO", 22), new Division(":", 2), new Division($"{reporte.CuentasConDescuento}")));
            lineas.Add(Relleno(40, new Division("CUENTAS CON CORTESIA", 22), new Division(":", 2), new Division($"{reporte.CuentasConCortesia}")));
            lineas.Add(Relleno(40, new Division("CUENTA PROMEDIO", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.CuentaPromedio))));
            lineas.Add(Relleno(40, new Division("CONSUMO PROMEDIO", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.ConsumoPromedio))));
            lineas.Add(Relleno(40, new Division("COMENSALES", 22), new Division(":", 2), new Division($"{reporte.Comensales}")));

            if (!reporte.NoConsiderarPropinas)
                lineas.Add(Relleno(40, new Division("PROPINAS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.Propinas))));

            lineas.Add(Relleno(40, new Division("CARGOS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.Cargos))));
            lineas.Add(Relleno(40, new Division("DESCUENTO MONEDERO", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.DescuentoMonedero))));
            lineas.Add(Relleno(40, new Division("FOLIO INICIAL", 22), new Division(":", 2), new Division($"{reporte.FolioInicial}")));
            lineas.Add(Relleno(40, new Division("FOLIO FINAL", 22), new Division(":", 2), new Division($"{reporte.FolioFinal}")));
            lineas.Add(Relleno(40, new Division("CORTESIA ALIMENTOS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.CortesiaAlimentos))));
            lineas.Add(Relleno(40, new Division("CORTESIA BEBIDAS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.CortesiaBebidas))));
            lineas.Add(Relleno(40, new Division("CORTESIA OTROS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.CortesiaOtros))));
            lineas.Add(Relleno(40, new Division("", 23), new Division("______________")));
            lineas.Add(Relleno(40, new Division("TOTAL CORTESIAS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.TotalCortesias))));
            lineas.Add(Relleno(40, new Division("DESCUENTO ALIMENTOS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.DescuentoAlimentos))));
            lineas.Add(Relleno(40, new Division("DESCUENTO BEBIDAS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.DescuentoBebidas))));
            lineas.Add(Relleno(40, new Division("DESCUENTO OTROS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.DescuentoOtros))));
            lineas.Add(Relleno(40, new Division("", 23), new Division("______________")));
            lineas.Add(Relleno(40, new Division("TOTAL DESCUENTOS", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.TotalDescuentos))));
            lineas.Add(Relleno(40, new Division("TOTAL DECLARADO", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.TotalDeclarado))));
            lineas.Add(Relleno(40, new Division("SOBRANTE O FALTANTE", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.SobranteOFaltante))));
            lineas.Add(Relleno(40, new Division("ACUMULADO MES ANTERIOR", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.AcumuladoMesAnterior))));
            lineas.Add(Relleno(40, new Division("ACUMULADO MES ACTUAL", 22), new Division(":", 2), new Division(string.Format("{0:C}", reporte.AcumuladoMesActual))));
            lineas.Add("");
            lineas.Add("");
            lineas.Add(Relleno('_', 15));
            lineas.Add(Centrado("GERENTE", 15));
            lineas.Add("");
            lineas.Add("");

            File.WriteAllLines(filePathCorte, lineas);

            lineas.Clear();
            lineas.Add("MOVIMIENTOS DE TARJETAS DE CREDITO");
            lineas.Add("");
            lineas.Add(string.Format("DEL {0}", reporte.FechaCorteInicio.ToString("dd/MM/yyyy hh:mm:ss tt")));
            lineas.Add(string.Format(" AL {0}", reporte.FechaCorteCierre.ToString("dd/MM/yyyy hh:mm:ss tt")));
            lineas.Add(Relleno('='));
            lineas.Add("");
            lineas.Add("");
            lineas.Add(Relleno(40, new Division(" FOLIO", 10), new Division("DESCRIPCION", 13), new Division(" CARGO")));

            foreach (var pagoTarjeta in reporte.PagosTarjeta)
            {
                lineas.Add(
                    Relleno(40, 
                    new Division(" "),
                    new Division($"{pagoTarjeta.numcheque}", 9),
                    new Division(pagoTarjeta.descripcion, 12), 
                    new Division(" "),
                    new Division(string.Format("{0:C}", reporte.NoConsiderarPropinas ? pagoTarjeta.importe : pagoTarjeta.Cargo))));
            }

            lineas.Add("");
            lineas.Add(Relleno('='));

            var pagos = reporte.Pagos.Where(x => x.propina > 0).OrderBy(x => x.idformadepago).ToList();

            foreach (var pago in pagos)
            {
                lineas.Add(pago.descripcion);
                lineas.Add(Relleno(40, new Division("VENTAS:", 10), new Division(string.Format("{0:C}", pago.importe))));

                if (reporte.NoConsiderarPropinas)
                {
                    lineas.Add(Relleno(40, new Division("TOTAL:", 10), new Division(string.Format("{0:C}", pago.importe))));
                }
                else
                {
                    lineas.Add(Relleno(40, new Division("PROPINAS:", 10), new Division(string.Format("{0:C}", pago.propina))));
                    lineas.Add(Relleno(40, new Division("TOTAL:", 10), new Division(string.Format("{0:C}", pago.importe + pago.propina))));
                }

                lineas.Add(Relleno('-'));
            }

            File.WriteAllLines(filePathTarjeta, lineas);

            if (tipoDestino == TipoDestino.EXPORTAR)
            {
                Process.Start(filePathCorte);
                Process.Start(filePathTarjeta);
            }
            else if (tipoDestino == TipoDestino.IMPRESION)
            {
                Impresion.Print_File(filePathCorte, "", 0, 8, 8);
                Impresion.Print_File(filePathTarjeta, "", 0, 8, 8);
            }
        }

        private List<string> Multilinia(string texto, int columnas = 40)
        {
            List<string> lineas = new List<string>();
            
            do
            {
                if (texto.Length > columnas)
                {
                    string parte = texto.Substring(0, columnas);
                    lineas.Add(parte);
                    texto = texto.Replace(parte, "");
                }
                else
                {
                    lineas.Add(texto);
                    texto = "";
                }
            } while (texto.Length > 0);

            return lineas;
        }

        private string Centrado(string texto, int columnas = 40)
        {
            if (texto.Length > columnas)
            {
                return texto.Substring(0, columnas);
            }

            int espaciosobrante = columnas - texto.Length;

            int izquierda = espaciosobrante / 2;
            //int derecha = izquierda;

            //if (espaciosobrante % 2 != 0)
            //{
            //    derecha++;
            //}

            string result = texto.PadLeft(texto.Length + izquierda, ' ');
            //result = result.PadRight(result.Length + derecha, caracterRelleno);
            return result;
        }
        private string Relleno(char caracterRelleno = ' ', int columnas = 40)
        {
            return "".PadRight(columnas, caracterRelleno);
        }

        private string Relleno(int columnas = 40, params Division[] divisiones)
        {
            string texto = "";

            for (int i = 0; i < divisiones.Length; i++)
            {
                var division = divisiones[i];

                if (division.Texto.Length > division.Longitud)
                {
                    texto += division.Texto.Substring(0, division.Longitud);
                }
                else
                {
                    texto += division.Texto.PadRight(division.Longitud, division.CaracterRelleno);
                }
            }

            //if (texto.Length > columnas) texto = texto.Substring(0, columnas);

            if (texto.Length > columnas) return texto;

            return texto;
        }
    }

    public class Division
    {
        public Division(string texto, char caraterRelleno = ' ')
        {
            Texto = texto;
            Longitud = texto.Length;
            CaracterRelleno = caraterRelleno;
        }

        public Division(string texto, int longitud, char caraterRelleno = ' ')
        {
            Texto = texto;
            Longitud = longitud;
            CaracterRelleno = caraterRelleno;
        }

        public string Texto { get; set; }
        public int Longitud { get; set; }
        public char CaracterRelleno { get; set; }
    }
}
