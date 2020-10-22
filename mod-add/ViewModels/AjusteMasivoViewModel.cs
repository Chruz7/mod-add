using mod_add.Datos.Modelos;
using mod_add.Enums;
using mod_add.Utils;
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

        public void AjustarCheques2()
        {
            decimal totalPrecioSustraendo = 0;

            foreach (var detalleMod in DetalleModificacionCheques.OrderBy(x => x.Folio))
            {
                var cheque = Cheqs1.Where(x => x.folio == detalleMod.Folio).FirstOrDefault();
                var detallesCheque = CheqsDet1.Where(x => x.foliodet == detalleMod.Folio).ToList();
                var chequepago = CheqsPago1.Where(x => x.folio == detalleMod.Folio).FirstOrDefault();
                var formapago = FormasPago.Where(x => x.idformadepago == chequepago.idformadepago).FirstOrDefault();

                decimal totalPrecio = Math.Round(detallesCheque.Sum(x => x.precio ?? 0 * x.cantidad ?? 0), 4, App.MidpointRounding);

                foreach (var detalle in detallesCheque)
                {
                    int iDet = CheqsDet1.IndexOf(detalle);

                    if (App.ProductosEliminar.Any(x => x.Clave == detalle.idproducto))
                    {
                        if (detalle.cantidad > 1)
                        {
                            //que se debe hacer en el caso de los productos con cantidad decimales? 
                            //al eliminar un producto con mod, los mod tambien se eliminan? si
                            //si cambia la cantidad del producto padre, la cantidad de los mod tambien cambia? si


                            CheqsDet1[iDet].cantidad -= 1m;
                        }
                        else
                        {
                            CheqsDet1.Remove(detalle);
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
                                    CheqsDet1[iDet].precio = detalleProducto.precio;
                                    CheqsDet1[iDet].impuesto1 = detalleProducto.impuesto1;
                                    CheqsDet1[iDet].impuesto2 = detalleProducto.impuesto2;
                                    CheqsDet1[iDet].impuesto3 = detalleProducto.impuesto3;
                                    CheqsDet1[iDet].preciosinimpuestos = detalleProducto.preciosinimpuestos;
                                    CheqsDet1[iDet].preciocatalogo = detalleProducto.precio;
                                    CheqsDet1[iDet].impuestoimporte3 = detalleProducto.impuestoimporte3;

                                    totalPrecioSustraendo += (detalle.precio ?? 0 * detalle.cantidad ?? 0);
                                    totalPrecioSustraendo -= (detalleProducto.precio ?? 0 * detalle.cantidad ?? 0);
                                }

                                break;
                            }
                        }
                    }

                    if ((ImporteAnterior - totalPrecioSustraendo) < ImporteMinimoAjustable)
                    {
                        break;
                    }
                }
            }
        }

        public void AjustarCheques()
        {
            //como funciona el checkbox de Modificar
            //como se realiza la fecha de busqueda
            //como se aplica el cambio y el eliminar -flujo del proceso
            foreach(var cheque in Cheqs1)
            {
                #region Ajuste del cheque
                var detalleCheques = CheqsDet1.Where(x => x.foliodet == cheque.folio).ToList();
                decimal totalConImpuestos_Det1 = Mat.Redondeo(detalleCheques.Sum(x => x.precio.Value * x.cantidad.Value * (100m - x.descuento.Value) / 100m));
                decimal totalConImpuestos_Det = Mat.Redondeo(detalleCheques.Sum(x => x.ImporteCICD));
                decimal descuentoAplicado = (100m - cheque.descuento.Value) / 100m;
                decimal totalNuevo = Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);

                if (cheque.total.Value == totalNuevo)
                    continue;
                else
                {
                    long folio = cheque.folio;
                    Console.WriteLine(folio.ToString());
                }

                decimal descuento = cheque.descuento.Value / 100m;
                var detalleModicacionCheque = DetalleModificacionCheques.FirstOrDefault(x => x.Folio == cheque.folio);
                int iDetModCheq = DetalleModificacionCheques.IndexOf(detalleModicacionCheque);
                var chequepago = CheqsPago1.Find(x => x.folio == x.folio);
                int iCheqPag = CheqsPago1.IndexOf(chequepago);
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

                DetalleModificacionCheques[iDetModCheq].TotalConDescuento = totalNuevo;
                DetalleModificacionCheques[iDetModCheq].Efectivo = cheque.efectivo.Value;
                DetalleModificacionCheques[iDetModCheq].Tarjeta = cheque.tarjeta.Value;
                DetalleModificacionCheques[iDetModCheq].Vales = cheque.vales.Value;
                DetalleModificacionCheques[iDetModCheq].Otros = cheque.otros.Value;
                DetalleModificacionCheques[iDetModCheq].ProductosEliminados = detalleModicacionCheque.TotalArticulos - (int)cheque.totalarticulos.Value;
            }

            NumeroTotalCuentasModificadas = Cheqs1.Count(x => x.Actualizar);
            ImporteNuevo = Cheqs1.Sum(x => x.total.Value);
            Diferencia = ImporteAnterior - ImporteNuevo;
            PorcentajeDiferencia = Diferencia / ImporteAnterior * 100m;
            EfectivoNuevo = Cheqs1.Sum(x => x.efectivo.Value);
        }

        public void EliminarProductos()
        {
            decimal totalPrecioSustraendo = 0;
            bool eliminar = false;

            foreach (var detalle in CheqsDet1.Where(x => !x.modificador.Value))
            {
                if (!App.ConfiguracionSistema.EliminarProductosSeleccionados || App.ProductosEliminar.Any(x => x.Clave == detalle.idproducto))
                {
                    var cheq = Cheqs.Find(x => x.folio == detalle.foliodet);
                    decimal descuentoCheque = (100m - cheq.descuento.Value) / 100m;

                    int iDet = CheqsDet2.IndexOf(detalle);
                    decimal cantidadDescontar = 0;

                    if (detalle.cantidad.Value > 1m)
                    {
                        eliminar = true;
                        cantidadDescontar = 1m;

                        CheqsDet2[iDet].cantidad -= cantidadDescontar;

                        if (!string.IsNullOrWhiteSpace(detalle.idproductocompuesto))
                        {
                            var result = EliminarProductosModificadores(detalle.idproductocompuesto, descuentoCheque);
                            totalPrecioSustraendo += result;
                        }
                    }
                    else
                    {
                        cantidadDescontar = detalle.cantidad.Value;

                        CheqsDet2.Remove(detalle);

                        if (!string.IsNullOrWhiteSpace(detalle.idproductocompuesto))
                        {
                            var result = EliminarProductosModificadores(detalle.idproductocompuesto, descuentoCheque, true);
                            totalPrecioSustraendo += result;
                        }
                    }

                    totalPrecioSustraendo += detalle.precio.Value * cantidadDescontar * (100m - detalle.descuento.Value) / 100m * descuentoCheque;
                }

                if ((ImporteAnterior - totalPrecioSustraendo) <= ImporteObjetivo)
                {
                    break;
                }
            }

            CheqsDet1 = (CheqsDet2 as IEnumerable<SR_cheqdet>).ToList();

            if ((ImporteAnterior - totalPrecioSustraendo) > ImporteObjetivo && eliminar)
            {
                EliminarProductos();
            }
        }

        public void CambiarProductos()
        {
            decimal totalPrecioSustraendo = 0;

            foreach (var detalle in CheqsDet1.Where(x => !x.modificador.Value))
            {
                int porcentajeAleatorio = Random.Next(1, 100);
                int porcentajeAnterior = 0;

                foreach (var productoReemplazo in App.ProductosReemplazo)
                {
                    if (productoReemplazo.Porcentaje > porcentajeAnterior && productoReemplazo.Porcentaje <= porcentajeAleatorio)
                    {
                        int iDet = CheqsDet2.IndexOf(detalle);

                        var detalleProductoCheque = detalle.Producto.Detalle;
                        var producto = ObtenerProductoSR(productoReemplazo.Clave);
                        var detalleProducto = producto.Detalle;

                        if (detalleProducto.precio < detalle.precio)
                        {
                            var cheq = Cheqs.Find(x => x.folio == detalle.foliodet);
                            decimal descuentoCheque = (100m - cheq.descuento.Value) / 100m;
                            //se complica en el caso de decimales porque no se sabe el rendimiento del producto

                            CheqsDet1[iDet].idproducto = producto.idproducto;
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
                                totalPrecioSustraendo += EliminarProductosModificadores(detalle.idproductocompuesto, descuentoCheque, true);
                            }

                            totalPrecioSustraendo += (detalle.ImporteCICD);
                            totalPrecioSustraendo -= (CheqsDet2[iDet].ImporteCICD);
                        }

                        break;
                    }
                }

                if ((ImporteAnterior - totalPrecioSustraendo) <= ImporteObjetivo)
                {
                    break;
                }
            }

            CheqsDet1 = (CheqsDet2 as IEnumerable<SR_cheqdet>).ToList();
        }

        public void ReordenarDetalles()
        {
            int movimiento = 1;
            long folio = 0;
            CheqsDet1 = CheqsDet1.OrderBy(x => x.foliodet).ThenBy(x => x.movimiento).ToList();

            foreach (var detalle in CheqsDet1)
            {
                if (folio != detalle.foliodet)
                {
                    folio = detalle.foliodet ?? 0;
                    movimiento = 1;
                }

                detalle.movimiento = movimiento;
            }
        }

        public decimal EliminarProductosModificadores(string idproductocompuesto, decimal descuentoCheque, bool eliminarTodo = false)
        {
            decimal totaldescontar = 0;
            var cheqDet_Mod = CheqsDet2.Where(x => x.idproductocompuesto.Equals(idproductocompuesto) && x.modificador.Value).ToList();

            foreach (var det_Mod in cheqDet_Mod)
            {
                if (!App.ConfiguracionSistema.EliminarProductosSeleccionados || App.ProductosEliminar.Any(x => x.Clave == det_Mod.idproducto) || eliminarTodo)
                {
                    int iDet = CheqsDet2.IndexOf(det_Mod);
                    decimal cantidadDescontar = 0;

                    if (det_Mod.cantidad.Value > 1m && !eliminarTodo)
                    {
                        cantidadDescontar = 1m;
                        CheqsDet2[iDet].cantidad -= cantidadDescontar;
                    }
                    else
                    {
                        cantidadDescontar = det_Mod.cantidad.Value;
                        CheqsDet2.Remove(det_Mod);
                    }

                    totaldescontar += det_Mod.precio.Value * cantidadDescontar * (100m - det_Mod.cantidad.Value) / 100m * descuentoCheque;
                }
            }

            return totaldescontar;
        }

        public void InicializarControles()
        {
            DetalleModificacionCheques = new ObservableCollection<DetalleAjusteMasivo>();
            Cheqs = new List<SR_cheques>();
            Cheqs1 = new List<SR_cheques>();
            CheqsDet = new List<SR_cheqdet>();
            CheqsDet1 = new List<SR_cheqdet>();
            CheqsDet2 = new List<SR_cheqdet>();
            CheqsPago = new List<SR_chequespagos>();
            CheqsPago1 = new List<SR_chequespagos>();
            FormasPago = new List<SR_formasdepago>();

            Turno = true;
            Periodo = false;
            FechaInicio = DateTime.Today;
            FechaCierre = DateTime.Today;
            HorarioTurno = "? - ?";
            ImporteMinimoAjustable = 0.00m;
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

        public Respuesta ObtenerInformacionSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    DateTime fechaInicio;
                    DateTime fechaCierre;

                    if (Turno)
                    {
                        fechaInicio = FechaInicio.AddSeconds(HoraInicio.TotalSeconds);
                        fechaCierre = FechaInicio.AddSeconds(HoraCierre.TotalSeconds);

                        if (HoraInicio > HoraCierre)
                        {
                            fechaCierre = fechaCierre.AddDays(1);
                        }
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

                    Cheqs = cheques_DAO.Get(queryWhere, new object[] {
                        new SqlParameter("FechaInicio", fechaInicio),
                        new SqlParameter("FechaCierre", fechaCierre),
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

                    foreach (var cheque in Cheqs)
                    {
                        DetalleModificacionCheques.Add(new DetalleAjusteMasivo
                        {
                            Folio = cheque.folio,
                            FolioNotaConsumo = cheque.folionotadeconsumo.Value,
                            Fecha = cheque.fecha.Value,
                            Cancelado = cheque.cancelado.Value,
                            Facturado = cheque.facturado.Value,
                            Descuento = cheque.descuento.Value,
                            TotalOriginal = cheque.total.Value,
                            ProductosEliminados = 0,
                            TotalArticulos = CheqsDet.Count(x => x.foliodet == cheque.folio),
                            TotalConDescuento = cheque.total.Value,
                            Efectivo = cheque.efectivo.Value,
                            Tarjeta = cheque.tarjeta.Value,
                            Vales = cheque.vales.Value,
                            Otros = cheque.otros.Value,
                            Modificar = true,
                        });
                    }

                    RefrescarControles();

                    return Respuesta.HECHO;
                }
                catch
                {
                    return Respuesta.ERROR;
                }
            }
        }

        public void RefrescarControles()
        {
            Cheqs1 = (Cheqs as IEnumerable<SR_cheques>).ToList();

            CheqsDet1 = (CheqsDet as IEnumerable<SR_cheqdet>).ToList();
            CheqsDet2 = (CheqsDet as IEnumerable<SR_cheqdet>).ToList();

            CheqsPago1 = (CheqsPago as IEnumerable<SR_chequespagos>).ToList();

            decimal total = Cheqs.Sum(x => x.total ?? 0);

            ImporteObjetivo = total * (100m - PorcentajeObjetivo) / 100m;
            NumeroTotalCuentas = Cheqs.Count;
            ImporteAnterior = total;
            EfectivoAnterior = Cheqs.Sum(x => x.efectivo.Value);
        }

        private SR_configuracion SRConfiguracion { get; set; }
        private List<SR_cheques> Cheqs { get; set; }
        private List<SR_cheques> Cheqs1 { get; set; }
        private List<SR_cheqdet> CheqsDet { get; set; }
        private List<SR_cheqdet> CheqsDet1 { get; set; }
        private List<SR_cheqdet> CheqsDet2 { get; set; }
        private List<SR_chequespagos> CheqsPago { get; set; }
        private List<SR_chequespagos> CheqsPago1 { get; set; }
        private List<SR_formasdepago> FormasPago { get; set; }
        private TimeSpan HoraInicio { get; set; }
        private TimeSpan HoraCierre { get; set; }

        private ObservableCollection<DetalleAjusteMasivo> _DetalleModificacionCheques;
        public ObservableCollection<DetalleAjusteMasivo> DetalleModificacionCheques
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
