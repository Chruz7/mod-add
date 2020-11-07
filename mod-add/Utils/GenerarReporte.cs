using mod_add.Enums;
using mod_add.Modelos;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace mod_add.Utils
{
    public static class GenerarReporte
    {
        public static void CorteZ(ReporteZ reporte, TipoDestino tipoDestino)
        {
            List<string> lineas = new List<string>();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filePathCorte = Path.Combine(path, "corte.txt");
            string filePathTarjeta = Path.Combine(path, "tarjeta.txt");
            List<string> multilineas = new List<string>();

            multilineas.AddRange(Multilinia(ConfiguracionLocalServicio.ReadSetting("NOMBRE-COMERCIAL")));
            multilineas.AddRange(Multilinia(ConfiguracionLocalServicio.ReadSetting("RAZON-SOCIAL")));

            string rfc = ConfiguracionLocalServicio.ReadSetting("RFC");
            multilineas.AddRange(Multilinia($"RFC: {rfc}"));

            string direccionFiscal = ConfiguracionLocalServicio.ReadSetting("DIRECCION-FISCAL");
            string cp = ConfiguracionLocalServicio.ReadSetting("CODIGO-POSTAL");
            string ciudad = ConfiguracionLocalServicio.ReadSetting("CIUDAD");
            string estado = ConfiguracionLocalServicio.ReadSetting("ESTADO");
            string pais = ConfiguracionLocalServicio.ReadSetting("PAIS");
            multilineas.AddRange(Multilinia($"{direccionFiscal} {ciudad} {estado} {pais}  CP  {cp}"));

            foreach (var linea in multilineas)
            {
                lineas.Add(linea);
            }

            string direccionSucursal = ConfiguracionLocalServicio.ReadSetting("DIRECCION-SUCURSAL");
            string ciudadSucursal = ConfiguracionLocalServicio.ReadSetting("CIUDAD-SUCURSAL");
            string estadoSucursal = ConfiguracionLocalServicio.ReadSetting("ESTADO-SUCURSAL");

            multilineas.Clear();
            multilineas = Multilinia($"{direccionSucursal} {ciudadSucursal} {estadoSucursal}");

            lineas.Add("");
            foreach (var linea in multilineas)
            {
                lineas.Add(linea);
            }

            lineas.Add("");
            lineas.Add($"{reporte.TituloCorteZ}");
            lineas.Add(string.Format("DEL {0}", reporte.FechaCorteInicio.ToString("dd/MM/yyyy")));
            lineas.Add("");
            lineas.Add($"FOLIO {reporte.TituloCorteZ}: {reporte.FolioCorte}");
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

                if (!reporte.NoConsiderarPropinas)
                    lineas.Add(Relleno(40, new Division("PROPINAS:", 10), new Division(string.Format("{0:C}", pago.propina))));

                lineas.Add(Relleno(40, new Division("TOTAL:", 10), new Division(string.Format("{0:C}", pago.importe + pago.propina))));
                lineas.Add(Relleno('-'));
            }

            File.WriteAllLines(filePathTarjeta, lineas);

            if (tipoDestino == TipoDestino.EXPORTAR_TXT)
            {
                Process.Start(filePathCorte);
                Process.Start(filePathTarjeta);
            }
            else if (tipoDestino == TipoDestino.IMPRESION)
            {
                SRLibrary.Utils.Print print = new SRLibrary.Utils.Print();

                print.Print_File(filePathCorte, "", 0, 8, 8);
                print.Print_File(filePathTarjeta, "", 0, 8, 8);
            }
        }

        public static List<string> Multilinia(string texto, int columnas = 40)
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

        public static string Centrado(string texto, char caracterRelleno, int columnas = 40)
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

            string result = texto.PadLeft(texto.Length + izquierda, caracterRelleno);
            //result = result.PadRight(result.Length + derecha, caracterRelleno);
            return result;
        }

        public static string Centrado(string texto, int columnas = 40)
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
        private static string Relleno(char caracterRelleno = ' ', int columnas = 40)
        {
            return "".PadRight(columnas, caracterRelleno);
        }


        private static string Relleno(int columnas = 40, params Division[] divisiones)
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
