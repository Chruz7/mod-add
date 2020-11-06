using mod_add.Enums;
using mod_add.Modelos;
using mod_add.Selectores;
using mod_add.Utils;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace mod_add.ViewModels
{
    public class CorteZViewModel : ViewModelBase
    {
        public CorteZViewModel()
        {
            Fecha = DateTime.Today.AddDays(-1);
            HorarioTurno = $"{App.SRConfiguracion.cortezinicio} - {App.SRConfiguracion.cortezfin}";

            Reportes = new List<Reporte>
            {
                new Reporte
                {
                    DisplayText = "Resumido",
                    TipoReporte = TipoReporte.RESUMIDO
                },
                new Reporte
                {
                    DisplayText = "Detallado vertical",
                    TipoReporte = TipoReporte.DETALLADO_VERTICAL
                },
                new Reporte
                {
                    DisplayText = "Detallado horizontal",
                    TipoReporte = TipoReporte.DETALLADO_HORIZONTAL
                },
                new Reporte
                {
                    DisplayText = "Detallado formas de pago",
                    TipoReporte = TipoReporte.DETALLADO_FORMAS_PAGO
                },
            };

            Reporte = Reportes.Find(x => x.TipoReporte == TipoReporte.RESUMIDO);

            ConvertirMonedaExtrangera = false;
            NoConsiderarDepositosRetiros = false;
            ConsiderarFondoinicial = false;
            NoConsiderarPropinas = false;

            Fecha = new DateTime(2020, 11, 1);
        }

        public Respuesta Generar(TipoDestino tipoDestino)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    DateTime FechaCorteInicio = Fecha.AddSeconds(App.SRConfiguracion.CorteInicio.TotalSeconds);
                    DateTime FechaCorteCierre = Fecha.AddSeconds(App.SRConfiguracion.CorteCierre.TotalSeconds);

                    if (App.SRConfiguracion.CorteInicio > App.SRConfiguracion.CorteCierre)
                    {
                        FechaCorteCierre = FechaCorteCierre.AddDays(1);
                    }

                    if (!Funciones.ValidarMesBusqueda(App.MesesValidos, FechaCorteInicio))
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.FECHA_INACCESIBLE,
                            Mensaje = FechaCorteInicio.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es"))
                        };
                    }

                    if (!Funciones.ValidarMesBusqueda(App.MesesValidos, FechaCorteCierre))
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.FECHA_INACCESIBLE,
                            Mensaje = FechaCorteCierre.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es"))
                        };
                    }

                    string query;

                    SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, App.ConfiguracionSistema.ModificarVentasReales);
                    query = $"apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND cierre IS NOT NULL AND idempresa=@{nameof(App.ClaveEmpresa)}";

                    var turnos = turnos_DAO.Get(query,
                                new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                                new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                                new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa));

                    if (turnos.Count == 0)
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                        };
                    }

                    List<string> nombresParametros = new List<string>();
                    object[] valores;
                    object[] parametrosSql;

                    var turno = turnos[0];

                    var idsturno = turnos.Select(x => (object)x.idturno).ToArray();

                    SR_parametros_DAO parametros_DAO = new SR_parametros_DAO(context);
                    var parametros = parametros_DAO.GetAll().FirstOrDefault();

                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, false);
                    var cheques = cheques_DAO.WhereIn("idturno", idsturno);

                    nombresParametros.Clear();
                    valores = cheques.Where(x => !x.cancelado.Value).OrderBy(x => x.folio).Select(x => (object)x.folio).ToArray();
                    parametrosSql = new object[valores.Length];

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i] = new SqlParameter(nombreParametro, valores[i]);
                    }
                    List<Pago> pagos = new List<Pago>();

                    if (valores.Length > 0)
                    {
                        query = $"SELECT cp.idformadepago, fp.descripcion, fp.tipodecambio, SUM(cp.importe) AS importe, SUM(cp.propina) AS propina, fp.prioridadboton FROM chequespagos AS cp INNER JOIN formasdepago AS fp ON fp.idformadepago = cp.idformadepago WHERE cp.folio IN ({string.Join(",", nombresParametros)}) GROUP BY cp.idformadepago, fp.descripcion, fp.tipodecambio, fp.prioridadboton ORDER BY fp.prioridadboton";

                        pagos = context.Database.SqlQuery<Pago>(query, parametrosSql).ToList();
                    }

                    query = $"SELECT ISNULL(sum(importe), 0) as importe FROM movtoscaja WHERE idempresa=@{nameof(App.ClaveEmpresa)} and idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND (CAST(cancelado as int)=0 or cancelado is null) and (CAST(pagodepropina as int)=0 or pagodepropina is null) and tipo=1";

                    decimal depositosEfectivo = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single(); ;

                    query = $"SELECT ISNULL(sum(importe), 0) as importe FROM movtoscaja WHERE idempresa=@{nameof(App.ClaveEmpresa)} and idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND (CAST(cancelado as int)=0 or cancelado is null) and tipo=2";

                    decimal retirosEfectivo = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single(); ;

                    query = $"SELECT ISNULL(sum(importe), 0) as importe FROM movtoscaja WHERE idempresa=@{nameof(App.ClaveEmpresa)} and idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND (CAST(cancelado as int)=0 or cancelado is null) and CAST(pagodepropina as int)=1 and tipo=1";

                    decimal propinasPagadas = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    query = $"SELECT CAST(ISNULL(SUM(cheqdet.cantidad), 0) as decimal(19,2)) as cantidad FROM productos INNER JOIN cheqdet ON Productos.idproducto=cheqdet.idproducto INNER JOIN cheques ON cheques.folio = cheqdet.foliodet INNER JOIN grupos ON Grupos.idgrupo=Productos.idgrupo WHERE cheques.idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND CAST(cheques.cancelado as int)=0 AND grupos.clasificacion=2";

                    decimal cantidadAlimentos = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    query = $"SELECT CAST(ISNULL(SUM(cheqdet.cantidad), 0) as decimal(19,2)) as cantidad FROM productos INNER JOIN cheqdet ON Productos.idproducto=cheqdet.idproducto INNER JOIN cheques ON cheques.folio = cheqdet.foliodet INNER JOIN grupos ON Grupos.idgrupo=Productos.idgrupo WHERE cheques.idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND CAST(cheques.cancelado as int)=0 AND grupos.clasificacion=1";

                    decimal cantidadBebidas = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    query = $"SELECT CAST(ISNULL(SUM(cheqdet.cantidad), 0) as decimal(19,2)) as cantidad FROM productos INNER JOIN cheqdet ON Productos.idproducto=cheqdet.idproducto INNER JOIN cheques ON cheques.folio = cheqdet.foliodet INNER JOIN grupos ON Grupos.idgrupo=Productos.idgrupo WHERE cheques.idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND CAST(cheques.cancelado as int)=0 AND grupos.clasificacion=3";

                    decimal cantidadOtros = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    ReporteZ reporte = new ReporteZ
                    {
                        TituloCorteZ = parametros.titulocortez,
                        FolioCorte = turno.idturno.Value,
                        FechaCorteInicio = FechaCorteInicio,
                        FechaCorteCierre = FechaCorteCierre,

                        EfectivoInicial = turno.fondo.Value,
                        Efectivo = cheques.Where(x => !x.cancelado.Value).Sum(x => x.efectivo.Value),
                        Tarjeta = cheques.Where(x => !x.cancelado.Value).Sum(x => x.tarjeta.Value),
                        Vales = cheques.Where(x => !x.cancelado.Value).Sum(x => x.vales.Value),
                        Otros = cheques.Where(x => !x.cancelado.Value).Sum(x => x.otros.Value),
                        DepositosEfectivo = depositosEfectivo,
                        RetirosEfectivo = retirosEfectivo,
                        PropinasPagadas = propinasPagadas,

                        PAlimentos = cheques.Where(x => !x.cancelado.Value).Sum(x => x.totalalimentossindescuentos.Value),
                        PCantidadAlimentos = cantidadAlimentos,
                        PBebidas = cheques.Where(x => !x.cancelado.Value).Sum(x => x.totalbebidassindescuentos.Value),
                        PCantidadBebidas = cantidadBebidas,
                        POtros = cheques.Where(x => !x.cancelado.Value).Sum(x => x.totalotrossindescuentos.Value),
                        PCantidadOtros = cantidadOtros,

                        Comedor = cheques.Where(x => !x.cancelado.Value && x.tipodeservicio.Value == 1).Sum(x => x.totalsindescuento.Value),
                        Domicilio = cheques.Where(x => !x.cancelado.Value && x.tipodeservicio.Value == 2).Sum(x => x.totalsindescuento.Value),
                        Rapido = cheques.Where(x => !x.cancelado.Value && x.tipodeservicio.Value == 3).Sum(x => x.totalsindescuento.Value),


                        CuentasNormales = cheques.Count(),
                        CuentasCanceladas = cheques.Count(x => x.cancelado.Value),
                        CuentasConDescuento = cheques.Count(x => x.descuento.Value > 0),
                        FolioInicial = cheques.Min(x => x.folio),
                        FolioFinal = cheques.Max(x => x.folio),
                        Pagos = pagos,
                    };

                    decimal totalTipoProducto = reporte.PAlimentos + reporte.PBebidas + reporte.POtros;

                    reporte.PPorcentajeAlimentos = Math.Round(reporte.PAlimentos / totalTipoProducto * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.PPorcentajeBebidas = Math.Round(reporte.PBebidas / totalTipoProducto * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.PPorcentajeOtros = Math.Round(reporte.POtros / totalTipoProducto * 100m, 0, MidpointRounding.AwayFromZero);

                    decimal totalTipoServicio = reporte.Comedor + reporte.Domicilio + reporte.Rapido;

                    reporte.ComedorPorcentaje = Math.Round(reporte.Comedor / totalTipoServicio * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.DomicilioPorcentaje = Math.Round(reporte.Domicilio / totalTipoServicio * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.RapidoPorcentaje = Math.Round(reporte.Rapido / totalTipoServicio * 100m, 0, MidpointRounding.AwayFromZero);

                    GenerarReporte.CorteZ(reporte, tipoDestino);

                    return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.HECHO
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.ERROR
                    };
                }
            }
        }

        private ReporteZ RealizarCorteZ(DateTime FechaCorteInicio, DateTime FechaCorteCierre)
        {
            ReporteZ reporte = new ReporteZ();

            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, App.ConfiguracionSistema.ModificarVentasReales);

                var turno = turnos_DAO.Get($"apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND idempresa=@{nameof(App.ClaveEmpresa)}",
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa)).FirstOrDefault();
            }

            return reporte;
        }

        private DateTime fecha;
        public DateTime Fecha
        {
            get { return fecha; }
            set
            {
                fecha = value;
                OnPropertyChanged(nameof(Fecha));
            }
        }

        private string horarioTurno;
        public string HorarioTurno
        {
            get { return horarioTurno; }
            set
            {
                horarioTurno = value;
                OnPropertyChanged(nameof(HorarioTurno));
            }
        }

        private List<Reporte> reportes;
        public List<Reporte> Reportes
        {
            get { return reportes; }
            set
            {
                reportes = value;
                OnPropertyChanged(nameof(Reportes));
            }
        }


        private Reporte reporte;
        public Reporte Reporte
        {
            get { return reporte; }
            set
            { 
                reporte = value;
                OnPropertyChanged(nameof(Reporte));
            }
        }


        private bool convertirMonedaExtrangera;
        public bool ConvertirMonedaExtrangera
        {
            get { return convertirMonedaExtrangera; }
            set
            {
                convertirMonedaExtrangera = value;
                OnPropertyChanged(nameof(ConvertirMonedaExtrangera));
            }
        }

        private bool noConsiderarDepositosRetiros;
        public bool NoConsiderarDepositosRetiros
        {
            get { return noConsiderarDepositosRetiros; }
            set
            {
                noConsiderarDepositosRetiros = value;
                OnPropertyChanged(nameof(NoConsiderarDepositosRetiros));
            }
        }

        private bool considerarFondoinicial;
        public bool ConsiderarFondoinicial
        {
            get { return considerarFondoinicial; }
            set
            {
                considerarFondoinicial = value;
                OnPropertyChanged(nameof(ConsiderarFondoinicial));
            }
        }

        private bool coConsiderarPropinas;

        public bool NoConsiderarPropinas
        {
            get { return coConsiderarPropinas; }
            set
            {
                coConsiderarPropinas = value;
                OnPropertyChanged(nameof(NoConsiderarPropinas));
            }
        }
    }
}
