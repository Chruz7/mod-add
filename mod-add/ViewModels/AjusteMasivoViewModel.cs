﻿using mod_add.Datos.Contexto;
using mod_add.Datos.Enums;
using mod_add.Datos.Modelos;
using mod_add.Enums;
using mod_add.Selectores;
using mod_add.Utils;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
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
            Procesos = new List<Proceso>
            {
                new Proceso
                {
                    Texto = "Folios",
                    TipoProceso = TipoProceso.FOLIOS
                },
                new Proceso
                {
                    Texto = "Productos",
                    TipoProceso = TipoProceso.PRODUCTOS
                }
            };

            InicializarControles();
        }

        public TipoRespuesta GuardarCambios()
        {
            //using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            //{
            //    try
            //    {
            //        SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
            //        SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
            //        SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

            //        var foliosAfectar = DetalleModificacionCheques.Where(x => x.RealizarAccion).Select(x => x.Folio).ToList();
            //        var cheqs = Cheqs1.Where(x => foliosAfectar.Contains(x.folio)).ToList();

            //        foreach(var cheque in cheqs)
            //        {
            //            cheques_DAO.Update(cheque);
            //            cheqdet_DAO.Delete(cheque.folio);

            //            var chequepago = CheqsPago1.Find(x => x.folio == cheque.folio);
            //            chequespagos_DAO.Update(chequepago);
            //        }

            //        var cheqsDet = ReenumerarDetalles();

            //        cheqdet_DAO.Create(cheqsDet);

            //        return TipoRespuesta.HECHO;
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            //        return TipoRespuesta.ERROR;
            //    }
            //}

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Cheques");
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Cheques_Detalle");
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Cheques_Pago");
            }

            return TipoRespuesta.HECHO;
        }

        public void ProcesarProductos()
        {
            Debug.WriteLine("INICIO-PROCESO-AJUSTE-MASIVO");
            try
            {
                EliminarProductos();
                CambiarProductos();
                AjustarCheques();

                //NumeroTotalCuentasModificadas = Cheqs1.Count(x => x.Actualizar);
                //ImporteNuevo = Cheqs1.Sum(x => x.total.Value);
                //Diferencia = ImporteAnterior - ImporteNuevo;
                //PorcentajeDiferencia = Math.Round(Diferencia / ImporteAnterior * 100m, 2, MidpointRounding.AwayFromZero);
                //EfectivoNuevo = Cheqs1.Sum(x => x.efectivo.Value);

                

                //foreach (var cheq in Cheqs)
                //{
                //    var cheqActualizado = Cheqs1.Find(x => x.folio == cheq.folio);
                //    int totalArticulosAnterior = CheqsDet.Where(x => x.foliodet == cheq.folio).Sum(x => x.CantidadEntera);
                //    int totalArticulosNuevo = CheqsDet1.Where(x => x.foliodet == cheq.folio).Sum(x => x.CantidadEntera);

                //    DetalleAjustes1.Add(new DetalleAjuste
                //    {
                //        Folio = cheq.folio,
                //        FolioNotaConsumo = cheq.folionotadeconsumo.Value,
                //        Fecha = cheq.fecha.Value,
                //        Cancelado = cheq.cancelado.Value ? TipoLogico.SI : TipoLogico.NO,
                //        Facturado = cheq.facturado.Value ? TipoLogico.SI : TipoLogico.NO,
                //        Descuento = cheq.descuento.Value,
                //        TotalOriginal = cheq.total.Value,
                //        ProductosEliminados = totalArticulosAnterior - totalArticulosNuevo,
                //        TotalArticulos = totalArticulosAnterior,
                //        TotalConDescuento = cheqActualizado.total.Value,
                //        Efectivo = cheqActualizado.efectivo.Value,
                //        Tarjeta = cheqActualizado.tarjeta.Value,
                //        Vales = cheqActualizado.vales.Value,
                //        Otros = cheqActualizado.otros.Value,
                //        RealizarAccion = cheqActualizado.Actualizar,
                //    });
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }

            Debug.WriteLine("FIN-PROCESO-AJUSTE-MASIVO");
        }

        public void AjustarCheques()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var cheques = context.Cheques
                        .Where(x =>
                            x.total.Value >= ImporteMinimoAjustable &&
                            (
                                x.TipoPago == TipoPago.EFECTIVO ||
                                (IncluirCuentaPagadaTarjeta ? x.TipoPago == TipoPago.TARJETA : false) ||
                                (IncluirCuentaPagadaVales ? x.TipoPago == TipoPago.TARJETA : false) ||
                                (IncluirCuentaPagadaOtros ? x.TipoPago == TipoPago.TARJETA : false)
                            ) &&
                            (IncluirCuentaFacturada ? true : !x.facturado.Value) &&
                            (IncluirCuentaNotaConsumo ? true : x.folionotadeconsumo.Value == 0) &&
                            (NoIncluirCuentasReimpresas ? x.impresiones.Value == 1 : true)
                        ).ToList();

                    foreach (var cheque in cheques)
                    {
                        #region Ajuste del cheque
                        var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet == cheque.folio).ToList();
                        var detActualizados = chequesDetalle.Where(x => x.TipoAccion != TipoAccion.ELIMINAR).ToList();
                        var detEliminados = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.ELIMINAR).ToList();
                        var detCambiados = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR && x.Cambiado).ToList();

                        decimal totalConImpuestos_Det1 = Mat.Redondeo(detActualizados.Sum(x => x.precio.Value * x.cantidad.Value * (100m - x.descuento.Value) / 100m));
                        decimal totalConImpuestos_Det = Mat.Redondeo(detActualizados.Sum(x => x.ImporteCICD));
                        decimal descuentoAplicado = (100m - cheque.descuento.Value) / 100m;
                        decimal totalNuevo = Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);

                        if (cheque.total.Value == totalNuevo) continue;

                        decimal descuento = cheque.descuento.Value / 100m;
                        decimal totalSinImpuestos_Det = Mat.Redondeo(detActualizados.Sum(x => x.ImporteSICD));

                        cheque.TipoAccion = TipoAccion.ACTUALIZAR;
                        cheque.TotalArticulosEliminados = detEliminados.Sum(x => x.CantidadAnterior);
                        cheque.TotalArticulosCambiados = detCambiados.Sum(x => x.CantidadAnterior);

                        if (QuitarPropinasManualmente)
                        {
                            cheque.propina = 0;
                            cheque.propinatarjeta = 0;
                            //falta validar lo de las propinas
                        }

                        cheque.totalarticulos = detActualizados.Sum(x => x.cantidad.Value);
                        cheque.subtotal = totalSinImpuestos_Det;
                        cheque.total = totalNuevo;

                        cheque.totalconpropina = cheque.total; // falta validar si la propina se agrega por configuracion

                        cheque.totalconcargo = cheque.total + cheque.cargo;
                        cheque.totalconpropinacargo = cheque.total + cheque.cargo; // falta validar si la propina se agrega por configuracion
                        cheque.descuentoimporte = Mat.Redondeo(totalSinImpuestos_Det * descuento);

                        if (cheque.TipoPago == TipoPago.EFECTIVO)
                        {
                            cheque.efectivo = cheque.total;
                        }
                        else if (cheque.TipoPago == TipoPago.TARJETA)
                        {
                            cheque.tarjeta = cheque.total;
                            cheque.propinatarjeta = cheque.propina;
                        }
                        else if (cheque.TipoPago == TipoPago.VALES)
                        {
                            cheque.vales = cheque.total;
                        }
                        else if (cheque.TipoPago == TipoPago.OTROS)
                        {
                            cheque.otros = cheque.total;
                        }

                        cheque.totalsindescuento = Mat.Redondeo(detActualizados.Sum(x => x.ImporteSISD));

                        decimal totalalimentos = 0;
                        decimal totalbebidas = 0;
                        decimal totalotros = 0;
                        decimal totalalimentosdescuento = 0;
                        decimal totalbebidasdescuento = 0;
                        decimal totalotrosdescuento = 0;
                        decimal totalalimentossindescuento = 0;
                        decimal totalbebidassindescuento = 0;
                        decimal totalotrossindescuento = 0;

                        foreach (var detalle in detActualizados)
                        {
                            decimal importedetalle = detalle.ImporteSICD;
                            decimal importedetallesindescuento = detalle.ImporteSISD;
                            decimal importedetalledescuento = importedetallesindescuento - importedetalle;

                            if (detalle.TipoClasificacion == TipoClasificacion.BEBIDAS)
                            {
                                totalbebidas += importedetalle;
                                totalbebidassindescuento += importedetallesindescuento;
                                totalbebidasdescuento += importedetalledescuento;
                            }
                            else if (detalle.TipoClasificacion == TipoClasificacion.ALIMENTOS)
                            {
                                totalalimentos += importedetalle;
                                totalalimentossindescuento += importedetallesindescuento;
                                totalalimentosdescuento += importedetalledescuento;
                            }
                            else if (detalle.TipoClasificacion == TipoClasificacion.OTROS)
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

                        cheque.totalimpuestod1 = Mat.Redondeo(detActualizados.Sum(x => x.ImporteI1CD));
                        cheque.totalimpuestod2 = Mat.Redondeo(detActualizados.Sum(x => x.ImporteI2CD));
                        cheque.totalimpuestod3 = Mat.Redondeo(detActualizados.Sum(x => x.ImporteI3CD));

                        cheque.totalimpuesto1 = cheque.totalimpuestod1;

                        cheque.desc_imp_original = cheque.descuentoimporte;

                        cheque.cambio = 0; // como se debe ajustar el cambio si es efectivo? - por el momento queda en ceros
                        cheque.cambiorepartidor = 0;
                        #endregion

                        #region Ajuste del cheque pago
                        var chequePago = context.ChequesPago.FirstOrDefault(x => x.folio == cheque.folio);
                        chequePago.importe = cheque.total;

                        if (cheque.TipoPago == TipoPago.TARJETA)
                        {
                            chequePago.propina = cheque.propina;
                        }
                        #endregion

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public void EliminarProductos(decimal movimiento = 1m)
        {
            if (movimiento == 1) Debug.WriteLine("INICIO-ELIMINACION");
            Debug.WriteLine($"movimiento: {movimiento}");
            bool continua = false;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var detalles = context.ChequesDetalle
                        .Where(x => !x.modificador.Value && x.movimiento == movimiento && x.TipoAccion != TipoAccion.ELIMINAR)
                        .OrderBy(x => x.movimiento.Value).ThenBy(x => x.foliodet.Value)
                        .ToList();

                    foreach (var det in detalles)
                    {
                        Debug.WriteLine($"folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}");

                        decimal productosRestantes = context.ChequesDetalle
                            .Where(x => x.foliodet == det.foliodet && !x.modificador.Value && x.TipoAccion != TipoAccion.ELIMINAR)
                            .Sum(x => x.cantidad.Value);

                        Debug.WriteLine($"productos principales restantes de la cuenta: {productosRestantes}");
                        Debug.WriteLine($"productos minimos por cuenta: {App.ConfiguracionSistema.MinProductosCuenta}");

                        if (productosRestantes <= App.ConfiguracionSistema.MinProductosCuenta)
                        {
                            Debug.WriteLine("Cantidad mínima alcanzada");
                            continue;
                        }

                        Debug.WriteLine("Pasó el fitro de productos minimos");

                        if (App.ConfiguracionSistema.EliminarProductosSeleccionados && !App.ProductosEliminar.Any((Func<ProductoEliminacion, bool>)(x => x.Clave == det.idproducto)))
                        {
                            Debug.WriteLine("Producto NO eliminable");
                            continue;
                        }

                        bool eliminarTodo = false;

                        if (det.cantidad.Value > 1m)
                        {
                            Debug.WriteLine($"Eliminar: {1}");

                            det.cantidad -= 1m;
                            det.TipoAccion = TipoAccion.ACTUALIZAR;
                            continua = true;
                        }
                        else
                        {
                            Debug.WriteLine($"Eliminar: {det.cantidad.Value}");

                            det.cantidad = 0;
                            det.TipoAccion = TipoAccion.ELIMINAR;
                            eliminarTodo = true;
                        }

                        context.SaveChanges();

                        EliminarProductosModificadores(context, det.idproductocompuesto, eliminarTodo);

                        CalcularImporteNuevo(context);

                        if (ImporteNuevo <= ImporteObjetivo)
                        {
                            break;
                        }
                    }

                    movimiento += 1m;

                    if (continua && (ImporteNuevo != -1) && (ImporteNuevo > ImporteObjetivo) && movimiento <= UltimoMovimiento)
                    {
                        EliminarProductos(movimiento);
                    }
                    else if (ImporteNuevo == -1)
                    {
                        ImporteNuevo = 0;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
            Debug.WriteLine("FIN-ELIMINACION");
        }

        private void EliminarProductosModificadores(ApplicationDbContext context, string idproductocompuesto, bool eliminarTodo = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idproductocompuesto)) return;

                Debug.WriteLine($"Eliminar modificadores con idproductocompuesto: {idproductocompuesto}. {(eliminarTodo ? "TODO" : "")}");

                Debug.WriteLine("INICIO-ELIMINACION-MODIFICADORES");
                var detalles = context.ChequesDetalle
                    .Where(x => x.idproductocompuesto.Equals(idproductocompuesto) && x.modificador.Value && x.TipoAccion != TipoAccion.ELIMINAR)
                    .ToList();

                foreach (var det in detalles)
                {
                    Debug.WriteLine($"folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}");

                    if (App.ConfiguracionSistema.EliminarProductosSeleccionados && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto) && !eliminarTodo)
                    {
                        Debug.WriteLine("Producto NO eliminable");
                        continue;
                    }

                    if (det.cantidad.Value > 1m && !eliminarTodo)
                    {
                        Debug.WriteLine($"Eliminar: {1}");

                        det.cantidad -= 1m;
                        det.TipoAccion = TipoAccion.ACTUALIZAR;
                    }
                    else
                    {
                        Debug.WriteLine($"Elimniar: {det.cantidad.Value}");

                        det.cantidad = 0;
                        det.TipoAccion = TipoAccion.ELIMINAR;
                    }

                    context.SaveChanges();
                }
                Debug.WriteLine("FIN-ELIMINACION-MODIFICADORES");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void CambiarProductos()
        {
            if (ImporteNuevo <= ImporteObjetivo) return;
            if (App.ProductosReemplazo.Count == 0) return;

            Debug.WriteLine("INICIO-CAMBIO-PRODUCTOS");

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var detalles = context.ChequesDetalle
                    .Where(x => !x.modificador.Value && x.TipoAccion != TipoAccion.ELIMINAR)
                    .OrderBy(x => x.movimiento).ThenBy(x => x.foliodet)
                    .ToList();

                foreach (var det in detalles)
                {
                    Debug.WriteLine($"folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}");
                    int porcentajeAleatorio = Random.Next(1, 100);
                    int porcentajeAnterior = 0;
                    Debug.WriteLine($"Porcentaje aleatorio: {porcentajeAleatorio}");
                    if (App.ConfiguracionSistema.EliminarProductosSeleccionados && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto))
                    {
                        Debug.WriteLine("Producto NO eliminable");
                        continue;
                    }

                    foreach (var productoReemplazo in App.ProductosReemplazo)
                    {
                        if (productoReemplazo.Porcentaje > porcentajeAnterior && productoReemplazo.Porcentaje <= porcentajeAleatorio)
                        {
                            var detalleProducto = App.SRProductosDetalle.Find(x => x.idproducto == productoReemplazo.Clave);
                            Debug.WriteLine($"producto reemplazo: {productoReemplazo.Clave}, porcentaje: {productoReemplazo.Porcentaje}, precio: {detalleProducto.precio}");

                            if (detalleProducto.precio < det.precio)
                            {
                                string idproductocompuesto = det.idproductocompuesto;

                                det.idproducto = detalleProducto.idproducto;
                                det.precio = detalleProducto.precio;
                                det.cantidad = 1m;
                                det.impuesto1 = detalleProducto.impuesto1;
                                det.impuesto2 = detalleProducto.impuesto2;
                                det.impuesto3 = detalleProducto.impuesto3;
                                det.preciosinimpuestos = detalleProducto.preciosinimpuestos;
                                det.preciocatalogo = detalleProducto.precio;
                                det.impuestoimporte3 = detalleProducto.impuestoimporte3;
                                det.idproductocompuesto = "";
                                det.TipoAccion = TipoAccion.ACTUALIZAR;
                                det.Cambiado = true;

                                context.SaveChanges();

                                Debug.WriteLine("producto reemplazado");

                                EliminarProductosModificadores(context, idproductocompuesto, true);
                            }
                            else
                            {
                                Debug.WriteLine("producto NO reemplazado");
                            }

                            break;
                        }

                        porcentajeAnterior = productoReemplazo.Porcentaje;
                    }

                    CalcularImporteNuevo(context);

                    if (ImporteNuevo <= ImporteObjetivo) break;
                }
            }
            Debug.WriteLine("FIN-CAMBIO-PRODUCTOS");
        }

        public void CalcularImporteNuevo(ApplicationDbContext context)
        {
            try
            {
                decimal importeNuevo = 0;
                foreach (var cheq in context.Cheques)
                {
                    var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet == cheq.folio && x.TipoAccion != TipoAccion.ELIMINAR).ToList();
                    decimal totalConImpuestos_Det = Mat.Redondeo(chequesDetalle.Sum(x => x.ImporteCICD));
                    decimal descuentoAplicado = (100m - cheq.descuento.Value) / 100m;
                    importeNuevo += Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);
                }
                ImporteNuevo = importeNuevo;

                Debug.WriteLine($"ImporteAnterior: {ImporteAnterior}, ImporteObjetivo: {ImporteObjetivo}, ImporteNuevo: {ImporteNuevo}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                ImporteNuevo = -1;
            }
        }

        public List<SR_cheqdet> ReenumerarDetalles()
        {
            //int movimiento = 1;
            //long folio = 0;
            //var foliosAModificar = DetalleModificacionCheques.Where(x => x.RealizarAccion).Select(x => x.Folio).ToList();
            //var cheqsDet = CheqsDet1.Where(x => foliosAModificar.Contains(x.foliodet.Value)).OrderBy(x => x.foliodet.Value).ThenBy(x => x.movimiento.Value).ToList();

            //foreach (var detalle in cheqsDet)
            //{
            //    if (folio != detalle.foliodet.Value)
            //    {
            //        folio = detalle.foliodet.Value;
            //        movimiento = 1;
            //    }

            //    detalle.movimiento = movimiento;
            //    movimiento++;
            //}

            //return cheqsDet;
            return null;
        }

        public void InicializarControles()
        {
            DetalleModificacionCheques = new ObservableCollection<DetalleAjuste>();
            UltimoMovimiento = 0;
            FechaCorteInicio = DateTime.Today;
            FechaCorteCierre = DateTime.Today;
            Turno = true;
            Periodo = false;
            FechaInicio = DateTime.Today;
            FechaCierre = DateTime.Today;
            CorteInicio = App.SRConfiguracion.CorteInicio;
            CorteCierre = App.SRConfiguracion.CorteCierre;
            HorarioTurno = $"{App.SRConfiguracion.cortezinicio} - {App.SRConfiguracion.cortezfin}";
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

        public RespuestaBusquedaMasiva ObtenerChequesSR()
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
                    string queryWhere = "";
                    //string queryWhere = "(efectivo >= total";

                    //if (IncluirCuentaPagadaTarjeta)
                    //    queryWhere += " OR tarjeta = (total + propinatarjeta)";

                    //if (IncluirCuentaPagadaVales)
                    //    queryWhere += " OR vales = total";

                    //if (IncluirCuentaPagadaOtros)
                    //    queryWhere += " OR otros = total";

                    //queryWhere += ") AND ";

                    //if (!IncluirCuentaFacturada)
                    //    queryWhere += "facturado = 0 AND ";

                    //if (!IncluirCuentaNotaConsumo)
                    //    queryWhere += "folionotadeconsumo = 0 AND ";

                    //if (NoIncluirCuentasReimpresas)
                    //    queryWhere += "impresiones = 1 AND ";

                    queryWhere += "fecha BETWEEN @FechaInicio AND @FechaCierre";

                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    var cheques = cheques_DAO.Get(queryWhere, new object[] {
                        new SqlParameter("FechaInicio", FechaCorteInicio),
                        new SqlParameter("FechaCierre", FechaCorteCierre),
                    });

                    if (cheques.Count == 0) return new RespuestaBusquedaMasiva
                    {
                        TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                    };

                    var folios = cheques.Select(x => (object)x.folio).ToArray();

                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    var cheqdet = cheqdet_DAO.WhereIn("foliodet", folios);

                    //CheqsDet = cheqdet.OrderBy(x => x.movimiento).ThenBy(x => x.foliodet).ToList();

                    SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    var chequespagos = chequespagos_DAO.WhereIn("folio", folios);

                    SR_formasdepago_DAO formasdepago_DAO = new SR_formasdepago_DAO(context);
                    var formasdepago = formasdepago_DAO.GetAll();

                    UltimoMovimiento = cheqdet.Max(x => x.movimiento.Value);
                    ImporteAnterior = cheques.Sum(x => x.total.Value);
                    ImporteObjetivo = ImporteAnterior * (100m - PorcentajeObjetivo) / 100;
                    NumeroTotalCuentas = cheques.Count;
                    EfectivoAnterior = cheques.Sum(x => x.efectivo.Value);

                    return new RespuestaBusquedaMasiva
                    {
                        TipoRespuesta = TipoRespuesta.HECHO,
                        Cheques = cheques,
                        Cheqdet = cheqdet,
                        Chequespagos = chequespagos,
                        Formasdepago = formasdepago,
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return new RespuestaBusquedaMasiva
                    {
                        TipoRespuesta = TipoRespuesta.ERROR
                    };
                }
            }
        }

        public void CrearRegistrosTemporales(RespuestaBusquedaMasiva respuesta)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    List<Cheque> cheques = new List<Cheque>();
                    List<ChequePago> chequesPago = new List<ChequePago>();
                    List<ChequeDetalle> chequesDetalle = new List<ChequeDetalle>();

                    foreach (var cheque in respuesta.Cheques)
                    {
                        var chequespagos2 = respuesta.Chequespagos.Where(x => x.folio == cheque.folio).ToList();

                        TipoPago tipoPago = TipoPago.NINGUNO;

                        if (chequespagos2.Count == 1)
                        {
                            var formapago = respuesta.Formasdepago.Find(x => x.idformadepago == chequespagos2[0].idformadepago);
                            tipoPago = (TipoPago)formapago.tipo;
                        }
                        else if (chequespagos2.Count > 1)
                        {
                            tipoPago = TipoPago.MULTIPLE;
                        }

                        cheques.Add(Funciones.ParseCheque(cheque, tipoPago));
                    }

                    foreach (var chequepago in respuesta.Chequespagos)
                    {
                        chequesPago.Add(Funciones.ParseChequePago(chequepago));
                    }

                    foreach (var det in respuesta.Cheqdet)
                    {
                        TipoClasificacion tipoClasificacion = ObtenerClasificacionProductoSR(det.idproducto);
                        chequesDetalle.Add(Funciones.ParseChequeDetalle(det, tipoClasificacion));
                    }

                    context.Cheques.AddRange(cheques);
                    context.ChequesDetalle.AddRange(chequesDetalle);
                    context.ChequesPago.AddRange(chequesPago);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public TipoClasificacion ObtenerClasificacionProductoSR(string idproducto)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {

                try
                {
                    SR_productos_DAO productos_DAO = new SR_productos_DAO(context);
                    var producto = productos_DAO.Find(idproducto);

                    var grupo = producto.Grupo;

                    return (TipoClasificacion)grupo.clasificacion;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return TipoClasificacion.NINGUNO;
                }
            }
        }

        public void CargarResultados()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var cheques = context.Cheques.ToList();
                NumeroTotalCuentasModificadas = cheques.Count(x => x.TipoAccion == TipoAccion.ACTUALIZAR);
                Diferencia = ImporteAnterior - ImporteNuevo;
                PorcentajeDiferencia = Math.Round(Diferencia / ImporteAnterior * 100m, 2, MidpointRounding.AwayFromZero);
                EfectivoNuevo = cheques.Sum(x => x.efectivo.Value);

                foreach (var cheque in cheques)
                {
                    DetalleModificacionCheques.Add(new DetalleAjuste
                    {
                        Folio = cheque.folio,
                        Fecha = cheque.fecha.Value,
                        FolioNotaConsumo = cheque.folionotadeconsumo.Value,
                        Cancelado = cheque.cancelado.Value ? TipoLogico.SI : TipoLogico.NO,
                        Facturado = cheque.facturado.Value ? TipoLogico.SI : TipoLogico.NO,
                        Descuento = cheque.descuento.Value,
                        TotalOriginal = cheque.TotalAnterior,
                        TotalArticulos = cheque.TotalArticulosAnterior,
                        ProductosEliminados = cheque.TotalArticulosEliminados,
                        TotalConDescuento = cheque.TotalAnterior - cheque.total.Value,
                        Efectivo = cheque.efectivo.Value,
                        Tarjeta = cheque.tarjeta.Value,
                        Vales = cheque.vales.Value,
                        Otros = cheque.otros.Value,
                        RealizarAccion = cheque.TipoAccion == TipoAccion.ACTUALIZAR || cheque.TipoAccion == TipoAccion.ELIMINAR,
                    });
                }
            }
        }

        public void ResgistrarBitacora()
        {
            var ChequesAModificar = DetalleModificacionCheques.Where(x => x.RealizarAccion).ToList();
            var importeNuevo = ChequesAModificar.Sum(x => x.TotalConDescuento);

            Funciones.RegistrarModificacion(new BitacoraModificacion
            {
                TipoAjuste = TipoAjuste.MASIVO,
                FechaProceso = DateTime.Now,
                FechaInicialVenta = FechaCorteInicio,
                FechaFinalVenta = FechaCorteCierre,
                TotalCuentas = NumeroTotalCuentas,
                CuentasModificadas = NumeroTotalCuentasModificadas,
                ImporteAnterior = ImporteAnterior,
                ImporteNuevo = ImporteNuevo,
                Diferencia = PorcentajeDiferencia,
            });
        }

        public decimal UltimoMovimiento { get; set; }
        public DateTime FechaCorteInicio { get; set; }
        public DateTime FechaCorteCierre { get; set; }
        private TimeSpan CorteInicio { get; set; }
        private TimeSpan CorteCierre { get; set; }

        public Proceso Proceso { get; set; }

        private List<Proceso> _Procesos;
        public List<Proceso> Procesos
        {
            get { return _Procesos; }
            set
            {
                _Procesos = value;
                OnPropertyChanged(nameof(Procesos));
            }
        }

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
