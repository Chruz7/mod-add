using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace mod_add.Modelos
{
    public class ReporteCorte
    {
        public ReporteCorte()
        {
            Pagos = new List<Pago>();
            PagosTarjeta = new List<PagoTarjeta>();
            ImpuestosVentas = new List<ImpuestoVenta>();
            ChequesReporte = new List<ChequeReporte>();
            TotalesChequesReporte = new List<ChequeReporte>();
            Turnos = new List<TurnoReporte>();
            VentasRapidas = new List<VentaRapida>();
        }
        public DateTime Fecha { get; set; }
        public string SoloFecha { get { return Fecha.ToString("dd/MM/yyyy"); } }
        public string SoloHora { get { return FechaCorteInicio.ToString("hh:mm:ss tt", CultureInfo.CreateSpecificCulture("US")); } }
        public string TituloCorte { get; set; }
        public long FolioCorte { get; set; }
        public string SFolioCorte { get { return $"{FolioCorte}"; } }
        public DateTime FechaCorteInicio { get; set; }
        public string SFechaCorteInicio { get { return FechaCorteInicio.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("US")); } }
        public DateTime FechaCorteCierre { get; set; }
        public string SFechaCorteCierre { get { return FechaCorteCierre.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("US")); } }
        public decimal EfectivoInicial { get; set; }
        public string SEfectivoInicial { get { return string.Format("{0:C}", EfectivoInicial); } }
        public decimal Efectivo { get; set; }
        public string SEfectivo { get { return string.Format("{0:C}", Efectivo); } }
        public decimal Tarjeta { get; set; }
        public string STarjeta { get { return string.Format("{0:C}", Tarjeta); } }
        public decimal Vales { get; set; }
        public string SVales { get { return string.Format("{0:C}", Vales); } }
        public decimal Otros { get; set; }
        public string SOtros { get { return string.Format("{0:C}", Otros); } }
        public decimal DepositosEfectivo { get; set; }
        public string SDepositosEfectivo { get { return string.Format("{0:C}", DepositosEfectivo); } }
        public decimal RetirosEfectivo { get; set; }
        public string SRetirosEfectivo { get { return string.Format("{0:C}", RetirosEfectivo); } }
        public decimal PropinasPagadas { get; set; }
        public string SPropinasPagadas { get { return string.Format("{0:C}", PropinasPagadas); } }
        public decimal SaldoFinal
        {
            get
            {
                return EfectivoInicial + Efectivo + Tarjeta + Vales + Otros + DepositosEfectivo - RetirosEfectivo - PropinasPagadas;
            }
        }
        public string SSaldoFinal { get { return string.Format("{0:C}", SaldoFinal); } }
        public decimal EfectivoFinal
        {
            get
            {
                return EfectivoInicial + Efectivo - PropinasPagadas;
            }
        }
        public string SEfectivoFinal { get { return string.Format("{0:C}", EfectivoFinal); } }
        public decimal TotalFormasPagoVentas
        {
            get
            {
                return Pagos.Sum(x => x.importe);
            }
        }
        public string STotalFormasPagoVentas { get { return string.Format("{0:C}", TotalFormasPagoVentas); } }
        public decimal TotalFormasPagoPropinas
        {
            get
            {
                return Pagos.Sum(x => x.propina);
            }
        }
        public string STotalFormasPagoPropinas { get { return string.Format("{0:C}", TotalFormasPagoPropinas); } }
        public decimal PAlimentos { get; set; }
        public string SPAlimentos { get { return string.Format("{0:C}", PAlimentos); } }
        public double PCantidadAlimentos { get; set; }
        public decimal PPorcentajeAlimentos { get; set; }
        public decimal PBebidas { get; set; }
        public string SPBebidas { get { return string.Format("{0:C}", PBebidas); } }
        public double PCantidadBebidas { get; set; }
        public decimal PPorcentajeBebidas { get; set; }
        public decimal POtros { get; set; }
        public string SPOtros { get { return string.Format("{0:C}", POtros); } }
        public double PCantidadOtros { get; set; }
        public decimal PPorcentajeOtros { get; set; }
        public decimal Comedor { get; set; }
        public string SComedor { get { return string.Format("{0:C}", Comedor); } }
        public decimal ComedorPorcentaje { get; set; }
        public decimal Domicilio { get; set; }
        public string SDomicilio { get { return string.Format("{0:C}", Domicilio); } }
        public decimal DomicilioPorcentaje { get; set; }
        public decimal Rapido { get; set; }
        public string SRapido { get { return string.Format("{0:C}", Rapido); } }
        public decimal RapidoPorcentaje { get; set; }
        public decimal Subtotal { get; set; }
        public string SSubtotal { get { return string.Format("{0:C}", Subtotal); } }
        public decimal Descuentos { get; set; }
        public string SDescuentos { get { return string.Format("{0:C}", Descuentos); } }
        public decimal VentaNeta { get; set; }
        public string SVentaNeta { get { return string.Format("{0:C}", VentaNeta); } }
        public decimal ImpuestoTotal
        {
            get
            {
                return ImpuestosVentas.Sum(x => x.impuesto);
            }
        }
        public string SImpuestoTotal { get { return string.Format("{0:C}", ImpuestoTotal); } }
        public decimal VentasConImpuesto { get; set; }
        public string SVentasConImpuesto { get { return string.Format("{0:C}", VentasConImpuesto); } }
        public decimal VentaFacturada { get; set; }
        public string SVentaFacturada { get { return string.Format("{0:C}", VentaFacturada); } }
        public decimal PropinaFacturada { get; set; }
        public string SPropinaFacturada { get { return string.Format("{0:C}", PropinaFacturada); } }
        public decimal Facturado
        {
            get
            {
                return VentaFacturada + PropinaFacturada;
            }
        }
        public string SFacturado { get { return string.Format("{0:C}", Facturado); } }
        public decimal VentaNoFacturada
        {
            get
            {
                return VentasConImpuesto - Facturado;
            }
        }
        public string SVentaNoFacturada { get { return string.Format("{0:C}", VentaNoFacturada); } }
        public int CuentasNormales { get; set; }
        public int CuentasCanceladas { get; set; }
        public int CuentasConDescuento { get; set; }
        public decimal CuentasConDescuentoImporte { get; set; }
        public string SCuentasConDescuentoImporte { get { return string.Format("{0:C}", CuentasConDescuentoImporte); } }
        public int CuentasConCortesia { get; set; }
        public decimal CuentasConCortesiaImporte { get; set; }
        public string SCuentasConCortesiaImporte { get { return string.Format("{0:C}", CuentasConCortesiaImporte); } }
        public decimal CuentaPromedio { get; set; }
        public string SCuentaPromedio { get { return string.Format("{0:C}", CuentaPromedio); } }
        public decimal ConsumoPromedio { get; set; }
        public string SConsumoPromedio { get { return string.Format("{0:C}", ConsumoPromedio); } }
        public int Comensales { get; set; }
        public decimal Propinas { get; set; }
        public string SPropinas { get { return string.Format("{0:C}", Propinas); } }
        public decimal Cargos { get; set; }
        public string SCargos { get { return string.Format("{0:C}", Cargos); } }
        public decimal DescuentoMonedero { get; set; }
        public string SDescuentoMonedero { get { return string.Format("{0:C}", DescuentoMonedero); } }
        public int FolioInicial { get; set; }
        public int FolioFinal { get; set; }
        public decimal CortesiaAlimentos { get; set; }
        public string SCortesiaAlimentos { get { return string.Format("{0:C}", CortesiaAlimentos); } }
        public decimal CortesiaBebidas { get; set; }
        public string SCortesiaBebidas { get { return string.Format("{0:C}", CortesiaBebidas); } }
        public decimal CortesiaOtros { get; set; }
        public string SCortesiaOtros { get { return string.Format("{0:C}", CortesiaOtros); } }
        public decimal TotalCortesias { get; set; }
        public string STotalCortesias { get { return string.Format("{0:C}", TotalCortesias); } }
        public decimal DescuentoAlimentos { get; set; }
        public string SDescuentoAlimentos { get { return string.Format("{0:C}", DescuentoAlimentos); } }
        public decimal DescuentoBebidas { get; set; }
        public string SDescuentoBebidas { get { return string.Format("{0:C}", DescuentoBebidas); } }
        public decimal DescuentoOtros { get; set; }
        public string SDescuentoOtros { get { return string.Format("{0:C}", DescuentoOtros); } }
        public decimal TotalDescuentos { get; set; }
        public string STotalDescuentos { get { return string.Format("{0:C}", TotalDescuentos); } }
        public decimal TotalDeclarado { get; set; }
        public string STotalDeslarado { get { return string.Format("{0:C}", TotalDeclarado); } }
        public decimal SobranteOFaltante
        {
            get
            {
                return TotalDeclarado - SaldoFinal;
            }
        }
        public string SSobrate { get { return SobranteOFaltante > 0 ? string.Format("{0:C}", SobranteOFaltante) : ""; } }
        public string SFaltante { get { return SobranteOFaltante < 0 ? string.Format("{0:C}", SobranteOFaltante * -1) : ""; } }
        public decimal Dolares { get; set; }
        public string SDolares { get { return string.Format("{0:C}", Dolares); } }
        public decimal AcumuladoMesAnterior { get; set; }
        public string SAcumuladoMesAnterior { get { return string.Format("{0:C}", AcumuladoMesAnterior); } }
        public decimal AcumuladoMesActual { get; set; }
        public string SAcumuladoMesActual { get { return string.Format("{0:C}", AcumuladoMesActual); } }

        public List<Pago> Pagos { get; set; }
        public List<ImpuestoVenta> ImpuestosVentas { get; set; }
        public List<PagoTarjeta> PagosTarjeta { get; set; }
        public List<ChequeReporte> ChequesReporte { get; set; }
        public List<ChequeReporte> TotalesChequesReporte { get; set; }
        public List<TurnoReporte> Turnos { get; set; }
        public List<VentaRapida> VentasRapidas { get; set; }

        public bool ConsiderarFondoInicial { get; set; }
        public bool NoConsiderarPropinas { get; set; }
        public bool NoConsiderarDepositosRetiros { get; set; }
    }
}
