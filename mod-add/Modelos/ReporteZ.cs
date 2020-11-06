using System;
using System.Collections.Generic;
using System.Linq;

namespace mod_add.Modelos
{
    public class ReporteZ
    {
        public ReporteZ()
        {
            Pagos = new List<Pago>();
        }
        public string TituloCorteZ { get; set; }
        public long FolioCorte { get; set; }
        public DateTime FechaCorteInicio { get; set; }
        public DateTime FechaCorteCierre { get; set; }
        public decimal EfectivoInicial { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Tarjeta { get; set; }
        public decimal Vales { get; set; }
        public decimal Otros { get; set; }
        public decimal DepositosEfectivo { get; set; }
        public decimal RetirosEfectivo { get; set; }
        public decimal PropinasPagadas { get; set; }
        public decimal SaldoFinal
        {
            get
            {
                return EfectivoInicial + Efectivo + Tarjeta + Vales + Otros + DepositosEfectivo - RetirosEfectivo - PropinasPagadas;
            }
        }
        public decimal EfectivoFinal
        {
            get
            {
                return EfectivoInicial + Efectivo - PropinasPagadas;
            }
        }
        public decimal TotalFormasPagoVentas
        {
            get
            {
                return Pagos.Sum(x => x.importe);
            }
        }
        public decimal TotalFormasPagoPropinas
        {
            get
            {
                return Pagos.Sum(x => x.propina);
            }
        }
        public decimal PAlimentos { get; set; }
        public decimal PCantidadAlimentos { get; set; }
        public decimal PPorcentajeAlimentos { get; set; }
        public decimal PBebidas { get; set; }
        public decimal PCantidadBebidas { get; set; }
        public decimal PPorcentajeBebidas { get; set; }
        public decimal POtros { get; set; }
        public decimal PCantidadOtros { get; set; }
        public decimal PPorcentajeOtros { get; set; }
        public decimal Comedor { get; set; }
        public decimal ComedorPorcentaje { get; set; }
        public decimal Domicilio { get; set; }
        public decimal DomicilioPorcentaje { get; set; }
        public decimal Rapido { get; set; }
        public decimal RapidoPorcentaje { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuentos { get; set; }
        public decimal VentaNeta { get; set; }
        //lista de venta e impuesto
        public decimal ImpuestoTotal { get; set; }
        public decimal VentasConImpuesto { get; set; }
        public decimal VentaFacturada { get; set; }
        public decimal PropinaFacturada { get; set; }
        public decimal Facturado { get; set; }
        public decimal VentaNoFacturada { get; set; }
        public int CuentasNormales { get; set; }
        public int CuentasCanceladas { get; set; }
        public int CuentasConDescuento { get; set; }
        public int CuentasConCortesia { get; set; }
        public decimal CuentaPromedio { get; set; }
        public decimal ConsumoPromedio { get; set; }
        public int Comensales { get; set; }
        public decimal Propinas { get; set; }
        public decimal Cargos { get; set; }
        public decimal DescuentoMonedero { get; set; }
        public long FolioInicial { get; set; }
        public long FolioFinal { get; set; }
        public decimal CortesiaAlimentos { get; set; }
        public decimal CortesiaBebidas { get; set; }
        public decimal CortesiaOtros { get; set; }
        public decimal TotalCortesias { get; set; }
        public decimal DescuentoAlimentos { get; set; }
        public decimal DescuentoBebidas { get; set; }
        public decimal DescuentoOtros { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal TotalDeclarado { get; set; }
        public decimal SobranteOFaltante { get; set; }
        public decimal AcumuladoMesAnterior { get; set; }
        public decimal AcumuladoMesActual { get; set; }
        public List<Pago> Pagos { get; set; }
    }
}
