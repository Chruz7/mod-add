using mod_add.Datos.Modelos;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;

namespace mod_add.ViewModels
{
    public class AjusteMasivoViewModel : ViewModelBase
    {
        private readonly Random Random;
        public AjusteMasivoViewModel()
        {
            InicializarControles();
            Random = new Random();
        }

        public void AjustarCheques()
        {
            decimal totalPrecioSustraendo = 0;

            foreach (var detalleMod in DetalleModificacionCheques.OrderBy(x => x.Folio))
            {
                var cheque = Cheques.Where(x => x.folio == detalleMod.Folio).FirstOrDefault();
                var detallesCheque = DetalleCheques.Where(x => x.foliodet == detalleMod.Folio).ToList();
                var chequepago = ChequesPagos.Where(x => x.folio == detalleMod.Folio).FirstOrDefault();
                var formapago = FormasPago.Where(x => x.idformadepago == chequepago.idformadepago).FirstOrDefault();

                decimal totalPrecio = Math.Round(detallesCheque.Sum(x => x.precio ?? 0 * x.cantidad ?? 0), 4, App.MidpointRounding);

                foreach (var detalle in detallesCheque)
                {
                    int iDet = DetalleCheques.IndexOf(detalle);

                    if (App.ProductosEliminar.Any(x => x.Clave == detalle.idproducto))
                    {
                        if (detalle.cantidad > 1)
                        {
                            //que se debe hacer en el caso de los productos con cantidad decimales? 
                            //al eliminar un producto con mod, los mod tambien se eliminan? si
                            //si cambia la cantidad del producto padre, la cantidad de los mod tambien cambia? si


                            DetalleCheques[iDet].cantidad -= 1m;
                        }
                        else
                        {
                            DetalleCheques.Remove(detalle);
                        }

                        totalPrecioSustraendo += detalle.precio ?? 0;
                    }
                    else
                    {
                        int porcentajeAleatorio = Random.Next(1, 100);
                        int porcentajeAnterior = 0;

                        foreach (var productoReemplazo in App.ProductosReemplazo)
                        {
                            if (productoReemplazo.Porcentaje > porcentajeAnterior && productoReemplazo.Porcentaje <= porcentajeAleatorio)
                            {
                                //al cambiar el producto se conserva la cantidad?
                                //puede cambiar un producto con modificadores? xq?

                                var producto = ObtenerProductoSR(productoReemplazo.Clave);
                                var detalleProducto = producto.Detalle;

                                if (detalleProducto.precio < detalle.precio)
                                {
                                    DetalleCheques[iDet].precio = detalleProducto.precio;
                                    DetalleCheques[iDet].impuesto1 = detalleProducto.impuesto1;
                                    DetalleCheques[iDet].impuesto2 = detalleProducto.impuesto2;
                                    DetalleCheques[iDet].impuesto3 = detalleProducto.impuesto3;
                                    DetalleCheques[iDet].preciosinimpuestos = detalleProducto.preciosinimpuestos;
                                    DetalleCheques[iDet].preciocatalogo = detalleProducto.precio;
                                    DetalleCheques[iDet].impuestoimporte3 = detalleProducto.impuestoimporte3;

                                    totalPrecioSustraendo += (detalle.precio ?? 0 * detalle.cantidad ?? 0);
                                    totalPrecioSustraendo -= (detalleProducto.precio ?? 0 * detalle.cantidad ?? 0);
                                }

                                break;
                            }
                        }
                    }

                    if ((ImporteAnterior - totalPrecioSustraendo) < ImporteAjuste)
                    {
                        break;
                    }
                }
            }



            
        }

        public void EliminarProductos()
        {
            decimal totalPrecioSustraendo = 0;
            bool eliminar = false;

            foreach (var detalle in DetalleCheques)
            {
                if (App.ProductosEliminar.Any(x => x.Clave == detalle.idproducto))
                {
                    int iDet = DetalleChequesCopia.IndexOf(detalle);

                    if (detalle.cantidad > 1)
                    {
                        eliminar = true;
                        DetalleChequesCopia[iDet].cantidad -= 1m;
                    }
                    else
                    {
                        DetalleChequesCopia.Remove(detalle);
                    }

                    totalPrecioSustraendo += detalle.precio ?? 0;
                }

                if ((ImporteAnterior - totalPrecioSustraendo) <= ImporteAjuste)
                {
                    break;
                }
            }

            DetalleCheques = (DetalleChequesCopia as IEnumerable<SR_cheqdet>).ToList();

            if ((ImporteAnterior - totalPrecioSustraendo) > ImporteAjuste && eliminar)
            {
                EliminarProductos();
            }
        }

        public void InicializarControles()
        {
            DetalleModificacionCheques = new ObservableCollection<DetalleModificacionCheque>();
            Cheques = new List<SR_cheques>();
            DetalleCheques = new List<SR_cheqdet>();
            DetalleChequesCopia = new List<SR_cheqdet>();
            ChequesPagos = new List<SR_chequespagos>();
            FormasPago = new List<SR_formasdepago>();

            Turno = true;
            Periodo = false;
            FechaInicio = DateTime.Today;
            FechaCierre = DateTime.Today;
            HorarioTurno = "? - ?";
            ImporteAjuste = 0.00m;
            PorcentajeObjetivo = 1;
            ImporteObjetivo = 0.00m;
            IncluirCuentaPagadaTarjeta = false;
            IncluirCuentaPagadaVales = false;
            IncluirCuentaPagadaOtros = false;
            IncluirCuentaFacturada = true;
            IncluirCuentaNotaConsumo = true;
            QuitarPropinasManualmente = false;
            NoIncluirCuentasReimpresas = false;
            NumeroTotalCuentas = 0;
            NumeroTotalCuentasModificadas = 0;
            ImporteAnterior = 0.00m;
            ImporteNuevo = 0.00m;
            Diferencia = 0.00m;
            PorcentajeDiferencia = 0.00m;
            EfectivoAnterior = 0.00m;
            EfectivoNuevo = 0.00m;
            EfectivoCaja = 0.00m;

            ObtenerConfiguracionSR();
        }

        public void ObtenerConfiguracionSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_configuracion_DAO configuracion_DAO = new SR_configuracion_DAO(context);
                    SRConfiguracion = configuracion_DAO.GetAll().FirstOrDefault();

                    DateTime.TryParse(SRConfiguracion.cortezinicio, out DateTime cortezinicio);
                    HoraInicio = cortezinicio.TimeOfDay;

                    DateTime.TryParse(SRConfiguracion.cortezfin, out DateTime cortezfin);
                    HoraCierre = cortezfin.TimeOfDay;

                    HorarioTurno = $"{SRConfiguracion.cortezinicio} - {SRConfiguracion.cortezfin}";
                }
                catch
                {

                }
            }
        }

        public SR_productos ObtenerProductoSR(string idproducto)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_productos_DAO productos_DAO = new SR_productos_DAO(context);

                return productos_DAO.Find(idproducto);
            }
        }

        public void ObtenerInformacionSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                DateTime fechaInicio;
                DateTime fechaCierre;

                if (Turno)
                {
                    fechaInicio = FechaInicio.AddSeconds(HoraInicio.TotalSeconds);
                    fechaCierre = FechaInicio.AddSeconds(HoraCierre.TotalSeconds);
                }
                else
                {
                    fechaInicio = FechaInicio.AddSeconds(HoraInicio.TotalSeconds);
                    fechaCierre = FechaCierre.AddSeconds(HoraCierre.TotalSeconds);
                }

                SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                string queryWhere = "(efectivo >= total";

                if (IncluirCuentaPagadaTarjeta)
                    queryWhere += " OR tarjeta = (total + propinatarjeta)";

                if (IncluirCuentaPagadaVales)
                    queryWhere += " OR vales = total";

                if (IncluirCuentaPagadaOtros)
                    queryWhere += " OR otros = total";

                queryWhere += ") AND ";

                if (!IncluirCuentaFacturada)
                    queryWhere += "facturado = 0 AND ";

                if (!IncluirCuentaNotaConsumo)
                    queryWhere += "folionotadeconsumo = 0 AND ";

                if (NoIncluirCuentasReimpresas)
                    queryWhere += "impresiones = 1 AND ";

                queryWhere += "total >= @total AND cancelado = 0 AND fecha BETWEEN @FechaInicio AND @FechaCierre";

                var cheques = cheques_DAO.Get(queryWhere, new object[] {
                    new SqlParameter("FechaInicio", fechaInicio),
                    new SqlParameter("FechaCierre", fechaCierre),
                    new SqlParameter("total", ImporteAjuste),
                });

                //Validar los tipos de cheque
                //foreach(var cheque in cheques)
                //{
                //    var chequespagos = cheque.Chequespagos;

                //    if (chequespagos.Count == 1)
                //    {
                //        var detalleCheque = cheque.Detalles;
                //        var chequepago = chequespagos[0];
                //        var formapago = chequepago.Formasdepago;

                //        Cheques.Add(cheque);
                //        DetalleCheques.AddRange(detalleCheque);
                //        ChequesPagos.Add(chequepago);

                //        if (!FormasPago.Any(x => x.idformadepago == formapago.idformadepago))
                //            FormasPago.Add(formapago);

                //        DetalleModificacionCheques.Add(new DetalleModificacionCheque
                //        {
                //            Folio = cheque.folio,
                //            FolioNotaConsumo = cheque.folionotadeconsumo.Value,
                //            Fecha = cheque.fecha.Value,
                //            Cancelado = cheque.cancelado.Value,
                //            Facturado = cheque.facturado.Value,
                //            Descuento = cheque.descuento.Value,
                //            TotalOriginal = cheque.total.Value,
                //            ProductosEliminados = 0,
                //            TotalConDescuento = cheque.total.Value,
                //            Efectivo = cheque.efectivo.Value,
                //            Tarjeta = cheque.tarjeta.Value,
                //            Vales = cheque.vales.Value,
                //            Otros = cheque.otros.Value,
                //            Modificar = true,
                //        });
                //    }
                //}

                if (cheques.Count > 0)
                {
                    Cheques = cheques;

                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    var detalleCheques = cheqdet_DAO.WhereIn("foliodet", cheques.Select(x => (object)x.folio).ToArray());

                    //DetalleCheques = detalleCheques.OrderBy(x => x.foliodet).ThenBy(x => x.movimiento).ToList();
                    DetalleCheques = detalleCheques.OrderBy(x => x.movimiento).ThenBy(x => x.foliodet).ToList();
                    DetalleChequesCopia = (DetalleCheques as IEnumerable<SR_cheqdet>).ToList();

                    SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    ChequesPagos = chequespagos_DAO.WhereIn("folio", cheques.Select(x => (object)x.folio).ToArray());

                    SR_formasdepago_DAO formasdepago_DAO = new SR_formasdepago_DAO(context);
                    FormasPago = formasdepago_DAO.GetAll();
                }

                foreach (var cheque in Cheques)
                {
                    DetalleModificacionCheques.Add(new DetalleModificacionCheque
                    {
                        Folio = cheque.folio,
                        FolioNotaConsumo = cheque.folionotadeconsumo.Value,
                        Fecha = cheque.fecha.Value,
                        Cancelado = cheque.cancelado.Value,
                        Facturado = cheque.facturado.Value,
                        Descuento = cheque.descuento.Value,
                        TotalOriginal = cheque.total.Value,
                        ProductosEliminados = 0,
                        TotalConDescuento = cheque.total.Value,
                        Efectivo = cheque.efectivo.Value,
                        Tarjeta = cheque.tarjeta.Value,
                        Vales = cheque.vales.Value,
                        Otros = cheque.otros.Value,
                        Modificar = true,
                    });
                }

                decimal total = cheques.Sum(x => x.total ?? 0);

                ImporteObjetivo = total - (total * (PorcentajeObjetivo / 100m));

                NumeroTotalCuentas = cheques.Count;
                NumeroTotalCuentasModificadas = cheques.Count;
                ImporteAnterior = total;
            }
        }

        private SR_configuracion SRConfiguracion { get; set; }
        private List<SR_cheques> Cheques { get; set; }
        private List<SR_cheqdet> DetalleCheques { get; set; }
        private List<SR_cheqdet> DetalleChequesCopia { get; set; }
        private List<SR_chequespagos> ChequesPagos { get; set; }
        private List<SR_formasdepago> FormasPago { get; set; }
        private TimeSpan HoraInicio { get; set; }
        private TimeSpan HoraCierre { get; set; }

        private ObservableCollection<DetalleModificacionCheque> _DetalleModificacionCheques;
        public ObservableCollection<DetalleModificacionCheque> DetalleModificacionCheques
        {
            get { return _DetalleModificacionCheques; }
            set
            {
                _DetalleModificacionCheques = value;
                OnPropertyChanged(nameof(DetalleModificacionCheques));
            }
        }

        private bool _Turno;
        public bool Turno
        {
            get { return _Turno; }
            set
            {
                _Turno = value;
                OnPropertyChanged(nameof(Turno));
            }
        }

        private bool _Periodo;
        public bool Periodo
        {
            get { return _Periodo; }
            set
            {
                _Periodo = value;
                OnPropertyChanged(nameof(Periodo));
            }
        }

        private DateTime _FechaInicio;
        public DateTime FechaInicio
        {
            get { return _FechaInicio; }
            set
            {
                _FechaInicio = value;
                OnPropertyChanged(nameof(FechaInicio));
            }
        }

        private DateTime _FechaCierre;
        public DateTime FechaCierre
        {
            get { return _FechaCierre; }
            set
            {
                _FechaCierre = value;
                OnPropertyChanged(nameof(FechaCierre));
            }
        }

        private string _HorarioTurno;
        public string HorarioTurno
        {
            get { return _HorarioTurno; }
            set
            {
                _HorarioTurno = value;
                OnPropertyChanged(nameof(HorarioTurno));
            }
        }

        private decimal _ImporteAjuste;
        public decimal ImporteAjuste
        {
            get { return _ImporteAjuste; }
            set
            {
                _ImporteAjuste = value;
                OnPropertyChanged(nameof(ImporteAjuste));
            }
        }

        private decimal _PocentajeObjetivo;
        public decimal PorcentajeObjetivo
        {
            get { return _PocentajeObjetivo; }
            set
            {
                _PocentajeObjetivo = value;
                OnPropertyChanged(nameof(PorcentajeObjetivo));
            }
        }

        private decimal _ImporteObjetivo;
        public decimal ImporteObjetivo
        {
            get { return _ImporteObjetivo; }
            set
            {
                _ImporteObjetivo = value;
                OnPropertyChanged(nameof(ImporteObjetivo));
            }
        }

        private bool _IncluirCuentaPagadaTarjeta;
        public bool IncluirCuentaPagadaTarjeta
        {
            get { return _IncluirCuentaPagadaTarjeta; }
            set
            {
                _IncluirCuentaPagadaTarjeta = value;
                OnPropertyChanged(nameof(IncluirCuentaPagadaTarjeta));
            }
        }

        private bool _IncluirCuentaPagadaVales;
        public bool IncluirCuentaPagadaVales
        {
            get { return _IncluirCuentaPagadaVales; }
            set
            {
                _IncluirCuentaPagadaVales = value;
                OnPropertyChanged(nameof(IncluirCuentaPagadaVales));
            }
        }

        private bool _CuentaPagadaOtros;
        public bool IncluirCuentaPagadaOtros
        {
            get { return _CuentaPagadaOtros; }
            set
            {
                _CuentaPagadaOtros = value;
                OnPropertyChanged(nameof(IncluirCuentaPagadaOtros));
            }
        }

        private bool _IncluirCuentaFacturada;
        public bool IncluirCuentaFacturada
        {
            get { return _IncluirCuentaFacturada; }
            set
            {
                _IncluirCuentaFacturada = value;
                OnPropertyChanged(nameof(IncluirCuentaFacturada));
            }
        }

        private bool _IncluirCuentaNotaConsumo;
        public bool IncluirCuentaNotaConsumo
        {
            get { return _IncluirCuentaNotaConsumo; }
            set
            {
                _IncluirCuentaNotaConsumo = value;
                OnPropertyChanged(nameof(IncluirCuentaNotaConsumo));
            }
        }

        private bool _QuitarPropinasManualmente;
        public bool QuitarPropinasManualmente
        {
            get { return _QuitarPropinasManualmente; }
            set
            {
                _QuitarPropinasManualmente = value;
                OnPropertyChanged(nameof(QuitarPropinasManualmente));
            }
        }

        private bool _NoIncluirCuentasReimpresas;
        public bool NoIncluirCuentasReimpresas
        {
            get { return _NoIncluirCuentasReimpresas; }
            set
            {
                _NoIncluirCuentasReimpresas = value;
                OnPropertyChanged(nameof(NoIncluirCuentasReimpresas));
            }
        }

        private int _NumeroTotalCuentas;
        public int NumeroTotalCuentas
        {
            get { return _NumeroTotalCuentas; }
            set
            {
                _NumeroTotalCuentas = value;
                OnPropertyChanged(nameof(NumeroTotalCuentas));
            }
        }

        private int _NumeroTotalCuentasModificadas;
        public int NumeroTotalCuentasModificadas
        {
            get { return _NumeroTotalCuentasModificadas; }
            set
            {
                _NumeroTotalCuentasModificadas = value;
                OnPropertyChanged(nameof(NumeroTotalCuentasModificadas));
            }
        }

        private decimal _ImporteAnterior;
        public decimal ImporteAnterior
        {
            get { return _ImporteAnterior; }
            set
            {
                _ImporteAnterior = value;
                OnPropertyChanged(nameof(ImporteAnterior));
            }
        }

        private decimal _ImporteNuevo;
        public decimal ImporteNuevo
        {
            get { return _ImporteNuevo; }
            set
            {
                _ImporteNuevo = value;
                OnPropertyChanged(nameof(ImporteNuevo));
            }
        }

        private decimal _Diferencia;
        public decimal Diferencia
        {
            get { return _Diferencia; }
            set
            {
                _Diferencia = value;
                OnPropertyChanged(nameof(Diferencia));
            }
        }

        private decimal _PorcentajeDiferencia;
        public decimal PorcentajeDiferencia
        {
            get { return _PorcentajeDiferencia; }
            set
            {
                _PorcentajeDiferencia = value;
                OnPropertyChanged(nameof(PorcentajeDiferencia));
            }
        }

        private decimal _EfectivoAnterior;
        public decimal EfectivoAnterior
        {
            get { return _EfectivoAnterior; }
            set
            {
                _EfectivoAnterior = value;
                OnPropertyChanged(nameof(EfectivoAnterior));
            }
        }

        private decimal _EfectivoNuevo;
        public decimal EfectivoNuevo
        {
            get { return _EfectivoNuevo; }
            set
            {
                _EfectivoNuevo = value;
                OnPropertyChanged(nameof(EfectivoNuevo));
            }
        }

        private decimal _EfecitivoCaja;
        public decimal EfectivoCaja
        {
            get { return _EfecitivoCaja; }
            set
            {
                _EfecitivoCaja = value;
                OnPropertyChanged(nameof(EfectivoCaja));
            }
        }
    }
}
