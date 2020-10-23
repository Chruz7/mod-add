using mod_add.Datos.Enums;
using mod_add.Datos.Modelos;
using mod_add.Enums;
using mod_add.Utils;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace mod_add.ViewModels
{
    public class AjusteMasivoViewModel : ViewModelBase
    {
        private readonly Random Random;
        public AjusteMasivoViewModel()
        {
            Random = new Random();

            InicializarControles();
        }

        public Respuesta GuardarCambios()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                    var foliosAfectar = DetalleModificacionCheques.Where(x => x.Modificar).Select(x => x.Folio).ToList();
                    var cheqs = Cheqs1.Where(x => foliosAfectar.Contains(x.folio)).ToList();

                    foreach(var cheque in cheqs)
                    {
                        cheques_DAO.Update(cheque);
                        cheqdet_DAO.Delete(cheque.folio);

                        var chequepago = CheqsPago1.Find(x => x.folio == cheque.folio);
                        chequespagos_DAO.Update(chequepago);
                    }

                    var cheqsDet = ReenumerarDetalles();

                    cheqdet_DAO.Create(cheqsDet);

                    return Respuesta.HECHO;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return Respuesta.ERROR;
                }
            }
        }

        public void Proceso()
        {
            try
            {
                Cheqs1 = Funciones.CloneList(Cheqs);
                CheqsDet1 = Funciones.CloneList(CheqsDet);
                CheqsDet2 = Funciones.CloneList(CheqsDet);
                CheqsPago1 = Funciones.CloneList(CheqsPago);
                DetalleAjustes1.Clear();

                EliminarProductos();
                CambiarProductos();
                AjustarCheques();

                NumeroTotalCuentasModificadas = Cheqs1.Count(x => x.Actualizar);
                ImporteNuevo = Cheqs1.Sum(x => x.total.Value);
                Diferencia = ImporteAnterior - ImporteNuevo;
                PorcentajeDiferencia = Math.Round(Diferencia / ImporteAnterior * 100m, 2, MidpointRounding.AwayFromZero);
                EfectivoNuevo = Cheqs1.Sum(x => x.efectivo.Value);

                foreach (var cheq in Cheqs)
                {
                    var cheqActualizado = Cheqs1.Find(x => x.folio == cheq.folio);
                    int totalArticulosAnterior = CheqsDet.Where(x => x.foliodet == cheq.folio).Sum(x => x.CantidadEntera);
                    int totalArticulosNuevo = CheqsDet1.Where(x => x.foliodet == cheq.folio).Sum(x => x.CantidadEntera);

                    DetalleAjustes1.Add(new DetalleAjuste
                    {
                        Folio = cheq.folio,
                        FolioNotaConsumo = cheq.folionotadeconsumo.Value,
                        Fecha = cheq.fecha.Value,
                        Cancelado = cheq.cancelado.Value ? TipoLogico.SI : TipoLogico.NO,
                        Facturado = cheq.facturado.Value ? TipoLogico.SI : TipoLogico.NO,
                        Descuento = cheq.descuento.Value,
                        TotalOriginal = cheq.total.Value,
                        ProductosEliminados = totalArticulosAnterior - totalArticulosNuevo,
                        TotalArticulos = totalArticulosAnterior,
                        TotalConDescuento = cheqActualizado.total.Value,
                        Efectivo = cheqActualizado.efectivo.Value,
                        Tarjeta = cheqActualizado.tarjeta.Value,
                        Vales = cheqActualizado.vales.Value,
                        Otros = cheqActualizado.otros.Value,
                        Modificar = cheqActualizado.Actualizar,
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void AjustarCheques()
        {
            //como se realiza la fecha de busqueda
            foreach(var cheque in Cheqs1)
            {
                #region Ajuste del cheque
                var detalleCheques = CheqsDet1.Where(x => x.foliodet == cheque.folio).ToList();
                decimal totalConImpuestos_Det1 = Mat.Redondeo(detalleCheques.Sum(x => x.precio.Value * x.cantidad.Value * (100m - x.descuento.Value) / 100m));
                decimal totalConImpuestos_Det = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteCICD));
                decimal descuentoAplicado = (100m - cheque.descuento.Value) / 100m;
                decimal totalNuevo = Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);

                if (cheque.total.Value == totalNuevo) continue;

                decimal descuento = cheque.descuento.Value / 100m;
                var chequepago = CheqsPago1.Find(x => x.folio == x.folio);
                int iCheqPag = CheqsPago1.FindIndex(x => x.folio == x.folio);
                var formapago = FormasPago.Find(x => x.idformadepago.Equals(chequepago.idformadepago));
                decimal totalSinImpuestos_Det = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteSICD));

                cheque.Actualizar = true;

                cheque.totalarticulos = detalleCheques.Count;
                cheque.subtotal = totalSinImpuestos_Det;
                cheque.total = totalNuevo;

                cheque.totalconpropina = cheque.total; // falta validar si la propina se agrega por configuracion

                cheque.totalconcargo = cheque.total + cheque.cargo;
                cheque.totalconpropinacargo = cheque.total + cheque.cargo; // falta validar si la propina se agrega por configuracion
                cheque.descuentoimporte = Mat.Redondeo(totalSinImpuestos_Det * descuento);

                if (formapago.tipo == (int)TipoPago.EFECTIVO)
                {
                    cheque.efectivo = cheque.total;
                }
                else if (formapago.tipo == (int)TipoPago.TARJETA)
                {
                    cheque.tarjeta = cheque.total;
                    cheque.propinatarjeta = cheque.propina;
                }
                else if (formapago.tipo == (int)TipoPago.VALES)
                {
                    cheque.vales = cheque.total;
                }
                else if (formapago.tipo == (int)TipoPago.OTROS)
                {
                    cheque.otros = cheque.total;
                }

                cheque.totalsindescuento = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteSISD));

                decimal totalalimentos = 0;
                decimal totalbebidas = 0;
                decimal totalotros = 0;
                decimal totalalimentosdescuento = 0;
                decimal totalbebidasdescuento = 0;
                decimal totalotrosdescuento = 0;
                decimal totalalimentossindescuento = 0;
                decimal totalbebidassindescuento = 0;
                decimal totalotrossindescuento = 0;

                foreach (var detalle in detalleCheques)
                {
                    var producto = detalle.Producto;
                    var grupo = producto.Grupo;

                    decimal importedetalle = detalle.ImporteSICD;
                    decimal importedetallesindescuento = detalle.ImporteSISD;
                    decimal importedetalledescuento = importedetallesindescuento - importedetalle;

                    if (grupo.clasificacion == 1)
                    {
                        totalbebidas += importedetalle;
                        totalbebidassindescuento += importedetallesindescuento;
                        totalbebidasdescuento += importedetalledescuento;
                    }
                    else if (grupo.clasificacion == 2)
                    {
                        totalalimentos += importedetalle;
                        totalalimentossindescuento += importedetallesindescuento;
                        totalalimentosdescuento += importedetalledescuento;
                    }
                    else if (grupo.clasificacion == 3)
                    {
                        totalotros += importedetalle;
                        totalotrossindescuento += importedetallesindescuento;
                        totalotrosdescuento += importedetalledescuento;
                    }
                }

                cheque.totalalimentos = Mat.Redondeo(totalalimentos * descuentoAplicado);
                cheque.totalbebidas = Mat.Redondeo(totalbebidas * descuentoAplicado);
                cheque.totalotros = Mat.Redondeo(totalotros * descuentoAplicado);

                cheque.totaldescuentos = cheque.descuentoimporte;
                cheque.totaldescuentoalimentos = Mat.Redondeo(totalalimentosdescuento * descuento);
                cheque.totaldescuentobebidas = Mat.Redondeo(totalbebidasdescuento * descuento);
                cheque.totaldescuentootros = Mat.Redondeo(totalotrosdescuento * descuento);
                // las cortesias se mantienen?

                cheque.totaldescuentoycortesia = cheque.totaldescuentos + cheque.totalcortesias;
                cheque.totalalimentossindescuentos = totalalimentossindescuento;
                cheque.totalbebidassindescuentos = totalbebidassindescuento;
                cheque.totalotrossindescuentos = totalotrossindescuento;

                cheque.subtotalcondescuento = cheque.subtotal - cheque.descuentoimporte;

                cheque.totalimpuestod1 = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteI1CD));
                cheque.totalimpuestod2 = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteI2CD));
                cheque.totalimpuestod3 = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteI3CD));

                cheque.totalimpuesto1 = cheque.totalimpuestod1;

                cheque.desc_imp_original = cheque.descuentoimporte;

                cheque.cambio = 0; // como se debe ajustar el cambio si es efectivo? - por el momento queda en ceros
                cheque.cambiorepartidor = 0;

                //falta ajustar mas campos del cheque
                #endregion

                #region Ajuste del cheque pago
                CheqsPago1[iCheqPag].importe = cheque.total;

                if (formapago.tipo == (int)TipoPago.TARJETA)
                {
                    CheqsPago1[iCheqPag].propina = cheque.propina;
                }
                #endregion
            }
        }

        public void EliminarProductos()
        {
            bool eliminar = false;

            foreach (var det in CheqsDet1.Where(x => !x.modificador.Value))
            {
                if (CheqsDet2.Where(x => x.foliodet == det.foliodet && !x.modificador.Value).Sum(x => x.CantidadEntera) == App.ConfiguracionSistema.MinProductosCuenta) continue;
                if (App.ConfiguracionSistema.EliminarProductosSeleccionados && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto)) continue;

                int iDet = CheqsDet2.FindIndex(x => x.foliodet == x.foliodet && x.movimiento == det.movimiento);

                if (det.cantidad.Value > 1m)
                {
                    eliminar = true;

                    CheqsDet2[iDet].cantidad -= 1m;

                    if (!string.IsNullOrWhiteSpace(det.idproductocompuesto))
                    {
                        EliminarProductosModificadores(det.idproductocompuesto);
                    }
                }
                else
                {
                    CheqsDet2.RemoveAt(iDet);

                    if (!string.IsNullOrWhiteSpace(det.idproductocompuesto))
                    {
                        EliminarProductosModificadores(det.idproductocompuesto, true);
                    }
                }

                ImporteNuevo = CalculoTotalNuevo();

                if (ImporteNuevo <= ImporteObjetivo) break;
            }

            CheqsDet1 = Funciones.CloneList(CheqsDet2);

            if (ImporteNuevo > ImporteObjetivo && eliminar)
            {
                EliminarProductos();
            }
        }

        public decimal CalculoTotalNuevo()
        {
            decimal importeNuevo = 0;
            foreach (var cheq in Cheqs)
            {
                decimal totalConImpuestos_Det = Mat.Redondeo(CheqsDet2.Where(x => x.foliodet == cheq.folio).Sum(x => x.ImporteCICD));
                decimal descuentoAplicado = (100m - cheq.descuento.Value) / 100m;
                importeNuevo += Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);
            }

            Debug.WriteLine($"ImporteAnterior: {ImporteAnterior}, ImporteObjetivo: {ImporteObjetivo}, ImporteNuevo: {importeNuevo}");
            return importeNuevo;
        }

        public void CambiarProductos()
        {
            if (ImporteNuevo <= ImporteObjetivo) return;

            foreach (var detalle in CheqsDet1.Where(x => !x.modificador.Value))
            {
                int porcentajeAleatorio = Random.Next(1, 100);
                int porcentajeAnterior = 0;

                foreach (var productoReemplazo in App.ProductosReemplazo)
                {
                    if (productoReemplazo.Porcentaje > porcentajeAnterior && productoReemplazo.Porcentaje <= porcentajeAleatorio)
                    {
                        int iDet = CheqsDet2.FindIndex(x => x.foliodet.Value == detalle.foliodet.Value);

                        var detalleProductoCheque = detalle.Producto.Detalle;
                        var detalleProducto = App.SRDetProductosReemplazo.Find(x => x.idproducto == productoReemplazo.Clave);

                        if (detalleProducto.precio < detalle.precio)
                        {
                            //se complica en el caso de decimales porque no se sabe el rendimiento del producto
                            CheqsDet1[iDet].idproducto = detalleProducto.idproducto;
                            CheqsDet1[iDet].precio = detalleProducto.precio;
                            CheqsDet1[iDet].impuesto1 = detalleProducto.impuesto1;
                            CheqsDet1[iDet].impuesto2 = detalleProducto.impuesto2;
                            CheqsDet1[iDet].impuesto3 = detalleProducto.impuesto3;
                            CheqsDet1[iDet].preciosinimpuestos = detalleProducto.preciosinimpuestos;
                            CheqsDet1[iDet].preciocatalogo = detalleProducto.precio;
                            CheqsDet1[iDet].impuestoimporte3 = detalleProducto.impuestoimporte3;
                            CheqsDet1[iDet].idproductocompuesto = "";

                            if (!detalleProducto.idunidad.Equals(detalleProductoCheque.idunidad))
                            {
                                CheqsDet2[iDet].cantidad = 1m;
                            }

                            if (!string.IsNullOrWhiteSpace(detalle.idproductocompuesto))
                            {
                                EliminarProductosModificadores(detalle.idproductocompuesto, true);
                            }
                        }

                        break;
                    }
                }

                ImporteNuevo = CalculoTotalNuevo();

                if (ImporteNuevo <= ImporteObjetivo) break;
            }

            CheqsDet1 = Funciones.CloneList(CheqsDet2);
        }


        public List<SR_cheqdet> ReenumerarDetalles()
        {
            int movimiento = 1;
            long folio = 0;
            var foliosAModificar = DetalleModificacionCheques.Where(x => x.Modificar).Select(x => x.Folio).ToList();
            var cheqsDet = CheqsDet1.Where(x => foliosAModificar.Contains(x.foliodet.Value)).OrderBy(x => x.foliodet.Value).ThenBy(x => x.movimiento.Value).ToList();

            foreach (var detalle in cheqsDet)
            {
                if (folio != detalle.foliodet.Value)
                {
                    folio = detalle.foliodet.Value;
                    movimiento = 1;
                }

                detalle.movimiento = movimiento;
            }

            return cheqsDet;
        }

        private void EliminarProductosModificadores(string idproductocompuesto, bool eliminarTodo = false)
        {
            foreach (var det_Mod in CheqsDet1.Where(x => x.idproductocompuesto.Equals(idproductocompuesto) && x.modificador.Value))
            {
                if (App.ConfiguracionSistema.EliminarProductosSeleccionados && !App.ProductosEliminar.Any(x => x.Clave == det_Mod.idproducto)) continue;

                int iDet = CheqsDet2.FindIndex(x => x.foliodet == det_Mod.foliodet && x.movimiento == det_Mod.movimiento);

                if (det_Mod.cantidad.Value > 1m && !eliminarTodo)
                {
                    CheqsDet2[iDet].cantidad -= 1m;
                }
                else
                {
                    CheqsDet2.RemoveAt(iDet);
                }
            }
        }

        public void InicializarControles()
        {
            DetalleModificacionCheques = new ObservableCollection<DetalleAjuste>();
            DetalleAjustes1 = new List<DetalleAjuste>();
            Cheqs = new List<SR_cheques>();
            Cheqs1 = new List<SR_cheques>();
            CheqsDet = new List<SR_cheqdet>();
            CheqsDet1 = new List<SR_cheqdet>();
            CheqsDet2 = new List<SR_cheqdet>();
            CheqsPago = new List<SR_chequespagos>();
            CheqsPago1 = new List<SR_chequespagos>();
            FormasPago = new List<SR_formasdepago>();

            FechaCorteInicio = DateTime.Today;
            FechaCorteCierre = DateTime.Today;
            Turno = true;
            Periodo = false;
            FechaInicio = DateTime.Today;
            FechaCierre = DateTime.Today;
            CorteInicio = App.SRConfiguracion.CorteInicio;
            CorteCierre = App.SRConfiguracion.CorteCierre;
            HorarioTurno = $"{App.SRConfiguracion.cortezinicio} - {App.SRConfiguracion.cortezinicio}";
            ImporteMinimoAjustable = 0m;
            PorcentajeObjetivo = 1;
            ImporteObjetivo = 0m;
            IncluirCuentaPagadaTarjeta = false;
            IncluirCuentaPagadaVales = false;
            IncluirCuentaPagadaOtros = false;
            IncluirCuentaFacturada = true;
            IncluirCuentaNotaConsumo = true;
            QuitarPropinasManualmente = false;
            NoIncluirCuentasReimpresas = false;
            NumeroTotalCuentas = 0;
            NumeroTotalCuentasModificadas = 0;
            ImporteAnterior = 0m;
            ImporteNuevo = 0m;
            Diferencia = 0m;
            PorcentajeDiferencia = 0m;
            EfectivoAnterior = 0m;
            EfectivoNuevo = 0m;
            EfectivoCaja = 0m;

            FechaInicio = new DateTime(2020, 10, 2);
            PorcentajeObjetivo = 10;
        }

        public Respuesta ObtenerChequesSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    if (Turno)
                    {
                        FechaCorteInicio = FechaInicio.AddSeconds(CorteInicio.TotalSeconds);
                        FechaCorteCierre = FechaInicio.AddSeconds(CorteCierre.TotalSeconds);

                        if (CorteInicio > CorteCierre)
                        {
                            FechaCorteCierre = FechaCorteCierre.AddDays(1);
                        }
                    }
                    else
                    {
                        FechaCorteInicio = FechaInicio.AddSeconds(CorteInicio.TotalSeconds);
                        FechaCorteCierre = FechaCierre.AddSeconds(CorteCierre.TotalSeconds);
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

                    Cheqs = cheques_DAO.Get(queryWhere, new object[] {
                        new SqlParameter("FechaInicio", FechaCorteInicio),
                        new SqlParameter("FechaCierre", FechaCorteCierre),
                        new SqlParameter("total", ImporteMinimoAjustable),
                    });

                    if (Cheqs.Count == 0)
                    {
                        return Respuesta.CHEQUE_NO_ENCONTRADO;
                    }

                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    var detalleCheques = cheqdet_DAO.WhereIn("foliodet", Cheqs.Select(x => (object)x.folio).ToArray());

                    CheqsDet = detalleCheques.OrderBy(x => x.movimiento).ThenBy(x => x.foliodet).ToList();

                    SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    CheqsPago = chequespagos_DAO.WhereIn("folio", Cheqs.Select(x => (object)x.folio).ToArray());

                    SR_formasdepago_DAO formasdepago_DAO = new SR_formasdepago_DAO(context);
                    FormasPago = formasdepago_DAO.GetAll();

                    RefrescarControles();

                    return Respuesta.HECHO;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return Respuesta.ERROR;
                }
            }
        }

        public void CargarResultados()
        {
            DetalleModificacionCheques = new ObservableCollection<DetalleAjuste>(Funciones.CloneList(DetalleAjustes1));
        }

        public void RefrescarControles()
        {
            decimal total = Cheqs.Sum(x => x.total.Value);

            ImporteObjetivo = total * (100m - PorcentajeObjetivo) / 100m;
            NumeroTotalCuentas = Cheqs.Count;
            ImporteAnterior = total;
            EfectivoAnterior = Cheqs.Sum(x => x.efectivo.Value);

            NumeroTotalCuentasModificadas = 0;
            ImporteNuevo = 0m;
            Diferencia = 0m;
            PorcentajeDiferencia = 0m;
            EfectivoNuevo = 0m;
        }

        public void ResgistrarBitacora()
        {
            Funciones.RegistrarModificacion(new BitacoraModificacion
            {
                FechaProceso = DateTime.Now,
                FechaInicialVenta = FechaCorteInicio,
                FechaFinalVenta = FechaCorteCierre,
                TotalCuentas = NumeroTotalCuentas,
                CuentasModificadas = NumeroTotalCuentasModificadas,
                ImporteAnterior = ImporteAnterior,
                ImporteNuevo = ImporteNuevo,
                Diferencia = Diferencia,
            });
        }
        public DateTime FechaCorteInicio { get; set; }
        public DateTime FechaCorteCierre { get; set; }
        private List<SR_cheques> Cheqs { get; set; }
        private List<SR_cheques> Cheqs1 { get; set; }
        private List<SR_cheqdet> CheqsDet { get; set; }
        private List<SR_cheqdet> CheqsDet1 { get; set; }
        private List<SR_cheqdet> CheqsDet2 { get; set; }
        private List<SR_chequespagos> CheqsPago { get; set; }
        private List<SR_chequespagos> CheqsPago1 { get; set; }
        private List<SR_formasdepago> FormasPago { get; set; }
        private List<DetalleAjuste> DetalleAjustes1 { get; set; }
        private TimeSpan CorteInicio { get; set; }
        private TimeSpan CorteCierre { get; set; }

        private ObservableCollection<DetalleAjuste> _DetalleModificacionCheques;
        public ObservableCollection<DetalleAjuste> DetalleModificacionCheques
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

        private decimal _ImporteMinimoAjustable;
        public decimal ImporteMinimoAjustable
        {
            get { return _ImporteMinimoAjustable; }
            set
            {
                _ImporteMinimoAjustable = value;
                OnPropertyChanged(nameof(ImporteMinimoAjustable));
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
