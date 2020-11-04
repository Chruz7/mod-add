using mod_add.Datos.Contexto;
using mod_add.Datos.Enums;
using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
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
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace mod_add.ViewModels
{
    public class AjusteMasivoViewModel : ViewModelBase
    {
        private readonly Random Random;
        private DatabaseFactory dbf;
        private ITurnoServicio turnoServicio;
        private IChequeServicio chequeServicio;
        private IChequeDetalleServicio chequeDetalleServicio;
        private IChequePagoServicio chequePagoServicio;

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

            //DetalleModificacionCheques.Add(new DetalleAjuste
            //{
            //    Folio = 1,
            //    Fecha = DateTime.Now,
            //    FolioNotaConsumo = 0,
            //    Cancelado = TipoLogico.NO,
            //    Facturado = TipoLogico.NO,
            //    Descuento = 0,
            //    TotalOriginal = 0,
            //    TotalArticulos = 0,
            //    ProductosEliminados = 0,
            //    TotalConDescuento = 0,
            //    Efectivo = 0,
            //    Tarjeta = 0,
            //    Vales = 0,
            //    Otros = 0,
            //    RealizarAccion = true,
            //    IsEnable = true,
            //});

            //DetalleModificacionCheques.Add(new DetalleAjuste
            //{
            //    Folio = 2,
            //    Fecha = DateTime.Now,
            //    FolioNotaConsumo = 0,
            //    Cancelado = TipoLogico.NO,
            //    Facturado = TipoLogico.NO,
            //    Descuento = 0,
            //    TotalOriginal = 0,
            //    TotalArticulos = 0,
            //    ProductosEliminados = 0,
            //    TotalConDescuento = 0,
            //    Efectivo = 0,
            //    Tarjeta = 0,
            //    Vales = 0,
            //    Otros = 0,
            //    RealizarAccion = false,
            //    IsEnable = false,
            //});
        }

        public TipoRespuesta Guardar()
        {
            Debug.WriteLine("GUARDADO-INICIO");
            TipoRespuesta tipoRespuesta = TipoRespuesta.NADA;

            InvertirCambiosDesmarcados();

            //if (Proceso.TipoProceso == TipoProceso.FOLIOS)
            //{
            //    EnumerarFoliosCheques();
            //}
            if (Proceso.TipoProceso == TipoProceso.PRODUCTOS)
            {
                EnumerarMovimientosChequesDetalle();
            }

            AjustarTurnos();

            dbf = new DatabaseFactory();
            turnoServicio = new TurnoServicio(dbf);
            chequeServicio = new ChequeServicio(dbf);
            chequeDetalleServicio = new ChequeDetalleServicio(dbf);
            chequePagoServicio = new ChequePagoServicio(dbf);

            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var query = "";
                        //List<string> nombresParametros = new List<string>();
                        //object[] parametros;
                        //object[] valores;
                        long folio = 0;
                        long idturnointerno = 0;
                        long idturno = 0;
                        string tablaCheques;
                        string tablaTurnos;

                        SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_cheqdetfeliminados_DAO cheqdetfeliminados_DAO = new SR_cheqdetfeliminados_DAO(context);
                        SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_bitacorafiscal_DAO bitacorafiscal_DAO = new SR_bitacorafiscal_DAO(context);

                        if (App.ConfiguracionSistema.ModificarVentasReales)
                        {
                            //folioMin = chequeServicio.PrimerFolio();
                            //idturnoMin = turnoServicio.Primeridturno();
                            tablaCheques = "cheques";
                            tablaTurnos = "turnos";
                        }
                        else
                        {
                            tablaCheques = "chequesf";
                            tablaTurnos = "turnosf";
                        }

                        query = $"SELECT CAST(ISNULL(MAX(idturnointerno), 0) AS bigint) AS idturnointerno FROM {tablaTurnos} WHERE apertura < @{nameof(FechaCorteInicio)} AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        idturnointerno = context.Database.SqlQuery<long>(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa)).Single();

                        query = $"SELECT CAST(ISNULL(MAX(idturno), 0) AS bigint) AS idturno FROM {tablaTurnos} WHERE apertura < @{nameof(FechaCorteInicio)} AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        idturno = context.Database.SqlQuery<long>(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa)).Single();

                        query = $"SELECT CAST(ISNULL(MAX(folio), 0) AS bigint) AS folio FROM {tablaCheques} WHERE idturno <= @{nameof(idturno)} AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        folio = context.Database.SqlQuery<long>(query,
                            new SqlParameter($"{nameof(idturno)}", idturno),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa)).Single();

                        //if (Proceso.TipoProceso == TipoProceso.FOLIOS)
                        //{
                        //    //EnumerarFoliosCheques(folio + 1, idturno + 1);
                        //}
                        EnumerarTurnosYFolios(idturno + 1, folio + 1);

                        Debug.WriteLine("Inicio de la transaccion");

                        #region Eliminacion de registros
                        Debug.WriteLine("Eliminando bitacora fiscal");
                        query = $"(@{nameof(FechaCorteInicio)} BETWEEN fechainicial AND fechafinal) AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        bitacorafiscal_DAO.Delete(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));

                        //Debug.WriteLine("Obteniendo folios a eliminar");
                        //valores = chequeServicio.ObtenerFolios();
                        //Debug.WriteLine($"folios {string.Join(",", valores)}");
                        //parametros = new object[valores.Length + 1];
                        //parametros[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);
                        //nombresParametros.Clear();

                        //for (int i = 0; i < valores.Length; i++)
                        //{
                        //    string nombreParametro = $"p{i + 1}";
                        //    nombresParametros.Add($"@{nombreParametro}");
                        //    parametros[i + 1] = new SqlParameter(nombreParametro, valores[i]);
                        //}

                        Debug.WriteLine("Eliminando cheques pago");
                        //query = $"folio IN ({string.Join(",", nombresParametros)})";
                        //chequespagos_DAO.Delete(query, parametros);
                        query = $"folio > @{nameof(folio)}";
                        chequespagos_DAO.Delete(query, new SqlParameter($"{nameof(folio)}", folio));

                        Debug.WriteLine("Eliminando cheques detalle");
                        //query = $"foliodet IN ({string.Join(",", nombresParametros)})";
                        //cheqdet_DAO.Delete(query, parametros);
                        query = $"foliodet > @{nameof(folio)}";
                        cheqdet_DAO.Delete(query, new SqlParameter($"{nameof(folio)}", folio));

                        if (!App.ConfiguracionSistema.ModificarVentasReales)
                        {
                            Debug.WriteLine("Eliminando cheques detalle fiscal eliminados");
                            //query = $"foliodet IN ({string.Join(",", nombresParametros)})";
                            //cheqdeteliminados_DAO.Delete(query, parametros);
                            query = $"foliodet > @{nameof(folio)}";
                            cheqdetfeliminados_DAO.Delete(query, new SqlParameter($"{nameof(folio)}", folio));
                        }

                        Debug.WriteLine("Eliminando cheques");
                        //query = $"folio IN ({string.Join(",", nombresParametros)}) AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        //cheques_DAO.Delete(query, parametros);
                        query = $"folio > @{nameof(folio)} AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        cheques_DAO.Delete(query,
                            new SqlParameter($"{nameof(folio)}", folio),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));

                        Debug.WriteLine("Eliminando turnos");
                        query = $"idturnointerno > @{nameof(idturnointerno)} AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        turnos_DAO.Delete(query,
                            new SqlParameter($"{nameof(idturnointerno)}", idturnointerno),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));

                        Debug.WriteLine("Restablecer indices cheques");
                        query = $"DBCC CHECKIDENT ({tablaCheques}, RESEED, @{nameof(folio)})";
                        context.Database.ExecuteSqlCommand(query, new SqlParameter($"{nameof(folio)}", folio));

                        Debug.WriteLine("Restablecer indices turnos");
                        query = $"DBCC CHECKIDENT ({tablaTurnos}, RESEED, @{nameof(idturnointerno)})";
                        context.Database.ExecuteSqlCommand(query, new SqlParameter($"{nameof(idturnointerno)}", idturnointerno));
                        #endregion

                        #region Creacion y actualizacion de registros
                        Debug.WriteLine("Actualizando turnos");
                        //ActualizarTurnosSR(turnos_DAO);
                        RecrearTurnosSR(turnos_DAO);

                        Debug.WriteLine("Recreando cheques");
                        RecrearChequesSR(cheques_DAO);

                        Debug.WriteLine("Recreando cheques detalle");
                        RecrearChequesDetalleSR(cheqdet_DAO);

                        Debug.WriteLine("Recreando cheques pago");
                        RecrearChequesPagoSR(chequespagos_DAO);

                        if (!App.ConfiguracionSistema.ModificarVentasReales)
                        {
                            Debug.WriteLine("Recreando cheques detalle eliminados");
                            RecrearChequesDetalleEliminadosSR(cheqdetfeliminados_DAO);
                        }

                        Debug.WriteLine("Recreando registro de la bitacora fiscal");

                        bitacorafiscal_DAO.Create(new SR_bitacorafiscal
                        {
                            fecha = DateTime.Now,
                            fechainicial = FechaCorteInicio,
                            fechafinal = FechaCorteCierre,
                            cuentastotal = NumeroTotalCuentas,
                            cuentasmodificadas = NumeroTotalCuentasModificadas,
                            importeanterior = ImporteAnterior,
                            importenuevo = ImporteNuevo,
                            diferencia = PorcentajeDiferencia,
                            tipo = Proceso.TipoProceso.ToString(),
                            modificaventareal = App.ConfiguracionSistema.ModificarVentasReales,
                            idempresa = App.ClaveEmpresa,
                        });
                        #endregion

                        transaction.Commit();
                        tipoRespuesta = TipoRespuesta.HECHO;
                        Debug.WriteLine("Fin de la transaccion");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                        Debug.WriteLine("Transaccion interrumpida");
                        tipoRespuesta = TipoRespuesta.ERROR;
                    }
                }
            }

            Debug.WriteLine("GUARDADO-FIN");
            return tipoRespuesta;
        }

        public void InvertirCambiosDesmarcados()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    //var turnos = context.Turnos.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.ELIMINAR).ToList();

                    //foreach (var turno in turnos)
                    //{
                    //    turno.efectivo = turno.EfectivoAnterior;
                    //    turno.tarjeta = turno.TarjetaAnterior;
                    //    turno.vales = turno.ValesAnterior;
                    //    turno.credito = turno.CreditoAnterior;

                    //    context.SaveChanges();
                    //}


                    var cheques = context.Cheques.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.ELIMINAR).ToList();

                    foreach (var cheque in cheques)
                    {
                        bool realizarAccion = DetalleModificacionCheques.Where(x => x.Folio == cheque.folio).Select(x => x.RealizarAccion).First();

                        if (realizarAccion)
                        {
                            continue;
                        }

                        Debug.WriteLine("Invirtiendo cambios");
                        Debug.WriteLine($"Tipo de proceso: {Proceso.TipoProceso}");
                        Debug.WriteLine($"Folio: {cheque.folio}, tipo accion {cheque.TipoAccion}");

                        cheque.TipoAccion = TipoAccion.NINGUNO;
                        cheque.TotalArticulosEliminados = 0;

                        cheque.propina = cheque.PropinaAnt;
                        cheque.propinatarjeta = cheque.PropinaTarjetaAnt;

                        cheque.totalarticulos = cheque.TotalArticulosAnt;
                        cheque.subtotal = cheque.SubtotalAnt;
                        cheque.total = cheque.TotalAnt;
                        cheque.totalconpropina = cheque.TotalConPropinaAnt;

                        cheque.totalconcargo = cheque.TotalConCargoAnt;
                        cheque.totalconpropinacargo = cheque.TotalConPropinaCargoAnt;
                        cheque.descuentoimporte = cheque.DescuentoImporteAnt;

                        cheque.efectivo = cheque.EfectivoAnt;
                        cheque.tarjeta = cheque.TarjetaAnt;
                        cheque.vales = cheque.ValesAnt;
                        cheque.otros = cheque.OtrosAnt;

                        cheque.totalsindescuento = cheque.TotalSinDescuentoAnt;

                        cheque.totalalimentos = cheque.TotalAlimentosAnt;
                        cheque.totalbebidas = cheque.TotalBebidasAnt;
                        cheque.totalotros = cheque.TotalOtrosAnt;

                        cheque.totaldescuentos = cheque.TotalDescuentosAnt;
                        cheque.totaldescuentoalimentos = cheque.TotalDescuentoAlimentosAnt;
                        cheque.totaldescuentobebidas = cheque.TotalDescuentoBebidasAnt;
                        cheque.totaldescuentootros = cheque.TotalDescuentoOtrosAnt;

                        cheque.totaldescuentoycortesia = cheque.TotalDescuentoYCortesiaAnt;
                        cheque.totalalimentossindescuentos = cheque.TotalAlimentosSinDescuentosAnt;
                        cheque.totalbebidassindescuentos = cheque.TotalBebidasSinDescuentosAnt;
                        cheque.totalotrossindescuentos = cheque.TotalOtrosSinDescuentosAnt;

                        cheque.subtotalcondescuento = cheque.SubtotalConDescuentoAnt;

                        cheque.totalimpuestod1 = cheque.TotalImpuestoD1Ant;
                        cheque.totalimpuestod2 = cheque.TotalImpuestoD2Ant;
                        cheque.totalimpuestod3 = cheque.TotalImpuestoD3Ant;

                        cheque.totalimpuesto1 = cheque.TotalImpuesto1Ant;

                        cheque.desc_imp_original = cheque.desc_imp_original;

                        cheque.cambio = cheque.CambioAnt;
                        cheque.cambiorepartidor = cheque.CambioRepartidorAnt;


                        if (Proceso.TipoProceso == TipoProceso.FOLIOS)
                        {
                            context.SaveChanges();
                            continue;
                        }

                        var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                        foreach (var det in chequesDetalle)
                        {
                            det.TipoAccion = TipoAccion.NINGUNO;
                            det.cantidad = det.CantidadAnt;
                            det.idproductocompuesto = det.IdProductoCompuestoAnt;

                            if (det.Cambiado)
                            {
                                det.Cambiado = false;

                                det.idproducto = det.IdProductoAnt;
                                det.precio = det.PrecioAnt;
                                det.impuesto1 = det.Impuesto1Ant;
                                det.impuesto2 = det.Impuesto2Ant;
                                det.impuesto3 = det.Impuesto3Ant;
                                det.preciosinimpuestos = det.PrecioSinImpuestosAnt;
                                det.preciocatalogo = det.PrecioCatalogoAnt;
                                det.impuestoimporte3 = det.ImpuestoImporte3Ant;
                            }
                        }

                        var chequesPago = context.ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                        foreach (var chequePago in chequesPago)
                        {
                            chequePago.TipoAccion = TipoAccion.NINGUNO;
                            chequePago.importe = chequePago.ImporteAnt;
                            chequePago.propina = chequePago.PropinaAnt;
                        }

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public void EnumerarMovimientosChequesDetalle()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var cheques = context.Cheques.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR).ToList();

                foreach (var cheque in cheques)
                {
                    var chequesDetalle = context.ChequesDetalle.
                        Where(x => x.foliodet.Value == cheque.folio && 
                             (x.TipoAccion == TipoAccion.MANTENER || x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.ACTUALIZAR))
                        .OrderBy(x => x.movimiento)
                        .ToList();

                    int movimiento = 1;

                    foreach (var det in chequesDetalle)
                    {
                        det.movimiento = movimiento;
                        movimiento++;
                    }

                    context.SaveChanges();
                }
            }
        }

        //public void ActualizarTurnosSR(SR_turnos_DAO turnos_DAO)
        //{
        //    var turnos = turnoServicio
        //        .GetMany(x => x.TipoAccion == TipoAccion.ACTUALIZAR)
        //        .ToList();

        //    foreach (var turno in turnos)
        //    {
        //        turnos_DAO.Update(Funciones.ParseSR_turnos(turno));
        //    }
        //}

        public void RecrearTurnosSR(SR_turnos_DAO turnos_DAO)
        {
            var turnos = turnoServicio
                .GetMany(x => x.TipoAccion != TipoAccion.ELIMINAR &&
                        (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR))
                )
                .ToList();

            foreach (var turno in turnos)
            {
                turnos_DAO.Create(Funciones.ParseSR_turnos(turno));
            }
        }

        public void RecrearChequesSR(SR_cheques_DAO cheques_DAO)
        {
            var cheques = chequeServicio
                .GetMany(x => x.TipoAccion != TipoAccion.ELIMINAR &&
                        (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR))
                )
                .ToList();

            foreach (var cheque in cheques)
            {
                cheques_DAO.Create(Funciones.ParseSR_cheques(cheque));
            }
        }

        public void RecrearChequesDetalleSR(SR_cheqdet_DAO cheqdet_DAO)
        {
            var chequesDetalle = chequeDetalleServicio
                        .GetMany(x => x.TipoAccion != TipoAccion.ELIMINAR &&
                                (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR))
                        )
                        .ToList();

            foreach (var chequeDetalle in chequesDetalle)
            {
                cheqdet_DAO.Create(Funciones.ParseSR_cheqdet(chequeDetalle));
            }
        }

        public void RecrearChequesDetalleEliminadosSR(SR_cheqdetfeliminados_DAO cheqdetfeliminados_DAO)
        {
            var chequesDetalle = chequeDetalleServicio
                .GetMany(x => x.TipoAccion == TipoAccion.ELIMINAR &&
                        (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR))
                )
                .ToList();

            foreach(var chequeDetalle in chequesDetalle)
            {
                cheqdetfeliminados_DAO.Create(new SR_cheqdetfeliminados
                {
                    foliodet = chequeDetalle.foliodet,
                    cantidad = chequeDetalle.CantidadAnt,
                    idproducto = chequeDetalle.idproducto,
                    descuento = chequeDetalle.descuento,
                    precio = chequeDetalle.precio,
                });
            }
        }

        public void RecrearChequesPagoSR(SR_chequespagos_DAO chequespagos_DAO)
        {
            var chequesPago = chequePagoServicio
                .GetMany(x => x.TipoAccion != TipoAccion.ELIMINAR &&
                        (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR))
                )
                .ToList();

            foreach(var chequePago in chequesPago)
            {
                chequespagos_DAO.Create(Funciones.ParseSR_chequespagos(chequePago));
            }
        }

        public void GenerarVistaPrevia()
        {
            if (Proceso.TipoProceso == TipoProceso.FOLIOS)
            {
                ProcesarFolios();
            }
            else if (Proceso.TipoProceso == TipoProceso.PRODUCTOS)
            {
                ProcesarProductos();
            }

            //AjustarTurnos();
        }

        private void ProcesarFolios()
        {
            Debug.WriteLine($"INICIO-PROCESO-AJUSTE-MASIVO-FOLIOS - {DateTime.Now}");
            EliminarFolios();
            //EnumerarFoliosRestantes();
            //AjustarTurnos();
            Debug.WriteLine($"INICIO-PROCESO-AJUSTE-MASIVO-FOLIOS");
        }

        public void EliminarFolios()
        {
            Debug.WriteLine("ELIMINACION-FOLIOS-INICIO");
            TipoRespuesta ObjetivoAlcanzado = TipoRespuesta.NO;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var cheques = context.Cheques
                        .Where(x => x.TipoAccion == TipoAccion.NINGUNO)
                        .OrderBy(x => x.folio)
                        .ToList();

                    foreach (var cheque in cheques)
                    {
                        Debug.WriteLine($"folio: {cheque.folio}. Eliminado");

                        cheque.TipoAccion = TipoAccion.ELIMINAR;

                        var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                        foreach (var det in chequesDetalle)
                        {
                            det.TipoAccion = TipoAccion.ELIMINAR;
                        }

                        var chequesPago = context.ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                        foreach (var chequePago in chequesPago)
                        {
                            chequePago.TipoAccion = TipoAccion.ELIMINAR;
                        }

                        context.SaveChanges();

                        ObjetivoAlcanzado = CalcularImporteNuevo(context);

                        if (ObjetivoAlcanzado == TipoRespuesta.SI)
                        {
                            Debug.WriteLine("Objetivo alcanzado");
                            break;
                        }
                    }

                    //context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }

            Debug.WriteLine("ELIMINACION-FOLIOS-FIN");
        }

        public void EnumerarTurnosYFolios(long idturno, long folio)
        {
            Debug.WriteLine("ENUMERCION-ID-TURNOS-INICIO");
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    //long folioMin = context.Cheques.Min(x => x.folio);
                    long idturnoActual = idturno;
                    long idturnoNuevo = idturno;
                    long folioActual = folio;
                    long folioNuevo = folio;

                    var turnos = context.Turnos
                        .OrderBy(x => x.idturnointerno)
                        .ToList();

                    foreach (var turno in turnos)
                    {
                        Debug.WriteLine($"idturno anterior: {turno.idturno}, idturno nuevo: {idturnoNuevo}");
                        var cheques = context.Cheques
                            .Where(x => x.idturno.Value == (decimal)turno.idturno.Value)
                            .OrderBy(x => x.folio)
                            .ToList();

                        foreach (var cheque in cheques)
                        {
                            Debug.WriteLine($"folio anterior: {cheque.FolioAnt}, folio nuevo: {folioNuevo}");

                            cheque.IdTurnoAnt = idturnoActual;
                            cheque.idturno = idturnoNuevo;

                            var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                            foreach (var det in chequesDetalle)
                            {
                                det.FolioAnt = folioActual;
                                det.foliodet = folioNuevo;
                            }

                            var chequesPago = context.ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                            foreach (var chequePago in chequesPago)
                            {
                                chequePago.FolioAnt = folioActual;
                                chequePago.folio = folioNuevo;
                            }

                            cheque.FolioAnt = folioActual;
                            cheque.folio = folioNuevo;

                            context.SaveChanges();

                            if (cheque.TipoAccion != TipoAccion.ELIMINAR)
                            {
                                folioNuevo++;
                            }
                            folioActual++;
                        }

                        turno.IdTurnoAnt = idturnoActual;
                        turno.idturno = idturnoNuevo;

                        context.SaveChanges();

                        if (turno.TipoAccion != TipoAccion.ELIMINAR)
                        {
                            idturnoNuevo++;
                        }
                        idturnoActual++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
            Debug.WriteLine("ENUMERCION-ID-TURNOS-FIN");
        }

        public void EnumerarFoliosCheques(long folio, long idturno)
        {
            Debug.WriteLine("ENUMERCION-FOLIOS-INICIO");
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    //long folioMin = context.Cheques.Min(x => x.folio);
                    long folioActual = folio;
                    long folioNuevo = folio;

                    var cheques = context.Cheques.OrderBy(x => x.folio).ToList();

                    foreach (var cheque in cheques)
                    {
                        Debug.WriteLine($"folio anterior: {cheque.FolioAnt}, folio nuevo: {folioNuevo}");

                        var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                        foreach (var det in chequesDetalle)
                        {
                            det.FolioAnt = folioActual;
                            det.foliodet = folioNuevo;
                        }

                        var chequesPago = context.ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                        foreach (var chequePago in chequesPago)
                        {
                            chequePago.FolioAnt = folioActual;
                            chequePago.folio = folioNuevo;
                        }

                        cheque.FolioAnt = folioActual;
                        cheque.folio = folioNuevo;

                        context.SaveChanges();

                        if (cheque.TipoAccion != TipoAccion.ELIMINAR)
                        {
                            folioNuevo++;
                        }
                        folioActual++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
            Debug.WriteLine("ENUMERCION-FOLIOS-FIN");
        }

        public void EliminarRegistrosTemporales()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE Turnos");
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE Cheques");
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE Cheques_Detalle");
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE Cheques_Pago");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public void ProcesarProductos()
        {
            Debug.WriteLine($"INICIO-PROCESO-AJUSTE-MASIVO-PRODUCTOS - {DateTime.Now}");
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

            Debug.WriteLine("FIN-PROCESO-AJUSTE-MASIVO-PRODUCTOS");
        }

        public void AjustarTurnos()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var turnos = context.Turnos.Where(x => x.TipoAccion == TipoAccion.NINGUNO).ToList();

                    foreach(var turno in turnos)
                    {
                        var cheques = context.Cheques
                            .Where(x => 
                                x.idturno == turno.idturno && 
                                (x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.MANTENER || x.TipoAccion == TipoAccion.ACTUALIZAR))
                            .ToList();

                        turno.efectivo = cheques.Sum(x => x.efectivo.Value) + turno.fondo.Value;
                        turno.tarjeta = cheques.Sum(x => x.tarjeta.Value);
                        turno.vales = cheques.Sum(x => x.vales.Value);
                        turno.credito = cheques.Sum(x => x.otros.Value);

                        if (turno.EfectivoAnterior != turno.efectivo.Value || 
                            turno.TarjetaAnterior != turno.tarjeta.Value || 
                            turno.ValesAnterior != turno.vales.Value || 
                            turno.CreditoAnterior != turno.credito.Value)
                        {
                            turno.TipoAccion = TipoAccion.ACTUALIZAR;
                        }
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public void AjustarCheques()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var cheques = context.Cheques
                        .Where(x => x.TipoAccion == TipoAccion.NINGUNO)
                        .ToList();

                    foreach (var cheque in cheques)
                    {
                        #region Ajuste del cheque
                        var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();
                        var detEliminados = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.ELIMINAR).ToList();
                        //var detActualizados = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR && !x.Cambiado).ToList();
                        //var detCambiados = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR && x.Cambiado).ToList();
                        var detRestantes = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.ACTUALIZAR).ToList();

                        decimal totalConImpuestos_Det1 = Mat.Redondeo(detRestantes.Sum(x => x.precio.Value * x.cantidad.Value * (100m - x.descuento.Value) / 100m));
                        decimal totalConImpuestos_Det = Mat.Redondeo(detRestantes.Sum(x => x.ImporteCICD));
                        decimal descuentoAplicado = (100m - cheque.descuento.Value) / 100m;
                        decimal totalNuevo = Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);

                        if (cheque.total.Value == totalNuevo) continue;

                        decimal descuento = cheque.descuento.Value / 100m;
                        decimal totalSinImpuestos_Det = Mat.Redondeo(detRestantes.Sum(x => x.ImporteSICD));

                        cheque.TipoAccion = TipoAccion.ACTUALIZAR;
                        cheque.TotalArticulosEliminados = chequesDetalle
                            .Where(x => (x.TipoAccion == TipoAccion.ELIMINAR || (x.TipoAccion == TipoAccion.ACTUALIZAR && x.Cambiado)) && !x.modificador.Value)
                            .Sum(x => x.CantidadAnt);

                        if (!QuitarPropinasManualmente)
                        {
                            cheque.propina = 0;
                            cheque.propinatarjeta = 0;
                            //falta validar lo de las propinas
                        }

                        cheque.totalarticulos = detRestantes.Sum(x => x.cantidad.Value);
                        cheque.subtotal = totalSinImpuestos_Det;
                        cheque.total = totalNuevo;

                        cheque.totalconpropina = cheque.total; // falta validar si la propina se agrega por configuracion

                        cheque.totalconcargo = cheque.total + cheque.cargo;
                        cheque.totalconpropinacargo = cheque.total + cheque.cargo; // falta validar si la propina se agrega por configuracion
                        cheque.descuentoimporte = Mat.Redondeo(totalSinImpuestos_Det * descuento);

                        cheque.efectivo = cheque.total;
                        cheque.tarjeta = 0;
                        cheque.vales = 0;
                        cheque.otros = 0;

                        //if (cheque.TipoPago == TipoPago.EFECTIVO)
                        //{
                        //    cheque.efectivo = cheque.total;
                        //}
                        //else if (cheque.TipoPago == TipoPago.TARJETA)
                        //{
                        //    cheque.tarjeta = cheque.total;
                        //}
                        //else if (cheque.TipoPago == TipoPago.VALES)
                        //{
                        //    cheque.vales = cheque.total;
                        //}
                        //else if (cheque.TipoPago == TipoPago.OTROS)
                        //{
                        //    cheque.otros = cheque.total;
                        //}

                        cheque.totalsindescuento = Mat.Redondeo(detRestantes.Sum(x => x.ImporteSISD));

                        decimal totalalimentos = 0;
                        decimal totalbebidas = 0;
                        decimal totalotros = 0;
                        decimal totalalimentosdescuento = 0;
                        decimal totalbebidasdescuento = 0;
                        decimal totalotrosdescuento = 0;
                        decimal totalalimentossindescuento = 0;
                        decimal totalbebidassindescuento = 0;
                        decimal totalotrossindescuento = 0;

                        foreach (var detalle in detRestantes)
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

                        cheque.totalimpuestod1 = Mat.Redondeo(detRestantes.Sum(x => x.ImporteI1CD));
                        cheque.totalimpuestod2 = Mat.Redondeo(detRestantes.Sum(x => x.ImporteI2CD));
                        cheque.totalimpuestod3 = Mat.Redondeo(detRestantes.Sum(x => x.ImporteI3CD));

                        cheque.totalimpuesto1 = cheque.totalimpuestod1;

                        cheque.desc_imp_original = cheque.descuentoimporte;

                        cheque.cambio = 0; // como se debe ajustar el cambio si es efectivo? - por el momento queda en ceros
                        cheque.cambiorepartidor = 0;
                        #endregion

                        #region Ajuste del cheque pago
                        var chequePago = context.ChequesPago.FirstOrDefault(x => x.folio == cheque.folio);
                        chequePago.importe = cheque.total;
                        chequePago.idformadepago = App.ClavePagoEfectivo;

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

        public void EliminarProductos(decimal movimiento = 1m, bool continuaEliminacion = false)
        {
            Debug.WriteLine($"movimiento: {movimiento}");

            TipoRespuesta ObjetivoAlcanzado = TipoRespuesta.NO;

            if (movimiento == 1)
                continuaEliminacion = false;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var detalles = context.ChequesDetalle
                        .Where(x => !x.modificador.Value && x.movimiento == movimiento && (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                        .OrderBy(x => x.movimiento.Value).ThenBy(x => x.foliodet.Value)
                        .ToList();

                    foreach (var det in detalles)
                    {
                        if (det.descuento.Value == 100m)
                        {
                            Debug.WriteLine($"Producto: {det.idproducto} no eliminado. Descuento de 100%");
                            continue;
                        }

                        if (App.ConfiguracionSistema.EliminarProductosSeleccionados && App.ProductosEliminar.Count > 0 && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto))
                        {
                            Debug.WriteLine($"Producto: {det.idproducto} NO eliminable");
                            continue;
                        }

                        Debug.WriteLine($"folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}, descuento: {det.descuento.Value}");

                        decimal cantidadRestanteUnidad = context.ChequesDetalle
                            .Where(x => x.foliodet == det.foliodet && !x.modificador.Value && (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                            .Sum(x => x.cantidad.Value == Math.Truncate(x.cantidad.Value) ? x.cantidad.Value : 1m);

                        decimal cantidadTotal = context.ChequesDetalle
                            .Where(x => x.foliodet == det.foliodet && !x.modificador.Value && (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                            .Sum(x => x.cantidad.Value);

                        Debug.WriteLine($"Cantidad restante como unidad entera: {cantidadRestanteUnidad}. Cantidad total: {cantidadTotal}");
                        Debug.WriteLine($"Productos minimos por cuenta: {App.ConfiguracionSistema.MinProductosCuenta}");

                        if (cantidadRestanteUnidad <= App.ConfiguracionSistema.MinProductosCuenta)
                        {
                            Debug.WriteLine("Cantidad mínima alcanzada");
                            continue;
                        }

                        bool eliminarTodo = false;
                        string idproductocompuesto = det.idproductocompuesto;

                        if (det.cantidad.Value > 1m && det.cantidad.Value == Math.Truncate(det.cantidad.Value))
                        {
                            Debug.WriteLine($"Eliminar: {1.00m}");

                            det.TipoAccion = TipoAccion.ACTUALIZAR;
                            det.cantidad -= 1m;
                            continuaEliminacion = true;
                        }
                        else
                        {
                            Debug.WriteLine($"Eliminar: {det.cantidad.Value}");
                            det.TipoAccion = TipoAccion.ELIMINAR;
                            det.cantidad = 0;
                            det.idproductocompuesto = "";
                            eliminarTodo = true;
                        }

                        context.SaveChanges();

                        EliminarProductosModificadores(context, det.foliodet.Value, idproductocompuesto, eliminarTodo);

                        ObjetivoAlcanzado = CalcularImporteNuevo(context);

                        if (ObjetivoAlcanzado == TipoRespuesta.SI)
                        {
                            Debug.WriteLine("Objetivo alcanzado");
                            break;
                        }
                    }

                    if (ObjetivoAlcanzado == TipoRespuesta.NO)
                    {
                        movimiento += 1m;

                        if (movimiento >= UltimoMovimiento && continuaEliminacion)
                        {
                            movimiento = 1m;
                        }
                        else if (movimiento >= UltimoMovimiento && !continuaEliminacion)
                        {
                            Debug.WriteLine("NO hay mas producto para eliminar");
                            ObjetivoAlcanzado = TipoRespuesta.SI;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }

            if (ObjetivoAlcanzado == TipoRespuesta.NO)
            {
                EliminarProductos(movimiento, continuaEliminacion);
                return;
            }

            Debug.WriteLine("FIN-ELIMINACION");
        }

        private void EliminarProductosModificadores(ApplicationDbContext context, long foliodet, string idproductocompuesto, bool eliminarTodo = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idproductocompuesto)) return;

                Debug.WriteLine($"Eliminar modificadores con idproductocompuesto: {idproductocompuesto}. {(eliminarTodo ? "TODO" : "")}");

                Debug.WriteLine("INICIO-ELIMINACION-MODIFICADORES");
                var detalles = context.ChequesDetalle
                    .Where(x => 
                        x.foliodet.Value == foliodet && 
                        x.idproductocompuesto.Equals(idproductocompuesto) && 
                        x.modificador.Value &&
                        (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                    .ToList();

                foreach (var det in detalles)
                {
                    Debug.WriteLine($"folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}");

                    if (App.ConfiguracionSistema.EliminarProductosSeleccionados && App.ProductosEliminar.Count > 0 && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto) && !eliminarTodo)
                    {
                        Debug.WriteLine("Producto NO eliminable");
                        continue;
                    }

                    if (det.cantidad.Value > 1m && det.cantidad.Value == Math.Truncate(det.cantidad.Value) && !eliminarTodo)
                    {
                        Debug.WriteLine($"Eliminar: {1.00m}");

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

            if (App.ProductosReemplazo.Count == 0)
            {
                Debug.WriteLine("No hay productos para reemplazo");
                return;
            }

            Debug.WriteLine("INICIO-CAMBIO-PRODUCTOS");

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var detalles = context.ChequesDetalle
                    .Where(x => !x.modificador.Value && (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                    .OrderBy(x => x.movimiento).ThenBy(x => x.foliodet)
                    .ToList();

                foreach (var det in detalles)
                {
                    if (App.ConfiguracionSistema.EliminarProductosSeleccionados && App.ProductosEliminar.Count > 0 && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto))
                    {
                        Debug.WriteLine($"Producto: {det.idproducto} NO eliminable");
                        continue;
                    }

                    Debug.WriteLine($"folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}, descuento: {det.descuento}");

                    int porcentajeAleatorio = Random.Next(1, 100);
                    int porcentajeAnterior = 0;
                    int porcentajeActual = 0;
                    Debug.WriteLine($"Porcentaje aleatorio: {porcentajeAleatorio}");

                    foreach (var productoReemplazo in App.ProductosReemplazo)
                    {
                        porcentajeActual += productoReemplazo.Porcentaje;

                        if (porcentajeAleatorio > porcentajeAnterior && porcentajeAleatorio <= porcentajeActual)
                        {
                            var detalleProducto = App.SRProductosDetalle.Find(x => x.idproducto == productoReemplazo.Clave);
                            decimal precioUnidad = det.precio.Value;

                            if (det.cantidad.Value != Math.Truncate(det.cantidad.Value))
                                precioUnidad = det.precio.Value * det.cantidad.Value;

                            Debug.WriteLine($"producto reemplazo: {productoReemplazo.Clave}, porcentaje: {productoReemplazo.Porcentaje}, precio: {detalleProducto.precio}, precio por unidad: {precioUnidad}");

                            if (detalleProducto.precio < precioUnidad)
                            {
                                string idproductocompuesto = det.idproductocompuesto;

                                det.TipoAccion = TipoAccion.ACTUALIZAR;
                                det.Cambiado = true;

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

                                context.SaveChanges();

                                Debug.WriteLine("producto reemplazado");

                                EliminarProductosModificadores(context, det.foliodet.Value, idproductocompuesto, true);
                            }
                            else
                            {
                                Debug.WriteLine("producto NO reemplazado");
                            }

                            break;
                        }

                        porcentajeAnterior += productoReemplazo.Porcentaje;
                    }

                    CalcularImporteNuevo(context);

                    if (ImporteNuevo <= ImporteObjetivo) break;
                }
            }
            Debug.WriteLine("FIN-CAMBIO-PRODUCTOS");
        }

        public TipoRespuesta CalcularImporteNuevo(ApplicationDbContext context)
        {
            try
            {
                decimal importeNuevo = 0;
                //var cheques = new List<Cheque>();

                //if (Proceso.TipoProceso == TipoProceso.FOLIOS)
                //    cheques = context.Cheques.ToList();
                //else if (Proceso.TipoProceso == TipoProceso.PRODUCTOS)
                //    cheques = context.Cheques.Where(x => x.TipoAccion == TipoAccion.ELIMINAR).ToList();
                var cheques = context.Cheques
                    .Where(x => x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.MANTENER || x.TipoAccion == TipoAccion.ACTUALIZAR)
                    .ToList();

                if (Proceso.TipoProceso == TipoProceso.FOLIOS)
                {
                    importeNuevo = cheques.Sum(x => x.total.Value);
                }
                else if (Proceso.TipoProceso == TipoProceso.PRODUCTOS)
                {
                    foreach (var cheque in cheques)
                    {
                        var chequesDetalle = context.ChequesDetalle.Where(x => x.foliodet == cheque.folio).ToList();

                        decimal totalConImpuestos_Det = Mat.Redondeo(chequesDetalle.Sum(x => x.ImporteCICD));
                        decimal descuentoAplicado = (100m - cheque.descuento.Value) / 100m;
                        importeNuevo += Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);
                    }
                }

                ImporteNuevo = importeNuevo;

                Debug.WriteLine($"ImporteAnterior: {ImporteAnterior}, ImporteObjetivo: {ImporteObjetivo}, ImporteNuevo: {ImporteNuevo}");

                return ImporteNuevo <= ImporteObjetivo ? TipoRespuesta.SI : TipoRespuesta.NO;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                return TipoRespuesta.ERROR;
            }
        }

        public void InicializarControles()
        {
            DetalleModificacionCheques = new ObservableCollection<DetalleAjuste>();
            UltimoMovimiento = 0;
            FechaCorteInicio = DateTime.Today.AddDays(-1);
            FechaCorteCierre = DateTime.Today.AddDays(-1);
            Turno = true;
            Periodo = false;
            FechaInicio = DateTime.Today.AddDays(-1);
            FechaCierre = DateTime.Today.AddDays(-1);
            CorteInicio = App.SRConfiguracion.CorteInicio;
            CorteCierre = App.SRConfiguracion.CorteCierre;
            HorarioTurno = $"{App.SRConfiguracion.cortezinicio} - {App.SRConfiguracion.cortezfin}";
            Proceso = Procesos.Find(x => x.TipoProceso == TipoProceso.PRODUCTOS);
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

        public RespuestaBusqueda ObtenerChequesSR()
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

                    if (!Funciones.ValidarMesBusqueda(App.MesesValidos, FechaCorteInicio))
                    {
                        return new RespuestaBusqueda
                        {
                            TipoRespuesta = TipoRespuesta.FECHA_INACCESIBLE,
                            Mensaje = FechaCorteInicio.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es"))
                        };
                    }

                    if (!Funciones.ValidarMesBusqueda(App.MesesValidos, FechaCorteCierre))
                    {
                        return new RespuestaBusqueda
                        {
                            TipoRespuesta = TipoRespuesta.FECHA_INACCESIBLE,
                            Mensaje = FechaCorteCierre.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es"))
                        };
                    }

                    SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, false);

                    List<SR_turnos> turnos = new List<SR_turnos>();

                    if (App.ConfiguracionSistema.ModificarVentasReales)
                    {
                        turnos = turnos_DAO.Get($"apertura >= @{nameof(FechaCorteInicio)} AND idempresa=@{nameof(App.ClaveEmpresa)}",
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));
                    }
                    else
                    {
                        turnos = turnos_DAO.Get($"apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND idempresa=@{nameof(App.ClaveEmpresa)}",
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));
                    }

                    if (turnos.Count == 0) return new RespuestaBusqueda
                    {
                        TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                    };

                    var idsturno = turnos.Select(x => (object)x.idturno).ToArray();

                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, false);
                    var cheques = cheques_DAO.WhereIn("idturno", idsturno);

                    if (cheques.Count == 0) return new RespuestaBusqueda
                    {
                        TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                    };

                    var folios = cheques.Select(x => (object)x.folio).ToArray();

                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, false);
                    var cheqdet = cheqdet_DAO.WhereIn("foliodet", folios);

                    SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, false);
                    var chequespagos = chequespagos_DAO.WhereIn("folio", folios);

                    SR_formasdepago_DAO formasdepago_DAO = new SR_formasdepago_DAO(context);
                    var formasdepago = formasdepago_DAO.GetAll();

                    //UltimoMovimiento = cheqdet.Max(x => x.movimiento.Value);
                    //ImporteAnterior = cheques.Sum(x => x.total.Value);
                    //ImporteObjetivo = ImporteAnterior * (100m - PorcentajeObjetivo) / 100;
                    //NumeroTotalCuentas = cheques.Count;
                    //EfectivoAnterior = cheques.Sum(x => x.efectivo.Value);

                    return new RespuestaBusqueda
                    {
                        TipoRespuesta = TipoRespuesta.HECHO,
                        Turnos = turnos,
                        Cheques = cheques.Where(x => x.idempresa.Equals(App.ClaveEmpresa)).ToList(),
                        Cheqdet = cheqdet,
                        Chequespagos = chequespagos,
                        Formasdepago = formasdepago,
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return new RespuestaBusqueda
                    {
                        TipoRespuesta = TipoRespuesta.ERROR
                    };
                }
            }
        }

        public void CrearRegistrosTemporales(RespuestaBusqueda respuesta)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    List<Turno> turnos = new List<Turno>();
                    List<Cheque> cheques = new List<Cheque>();
                    List<ChequePago> chequesPago = new List<ChequePago>();
                    List<ChequeDetalle> chequesDetalle = new List<ChequeDetalle>();

                    foreach (var turno in respuesta.Turnos)
                    {
                        TipoAccion tipoAccion = (turno.apertura >= FechaCorteInicio && turno.apertura <= FechaCorteCierre) ? TipoAccion.NINGUNO : TipoAccion.OMITIR;
                        turnos.Add(Funciones.ParseTurno(turno, tipoAccion));
                    }

                    foreach (var cheque in respuesta.Cheques)
                    {
                        var turno = turnos.Find(x => x.idturno == cheque.idturno);
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

                        TipoAccion tipoAccion = turno.TipoAccion;

                        if (tipoAccion == TipoAccion.NINGUNO)
                        {
                            tipoAccion = (
                                cheque.total.Value >= ImporteMinimoAjustable &&
                                    (
                                        tipoPago == TipoPago.EFECTIVO ||
                                        (IncluirCuentaPagadaTarjeta ? tipoPago == TipoPago.TARJETA : false) ||
                                        (IncluirCuentaPagadaVales ? tipoPago == TipoPago.VALES : false) ||
                                        (IncluirCuentaPagadaOtros ? tipoPago == TipoPago.OTROS : false)
                                    ) &&
                                    (IncluirCuentaFacturada ? true : !cheque.facturado.Value) &&
                                    (IncluirCuentaNotaConsumo ? true : cheque.folionotadeconsumo.Value == 0) &&
                                    (NoIncluirCuentasReimpresas ? cheque.impresiones.Value == 1 : true)
                                ) ? TipoAccion.NINGUNO : TipoAccion.MANTENER;
                        }

                        cheques.Add(Funciones.ParseCheque(cheque, tipoAccion, tipoPago));
                    }

                    foreach (var chequepago in respuesta.Chequespagos)
                    {
                        var cheque = cheques.Find(x => x.folio == chequepago.folio);
                        chequesPago.Add(Funciones.ParseChequePago(chequepago, cheque.TipoAccion));
                    }

                    foreach (var det in respuesta.Cheqdet)
                    {
                        var cheque = cheques.Find(x => x.folio == det.foliodet.Value);
                        TipoClasificacion tipoClasificacion = ObtenerClasificacionProductoSR(det.idproducto);
                        chequesDetalle.Add(Funciones.ParseChequeDetalle(det, cheque.TipoAccion, tipoClasificacion));
                    }

                    UltimoMovimiento = chequesDetalle.Where(x => x.TipoAccion != TipoAccion.OMITIR).Max(x => x.movimiento.Value);
                    ImporteAnterior = cheques.Where(x => x.TipoAccion != TipoAccion.OMITIR).Sum(x => x.total.Value);
                    ImporteObjetivo = ImporteAnterior * (100m - PorcentajeObjetivo) / 100;
                    NumeroTotalCuentas = cheques.Count(x => x.TipoAccion != TipoAccion.OMITIR);
                    EfectivoAnterior = cheques.Where(x => x.TipoAccion != TipoAccion.OMITIR).Sum(x => x.efectivo.Value);

                    context.Turnos.AddRange(turnos);
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
                var cheques = context.Cheques.Where(x => x.TipoAccion != TipoAccion.OMITIR).ToList();
                var fondoTurnos = context.Turnos.Where(x => x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.ACTUALIZAR).Sum(x => x.fondo.Value);

                NumeroTotalCuentasModificadas = cheques.Count(x => x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.ELIMINAR);
                Diferencia = ImporteAnterior - ImporteNuevo;
                PorcentajeDiferencia = Math.Round(Diferencia / ImporteAnterior * 100m, 2, MidpointRounding.AwayFromZero);
                EfectivoNuevo = cheques
                    .Where(x => x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.MANTENER || x.TipoAccion == TipoAccion.ACTUALIZAR)
                    .Sum(x => x.efectivo.Value);
                EfectivoCaja = EfectivoNuevo + fondoTurnos;

                foreach (var cheque in cheques)
                {
                    decimal totalconDescuento = 0;

                    if (Proceso.TipoProceso == TipoProceso.FOLIOS)
                        totalconDescuento = cheque.total.Value;
                    else if (Proceso.TipoProceso == TipoProceso.PRODUCTOS)
                        totalconDescuento = cheque.total.Value;

                    DetalleModificacionCheques.Add(new DetalleAjuste
                    {
                        Folio = (int)cheque.folionotadeconsumo.Value,
                        FolioNotaConsumo = (int)cheque.folionotadeconsumo.Value,
                        Fecha = cheque.fecha.Value,
                        Cancelado = cheque.cancelado.Value ? TipoLogico.SI : TipoLogico.NO,
                        Facturado = cheque.facturado.Value ? TipoLogico.SI : TipoLogico.NO,
                        Descuento = cheque.descuento.Value,
                        TotalOriginal = cheque.TotalAnt,
                        TotalArticulos = cheque.totalarticulos.Value,
                        ProductosEliminados = cheque.TotalArticulosEliminados,
                        TotalConDescuento = totalconDescuento,
                        Efectivo = cheque.EfectivoAnt,
                        Tarjeta = cheque.TarjetaAnt,
                        Vales = cheque.ValesAnt,
                        Otros = cheque.OtrosAnt,
                        RealizarAccion = cheque.TipoAccion == TipoAccion.ACTUALIZAR || cheque.TipoAccion == TipoAccion.ELIMINAR,
                        IsEnable = cheque.TipoAccion == TipoAccion.ACTUALIZAR || cheque.TipoAccion == TipoAccion.ELIMINAR,
                    });
                }
            }
        }

        //public void ResgistrarBitacora()
        //{
        //    var ChequesAModificar = DetalleModificacionCheques.Where(x => x.RealizarAccion).ToList();
        //    var importeNuevo = ChequesAModificar.Sum(x => x.TotalConDescuento);

        //    Funciones.RegistrarModificacion(new BitacoraModificacion
        //    {
        //        TipoAjuste = TipoAjuste.MASIVO,
        //        FechaProceso = DateTime.Now,
        //        FechaInicialVenta = FechaCorteInicio,
        //        FechaFinalVenta = FechaCorteCierre,
        //        TotalCuentas = NumeroTotalCuentas,
        //        CuentasModificadas = NumeroTotalCuentasModificadas,
        //        ImporteAnterior = ImporteAnterior,
        //        ImporteNuevo = ImporteNuevo,
        //        Diferencia = PorcentajeDiferencia,
        //    });
        //}
        
        public decimal UltimoMovimiento { get; set; }
        public DateTime FechaCorteInicio { get; set; }
        public DateTime FechaCorteCierre { get; set; }
        private TimeSpan CorteInicio { get; set; }
        private TimeSpan CorteCierre { get; set; }

        private string headerAccion;
        public string HeaderAccion
        {
            get { return headerAccion; }
            set
            {
                headerAccion = value;
                OnPropertyChanged(nameof(HeaderAccion));
            }
        }

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

        private Proceso _Proceso;
        public Proceso Proceso
        {
            get { return _Proceso; }
            set
            {
                _Proceso = value;
                OnPropertyChanged(nameof(Proceso));
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
