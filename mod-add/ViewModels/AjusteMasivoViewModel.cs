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

        public TipoRespuesta Guardar()
        {
            Debug.WriteLine("GUARDADO-INICIO");
            TipoRespuesta tipoRespuesta = TipoRespuesta.NADA;

            InvertirCambiosDesmarcados();

            if (Proceso.TipoProceso == TipoProceso.PRODUCTOS)
            {
                EnumerarMovimientosChequesDetalle();
            }

            AjustarTurnos();

            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        string query;
                        //List<string> nombresParametros = new List<string>();
                        //object[] parametros;
                        //object[] valores;
                        string tablaCheques;
                        string tablaTurnos;
                        //long folio;
                        //decimal numcheque;
                        //long idturnointerno;
                        //long idturno;

                        SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_cheqdetfeliminados_DAO cheqdetfeliminados_DAO = new SR_cheqdetfeliminados_DAO(context);
                        SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_bitacorafiscal_DAO bitacorafiscal_DAO = new SR_bitacorafiscal_DAO(context);

                        Debug.WriteLine("Inicio de la transaccion");

                        #region Eliminacion de registros
                        Debug.WriteLine("Eliminando bitacora fiscal");
                        query = $"(@{nameof(FechaCorteInicio)} BETWEEN fechainicial AND fechafinal) AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        bitacorafiscal_DAO.Delete(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));

                        if (App.ConfiguracionSistema.ModificarVentasReales)
                        {
                            EliminarRegistrosReales(context);
                        }
                        else
                        {
                            EliminarRegistrosFiscales(context);
                        }

                        #endregion


                        if (App.ConfiguracionSistema.ModificarVentasReales)
                        {
                            tablaCheques = "cheques";
                            tablaTurnos = "turnos";
                        }
                        else
                        {
                            tablaCheques = "chequesf";
                            tablaTurnos = "turnosf";
                        }

                        query = $"SELECT CAST(ISNULL(MAX(idturnointerno), 0) AS bigint) AS idturnointerno FROM {tablaTurnos}";
                        long idturnointerno = context.Database.SqlQuery<long>(query).Single();

                        query = $"SELECT CAST(ISNULL(MAX(idturno), 0) AS bigint) AS idturno FROM {tablaTurnos}";
                        long idturno = context.Database.SqlQuery<long>(query).Single();

                        query = $"SELECT CAST(ISNULL(MAX(folio), 0) AS bigint) AS folio FROM {tablaCheques}";
                        long folio = context.Database.SqlQuery<long>(query).Single();

                        query = $"SELECT CAST(ISNULL(MAX(numcheque), 0) AS numeric(8,0)) AS numcheque FROM {tablaCheques}";
                        decimal numcheque = context.Database.SqlQuery<decimal>(query).Single();

                        query = $"DBCC CHECKIDENT ({tablaCheques}, RESEED, @{nameof(folio)})";
                        context.Database.ExecuteSqlCommand(query, new SqlParameter($"{nameof(folio)}", folio));

                        query = $"DBCC CHECKIDENT ({tablaTurnos}, RESEED, @{nameof(idturnointerno)})";
                        context.Database.ExecuteSqlCommand(query, new SqlParameter($"{nameof(idturnointerno)}", idturnointerno));

                        //idturnointerno = idturnointerno > 0 ? idturnointerno + 1 : 0;
                        idturno++;
                        folio = folio > 0 ? folio + 1 : 0;
                        numcheque++;

                        EnumerarTurnosYFolios(idturno, folio, numcheque);

                        #region Creacion y actualizacion de registros
                        Debug.WriteLine("Recreando turnos");
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
                        Debug.WriteLine("Transaccion interrumpida");
                        Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                        tipoRespuesta = TipoRespuesta.ERROR;
                    }
                }
            }

            Debug.WriteLine("GUARDADO-FIN");
            return tipoRespuesta;
        }

        public void EliminarRegistrosFiscales(SoftRestaurantDBContext context)
        {
            string query;
            List<string> nombresParametros = new List<string>();
            object[] valores;
            object[] parametrosSql;
            List<long> result = new List<long>();

            query = $"SELECT CAST(ISNULL(folio, 0) AS bigint) AS folio FROM chequesf WHERE idturno IN (SELECT idturno FROM turnosf WHERE apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND idempresa=@{nameof(App.ClaveEmpresa)})";
            result = context.Database.SqlQuery<long>(query, 
                new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
            .ToList();

            if (result.Count == 0) return;

            nombresParametros.Clear();
            valores = result.OrderBy(x => x).Select(x => (object)x).ToArray();
            Debug.WriteLine($"Folios a eliminar {string.Join(",", valores)}");
            parametrosSql = new object[valores.Length + 1];
            parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

            for (int i = 0; i < valores.Length; i++)
            {
                string nombreParametro = $"p{i + 1}";
                nombresParametros.Add($"@{nombreParametro}");
                parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
            }

            query = $"DELETE FROM cheqdetfeliminados WHERE foliodet IN ({string.Join(",", nombresParametros)})";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            query = $"DELETE FROM chequespagosf WHERE folio IN ({string.Join(",", nombresParametros)})";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            query = $"DELETE FROM cheqdetf WHERE folio IN ({string.Join(",", nombresParametros)})";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            query = $"DELETE FROM chequesf WHERE folio IN ({string.Join(",", nombresParametros)}) AND idempresa=@{nameof(App.ClaveEmpresa)}";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            query = $"SELECT CAST(ISNULL(idturno, 0) AS bigint) AS idturno FROM turnosf WHERE apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND idempresa=@{nameof(App.ClaveEmpresa)}";
            result = context.Database.SqlQuery<long>(query,
                new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
            .ToList();

            nombresParametros.Clear();
            valores = result.OrderBy(x => x).Select(x => (object)x).ToArray();
            Debug.WriteLine($"Turnos a eliminar {string.Join(",", valores)}");
            parametrosSql = new object[valores.Length + 1];
            parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

            for (int i = 0; i < valores.Length; i++)
            {
                string nombreParametro = $"p{i + 1}";
                nombresParametros.Add($"@{nombreParametro}");
                parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
            }

            query = $"DELETE FROM turnos WHERE idturno IN ({string.Join(",", nombresParametros)}) AND idempresa=@{nameof(App.ClaveEmpresa)}";
            context.Database.ExecuteSqlCommand(query, parametrosSql);
        }

        public void EliminarRegistrosReales(SoftRestaurantDBContext context)
        {
            string query;
            List<string> nombresParametros = new List<string>();
            object[] valores;
            object[] parametrosSql;

            nombresParametros.Clear();
            valores = Cheques.OrderBy(x => x.folio).Select(x => (object)x.folio).ToArray();
            Debug.WriteLine($"Folios a eliminar {string.Join(",", valores)}");
            parametrosSql = new object[valores.Length + 1];
            parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

            for (int i = 0; i < valores.Length; i++)
            {
                string nombreParametro = $"p{i + 1}";
                nombresParametros.Add($"@{nombreParametro}");
                parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
            }

            query = $"DELETE FROM chequespagos WHERE folio IN ({string.Join(",", nombresParametros)})";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            query = $"DELETE FROM cheqdet WHERE folio IN ({string.Join(",", nombresParametros)})";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            query = $"DELETE FROM cheques WHERE folio IN ({string.Join(",", nombresParametros)}) AND idempresa=@{nameof(App.ClaveEmpresa)}";
            context.Database.ExecuteSqlCommand(query, parametrosSql);

            nombresParametros.Clear();
            valores = Turnos.OrderBy(x => x.idturno).Select(x => (object)x.idturno).ToArray();
            Debug.WriteLine($"Turnos a eliminar {string.Join(",", valores)}");
            parametrosSql = new object[valores.Length + 1];
            parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

            for (int i = 0; i < valores.Length; i++)
            {
                string nombreParametro = $"p{i + 1}";
                nombresParametros.Add($"@{nombreParametro}");
                parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
            }

            query = $"DELETE FROM turnos WHERE idturno IN ({string.Join(",", nombresParametros)}) AND idempresa=@{nameof(App.ClaveEmpresa)}";
            context.Database.ExecuteSqlCommand(query, parametrosSql);
        }

        public void InvertirCambiosDesmarcados()
        {
            try
            {
                var cheques = Cheques.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.ELIMINAR).ToList();

                foreach (var cheque in cheques)
                {
                    bool realizarAccion = DetalleModificacionCheques.Where(x => x.FolioCuenta == cheque.numcheque).Select(x => x.RealizarAccion).First();

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
                        continue;
                    }

                    var chequesDetalle = ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

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

                    var chequesPago = ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                    foreach (var chequePago in chequesPago)
                    {
                        chequePago.TipoAccion = TipoAccion.NINGUNO;
                        chequePago.importe = chequePago.ImporteAnt;
                        chequePago.propina = chequePago.PropinaAnt;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void EnumerarMovimientosChequesDetalle()
        {
            var cheques = Cheques.Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR).ToList();

            foreach (var cheque in cheques)
            {
                var chequesDetalle = ChequesDetalle.
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
            }
        }

        public void RecrearTurnosSR(SR_turnos_DAO turnos_DAO)
        {
            var turnos = Turnos
                .Where(x => x.TipoAccion != TipoAccion.ELIMINAR && (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR)))
                .ToList();

            foreach (var turno in turnos)
            {
                turnos_DAO.Create(Funciones.ParseSR_turnos(turno));
            }
        }

        public void RecrearChequesSR(SR_cheques_DAO cheques_DAO)
        {
            var cheques = Cheques
                .Where(x => x.TipoAccion != TipoAccion.ELIMINAR && (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR)))
                .ToList();

            foreach (var cheque in cheques)
            {
                cheques_DAO.Create(Funciones.ParseSR_cheques(cheque));
            }
        }

        public void RecrearChequesDetalleSR(SR_cheqdet_DAO cheqdet_DAO)
        {
            var chequesDetalle = ChequesDetalle
                .Where(x => x.TipoAccion != TipoAccion.ELIMINAR && (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR)))
                .ToList();

            foreach (var chequeDetalle in chequesDetalle)
            {
                cheqdet_DAO.Create(Funciones.ParseSR_cheqdet(chequeDetalle));
            }
        }

        public void RecrearChequesDetalleEliminadosSR(SR_cheqdetfeliminados_DAO cheqdetfeliminados_DAO)
        {
            var chequesDetalle = ChequesDetalle
                .Where(x => x.TipoAccion == TipoAccion.ELIMINAR && (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR)))
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
            var chequesPago = ChequesPago
                .Where(x => x.TipoAccion != TipoAccion.ELIMINAR && (App.ConfiguracionSistema.ModificarVentasReales ? true : (x.TipoAccion != TipoAccion.OMITIR)))
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
        }

        private void ProcesarFolios()
        {
            Debug.WriteLine($"INICIO-PROCESO-AJUSTE-MASIVO-FOLIOS - {DateTime.Now}");
            EliminarFolios();
            Debug.WriteLine($"FIN-PROCESO-AJUSTE-MASIVO-FOLIOS");
        }

        public void EliminarFolios()
        {
            Debug.WriteLine("ELIMINACION-FOLIOS-INICIO");
            TipoRespuesta ObjetivoAlcanzado = TipoRespuesta.NO;

            try
            {
                var cheques = Cheques
                    .Where(x => x.TipoAccion == TipoAccion.NINGUNO)
                    .ToList();

                foreach (var cheque in cheques)
                {
                    Debug.WriteLine($"Info. - folio: {cheque.folio}. Eliminado");

                    cheque.TipoAccion = TipoAccion.ELIMINAR;

                    var chequesDetalle = ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                    foreach (var det in chequesDetalle)
                    {
                        det.TipoAccion = TipoAccion.ELIMINAR;
                    }

                    var chequesPago = ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                    foreach (var chequePago in chequesPago)
                    {
                        chequePago.TipoAccion = TipoAccion.ELIMINAR;
                    }

                    ObjetivoAlcanzado = CalcularImporteNuevo();

                    if (ObjetivoAlcanzado == TipoRespuesta.SI)
                    {
                        Debug.WriteLine("Objetivo alcanzado");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }

            Debug.WriteLine("ELIMINACION-FOLIOS-FIN");
        }

        public void EnumerarTurnosYFolios(long idturno, long folio, decimal numcheque)
        {
            Debug.WriteLine("ENUMERCION-ID-TURNOS-INICIO");

            try
            {
                var turnos = Turnos
                    .OrderBy(x => x.idturno)
                    .ToList();

                foreach (var turno in turnos)
                {
                    var cheques = Cheques
                        .Where(x => x.idturno.Value == turno.idturno.Value)
                        .OrderBy(x => x.numcheque.Value)
                        .ToList();

                    foreach (var cheque in cheques)
                    {
                        var chequesDetalle = ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                        foreach (var det in chequesDetalle)
                        {
                            det.foliodet = folio;
                        }

                        var chequesPago = ChequesPago.Where(x => x.folio == cheque.folio).ToList();

                        foreach (var chequePago in chequesPago)
                        {
                            chequePago.folio = folio;
                        }

                        if (cheque.TipoAccion != TipoAccion.ELIMINAR)
                        {
                            Debug.WriteLine($"Folio anterior: {cheque.folio}, folio nuevo: {folio}");
                            Debug.WriteLine($"Número de cheque anterior: {cheque.numcheque}, Número de cheque nuevo: {numcheque}");
                            cheque.idturno = idturno;
                            cheque.folio = folio;
                            cheque.numcheque = numcheque;
                            if (cheque.folionotadeconsumo > 0) cheque.folionotadeconsumo = numcheque;

                            folio++;
                            numcheque++;
                        }
                    }

                    if (turno.TipoAccion != TipoAccion.ELIMINAR)
                    {
                        Debug.WriteLine($"idturno anterior: {turno.idturno}, idturno nuevo: {idturno}");
                        turno.idturno = idturno;
                        idturno++;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
            Debug.WriteLine("ENUMERCION-ID-TURNOS-FIN");
        }

        public void ProcesarProductos()
        {
            Debug.WriteLine($"INICIO-PROCESO-AJUSTE-MASIVO-PRODUCTOS - {DateTime.Now}");
            try
            {
                Debug.WriteLine("INICIO-ELIMINACION-PRODUTOS");
                int maxMovimietos = ChequesDetalle.GroupBy(x => x.foliodet.Value).Select(x => x.Count()).Max();
                int maxIndex = maxMovimietos - 1;
                Debug.WriteLine($"Indice máximo: {maxIndex}");
                EliminarProductos(maxIndex, 0, false);
                Debug.WriteLine("FIN-ELIMINACION-PRODUTOS");

                CambiarProductos();
                AjustarCheques();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }

            Debug.WriteLine("FIN-PROCESO-AJUSTE-MASIVO-PRODUCTOS");
        }

        public void AjustarTurnos()
        {
            try
            {
                var turnos = Turnos.Where(x => x.TipoAccion == TipoAccion.NINGUNO).ToList();

                foreach (var turno in turnos)
                {
                    var cheques = Cheques
                        .Where(x => x.idturno == turno.idturno && (x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.MANTENER || x.TipoAccion == TipoAccion.ACTUALIZAR))
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void AjustarCheques()
        {
            try
            {
                var cheques = Cheques
                    .Where(x => x.TipoAccion == TipoAccion.NINGUNO)
                    .ToList();

                foreach (var cheque in cheques)
                {
                    #region Ajuste del cheque
                    var chequesDetalle = ChequesDetalle.Where(x => x.foliodet.Value == cheque.folio).ToList();

                    //var detEliminados = chequesDetalle.Where(x => x.TipoAccion == TipoAccion.ELIMINAR).ToList();
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
                    var chequePago = ChequesPago.FirstOrDefault(x => x.folio == cheque.folio);

                    chequePago.importe = cheque.total;
                    chequePago.idformadepago = App.ClavePagoEfectivo;

                    if (cheque.TipoPago == TipoPago.TARJETA)
                    {
                        chequePago.propina = cheque.propina;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public bool EliminarProductos(int maxIndex, int index, bool continuaEliminacion)
        {
            Debug.WriteLine($"Indice actual: {index}");

            TipoRespuesta ObjetivoAlcanzado = TipoRespuesta.NO;

            if (index == 0)
                continuaEliminacion = false;

            try
            {
                var cheques = Cheques.Where(x => x.TipoAccion == TipoAccion.NINGUNO).ToList();

                foreach (var cheque in cheques)
                {
                    var detalles = ChequesDetalle
                        .Where(x => x.foliodet.Value == cheque.folio && !x.modificador.Value)
                        .OrderByDescending(x => x.PrecionEnUnidad)
                        .ToList();

                    if (index >= detalles.Count)
                    {
                        continue;
                    }

                    var det = detalles[index];

                    if (det.TipoAccion == TipoAccion.ELIMINAR)
                    {
                        continue;
                    }

                    Debug.WriteLine($"Info. - folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}, descuento: {det.descuento.Value}");

                    if (det.descuento.Value == 100m)
                    {
                        Debug.WriteLine($"Producto no eliminado. Descuento de 100%");
                        continue;
                    }

                    if (App.ConfiguracionSistema.EliminarProductosSeleccionados && App.ProductosEliminar.Count > 0 && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto))
                    {
                        Debug.WriteLine($"Producto NO eliminable");
                        continue;
                    }

                    decimal cantidadRestanteEnUnidad = detalles
                        .Where(x => (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO) && x.descuento.Value != 100m)
                        .Sum(x => x.cantidad.Value == Math.Truncate(x.cantidad.Value) ? x.cantidad.Value : 1m);

                    decimal cantidadTotal = detalles
                        .Where(x => x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO && x.descuento.Value != 100m)
                        .Sum(x => x.cantidad.Value);

                    Debug.WriteLine($"Cantidad restante como unidad entera: {cantidadRestanteEnUnidad}. Cantidad total: {cantidadTotal}");
                    Debug.WriteLine($"Productos minimos por cuenta: {App.ConfiguracionSistema.MinProductosCuenta}");

                    if (cantidadRestanteEnUnidad <= App.ConfiguracionSistema.MinProductosCuenta)
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

                    EliminarProductosModificadores(det.foliodet.Value, idproductocompuesto, eliminarTodo);

                    ObjetivoAlcanzado = CalcularImporteNuevo();

                    if (ObjetivoAlcanzado == TipoRespuesta.SI)
                    {
                        Debug.WriteLine("Objetivo alcanzado");
                        break;
                    }
                }

                if (ObjetivoAlcanzado == TipoRespuesta.NO)
                {
                    index += 1;

                    if (index > maxIndex && continuaEliminacion)
                    {
                        index = 1;
                    }
                    else if (index > maxIndex && !continuaEliminacion)
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

            if (ObjetivoAlcanzado == TipoRespuesta.NO)
            {
                return EliminarProductos(maxIndex, index, continuaEliminacion);
            }

            return true;
        }

        private void EliminarProductosModificadores(long foliodet, string idproductocompuesto, bool eliminarTodo = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idproductocompuesto)) return;
                Debug.WriteLine("INICIO-ELIMINACION-MODIFICADORES");

                Debug.WriteLine($"Eliminar modificadores con idproductocompuesto: {idproductocompuesto}. {(eliminarTodo ? "TODO" : "")}");

                var detalles = ChequesDetalle
                    .Where(x =>
                        x.foliodet.Value == foliodet &&
                        x.idproductocompuesto.Equals(idproductocompuesto) &&
                        x.modificador.Value &&
                        (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                    .ToList();

                foreach (var det in detalles)
                {
                    if (App.ConfiguracionSistema.EliminarProductosSeleccionados && App.ProductosEliminar.Count > 0 && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto) && !eliminarTodo)
                    {
                        Debug.WriteLine($"Producto {det.idproducto} NO eliminable");
                        continue;
                    }

                    Debug.WriteLine($"Info. - folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}, descuento: {det.descuento.Value}");



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

            var detalles = ChequesDetalle
                    .Where(x => !x.modificador.Value && (x.TipoAccion == TipoAccion.ACTUALIZAR || x.TipoAccion == TipoAccion.NINGUNO))
                    .OrderByDescending(x => x.movimiento.Value).ThenBy(x => x.foliodet)
                    .ToList();

            foreach (var det in detalles)
            {
                if (App.ConfiguracionSistema.EliminarProductosSeleccionados && App.ProductosEliminar.Count > 0 && !App.ProductosEliminar.Any(x => x.Clave == det.idproducto))
                {
                    Debug.WriteLine($"Producto {det.idproducto} NO eliminable");
                    continue;
                }

                Debug.WriteLine($"Info. - folio: {det.foliodet}, movimiento: {det.movimiento}, producto: {det.idproducto}, precio: {det.precio}, cantidad: {det.cantidad}, descuento: {det.descuento.Value}");

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


                            Debug.WriteLine("producto reemplazado");

                            EliminarProductosModificadores(det.foliodet.Value, idproductocompuesto, true);
                        }
                        else
                        {
                            Debug.WriteLine("producto NO reemplazado");
                        }

                        break;
                    }

                    porcentajeAnterior += productoReemplazo.Porcentaje;
                }

                CalcularImporteNuevo();

                if (ImporteNuevo <= ImporteObjetivo) break;
            }
            Debug.WriteLine("FIN-CAMBIO-PRODUCTOS");
        }

        public TipoRespuesta CalcularImporteNuevo()
        {
            try
            {
                decimal importeNuevo = 0;
                var cheques = Cheques
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
                        var chequesDetalle = ChequesDetalle.Where(x => x.foliodet == cheque.folio).ToList();

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

            Turnos = new List<Turno>();
            Cheques = new List<Cheque>();
            ChequesDetalle = new List<ChequeDetalle>();
            ChequesPago = new List<ChequePago>();

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
            PorcentajeObjetivo = 50;
            ImporteObjetivo = 0m;
            IncluirCuentaPagadaTarjeta = false;
            IncluirCuentaPagadaVales = false;
            IncluirCuentaPagadaOtros = false;
            IncluirCuentaFacturada = false;
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

            FechaInicio = new DateTime(2020, 11, 1);
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

                    return new RespuestaBusqueda
                    {
                        TipoRespuesta = TipoRespuesta.HECHO,
                        Turnos = turnos.OrderBy(x => x.idturno).ToList(),
                        Cheques = cheques.OrderBy(x => x.numcheque.Value).ToList(),
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
            try
            {
                foreach (var turno in respuesta.Turnos)
                {
                    TipoAccion tipoAccion = (turno.apertura >= FechaCorteInicio && turno.apertura <= FechaCorteCierre) ? TipoAccion.NINGUNO : TipoAccion.OMITIR;
                    Turnos.Add(Funciones.ParseTurno(turno, tipoAccion));
                }

                foreach (var cheque in respuesta.Cheques)
                {
                    var turno = Turnos.Find(x => x.idturno == cheque.idturno);
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

                    Cheques.Add(Funciones.ParseCheque(cheque, tipoAccion, tipoPago));
                }

                foreach (var chequepago in respuesta.Chequespagos)
                {
                    var cheque = Cheques.Find(x => x.folio == chequepago.folio);
                    ChequesPago.Add(Funciones.ParseChequePago(chequepago, cheque.TipoAccion));
                }

                foreach (var det in respuesta.Cheqdet)
                {
                    var cheque = Cheques.Find(x => x.folio == det.foliodet.Value);
                    TipoClasificacion tipoClasificacion = ObtenerClasificacionProductoSR(det.idproducto);
                    ChequesDetalle.Add(Funciones.ParseChequeDetalle(det, cheque.TipoAccion, tipoClasificacion));
                }

                UltimoMovimiento = ChequesDetalle.Where(x => x.TipoAccion != TipoAccion.OMITIR).Max(x => x.movimiento.Value);
                ImporteAnterior = Cheques.Where(x => x.TipoAccion != TipoAccion.OMITIR).Sum(x => x.total.Value);
                ImporteObjetivo = ImporteAnterior * (100m - PorcentajeObjetivo) / 100;
                NumeroTotalCuentas = Cheques.Count(x => x.TipoAccion != TipoAccion.OMITIR);
                EfectivoAnterior = Cheques.Where(x => x.TipoAccion != TipoAccion.OMITIR).Sum(x => x.efectivo.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
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
            try
            {
                var cheques = Cheques.Where(x => x.TipoAccion != TipoAccion.OMITIR).ToList();
                var fondoTurnos = Turnos.Where(x => x.TipoAccion == TipoAccion.NINGUNO || x.TipoAccion == TipoAccion.ACTUALIZAR).Sum(x => x.fondo.Value);

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
                        FolioCuenta = (int)cheque.numcheque.Value,
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
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        private List<Turno> Turnos { get; set; }
        private List<Cheque> Cheques { get; set; }
        private List<ChequeDetalle> ChequesDetalle { get; set; }
        private List<ChequePago> ChequesPago { get; set; }
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
