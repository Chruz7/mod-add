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
        private readonly GenerarReporte generarReporte;
        public CorteZViewModel(TipoCorte tipoCorte)
        {
            generarReporte = new GenerarReporte();

            if (App.SRConfiguracion.CorteInicio > App.SRConfiguracion.CorteCierre)
            {
                FechaInicio = DateTime.Today.AddDays(-1);
            }
            else
            {
                FechaInicio = DateTime.Today;
            }

            AjustarFechaCierre();
            CorteInicio = FechaInicio.AddSeconds(App.SRConfiguracion.CorteInicio.TotalSeconds);
            CorteCierre = FechaCierre.AddSeconds(App.SRConfiguracion.CorteCierre.TotalSeconds);

            if (tipoCorte == TipoCorte.PERIODO)
            {
                HorarioTurno = "";
            }
            else if (tipoCorte == TipoCorte.TURNO)
            {
                HorarioTurno = $"{App.SRConfiguracion.cortezinicio} - {App.SRConfiguracion.cortezfin}";
            }

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

            ConvertirMonedaExtranjera = true;
            NoConsiderarDepositosRetiros = false;
            ConsiderarFondoinicial = false;
            NoConsiderarPropinas = false;
        }

        public void AjustarFechaCierre()
        {
            if (App.SRConfiguracion.CorteInicio > App.SRConfiguracion.CorteCierre)
            {
                FechaCierre = FechaInicio.AddDays(1);
            }
            else
            {
                FechaCierre = FechaInicio;
            }
        }

        private string CamposCheques()
        {
            List<string> campos = new List<string>
            {
                "c.folio",
                "CAST(c.numcheque AS Decimal) as numcheque",
                "c.cierre",
                "c.mesa",
                "CAST(c.nopersonas AS Decimal) as nopersonas",
                "c.cancelado",
                "CAST(c.impresiones AS Decimal) as impresiones",
                "c.descuento",
                "CAST(c.reabiertas AS Decimal) as reabiertas",
                "CAST(c.tipodeservicio AS Decimal) as tipodeservicio",
                "CAST(c.idturno AS Decimal) as idturno",
                "c.idtipodescuento",
                "CAST(c.folionotadeconsumo AS Decimal) as folionotadeconsumo",
                "c.total",
                "c.cargo",
                "c.descuentoimporte",
                "c.efectivo",
                "c.tarjeta",
                "c.vales",
                "c.otros",
                "c.propina",
                "c.tipoventarapida",
                "c.totalsindescuento",
                "c.totaldescuentos",
                "c.totaldescuentoalimentos",
                "c.totaldescuentobebidas",
                "c.totaldescuentootros",
                "c.totalcortesias",
                "c.totalcortesiaalimentos",
                "c.totalcortesiabebidas",
                "c.totalcortesiaotros",
                "c.totaldescuentoycortesia",
                "c.totalalimentossindescuentos",
                "c.totalbebidassindescuentos",
                "c.totalotrossindescuentos",
                "c.descuentomonedero",
                "c.subtotalcondescuento",
                "td.desc_tipodescuento AS DescripcionTipoDescuento",
            };

            return string.Join(",", campos);
        }

        private string CamposTurnos()
        {
            List<string> campos = new List<string>
            {
                "idturno",
                "fondo",
                "apertura",
                "cierre",
                "idestacion",
                "cajero",
                "efectivo",
                "tarjeta",
                "vales",
                "credito",
            };

            return string.Join(",", campos);
        }

        private string CamposChequesPagos()
        {
            List<string> campos = new List<string>
            {
                "CAST(cp.folio AS bigint) AS folio",
                "cp.idformadepago",
                "cp.importe",
                "cp.propina",
                "cp.tipodecambio",
                "cp.referencia",
                "fp.descripcion AS DescripcionFormaPago",
            };

            return string.Join(",", campos);
        }

        public Respuesta Generar(TipoDestino tipoDestino)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    DateTime FechaCorteInicio = FechaInicio.AddSeconds(App.SRConfiguracion.CorteInicio.TotalSeconds);
                    DateTime FechaCorteCierre = FechaInicio.AddSeconds(App.SRConfiguracion.CorteCierre.TotalSeconds);

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



                    //SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, false);
                    //query = $"apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND cierre IS NOT NULL AND idempresa=@{nameof(App.ClaveEmpresa)}";

                    query = $"SELECT {CamposTurnos()} FROM turnos WHERE apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND cierre IS NOT NULL AND idempresa=@{nameof(App.ClaveEmpresa)}";

                    var turnos = context.Database
                        .SqlQuery<TurnoReporte>(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .ToList();

                    if (turnos.Count == 0)
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                        };
                    }

                    List<TurnoReporte> turnosf = new List<TurnoReporte>();

                    if (!App.ConfiguracionSistema.ModificarVentasReales)
                    {
                        //SR_turnos_DAO turnosf_DAO = new SR_turnos_DAO(context, true);
                        //query = $"apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND cierre IS NOT NULL AND idempresa=@{nameof(App.ClaveEmpresa)}";

                        query = $"SELECT {CamposTurnos()} FROM turnosf WHERE apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)} AND cierre IS NOT NULL AND idempresa=@{nameof(App.ClaveEmpresa)}";
                        turnosf = context.Database
                            .SqlQuery<TurnoReporte>(query,
                                new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                                new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                                new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .ToList();
                    }

                    if (!App.ConfiguracionSistema.ModificarVentasReales && turnosf.Count == 0)
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                        };
                    }

                    string turnosT;
                    string chequesT;
                    string cheqdetT;
                    string chequespagosT;

                    if (App.ConfiguracionSistema.ModificarVentasReales)
                    {
                        turnosT = "turnos";
                        chequesT = "cheques";
                        cheqdetT = "cheqdet";
                        chequespagosT = "chequespagos";
                    }
                    else
                    {
                        turnosT = "turnosf";
                        chequesT = "chequesf";
                        cheqdetT = "cheqdetf";
                        chequespagosT = "chequespagosf";
                    }

                    List<string> nombresParametros = new List<string>();
                    object[] valores;
                    object[] parametrosSql;

                    TurnoReporte turno;

                    if (App.ConfiguracionSistema.ModificarVentasReales)
                    {
                        turno = turnos[0];
                    }
                    else
                    {
                        turno = turnosf[0];
                    }

                    var idsturnor = turnos.Select(x => (object)x.idturno).ToArray();
                    var idsturnof = turnosf.Select(x => (object)x.idturno).ToArray();

                    var idsturnos = App.ConfiguracionSistema.ModificarVentasReales ? idsturnor : idsturnof;

                    SR_parametros_DAO parametros_DAO = new SR_parametros_DAO(context);
                    var parametros = parametros_DAO.GetAll().FirstOrDefault();

                    //SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    //var cheques = cheques_DAO.WhereIn("idturno", idsturno);
                    

                    nombresParametros.Clear();
                    valores = idsturnos;
                    parametrosSql = new object[valores.Length];

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT {CamposCheques()} FROM {chequesT} c LEFT JOIN tipodescuento td ON c.idtipodescuento = td.idtipodescuento WHERE idturno IN ({string.Join(",", nombresParametros)})";

                    List<ChequeReporte> cheques = context.Database.SqlQuery<ChequeReporte>(query, parametrosSql).ToList();

                    int cuentasCanceladas = cheques.Count(x => x.cancelado ?? false);

                    cheques = cheques.Where(x => !(x.cancelado ?? false)).OrderBy(x => x.numcheque ?? 0).ToList();

                    var folios = cheques.OrderBy(x => x.folio).Select(x => (object)x.folio).ToArray();

                    nombresParametros.Clear();
                    valores = folios;
                    parametrosSql = new object[valores.Length];

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT {CamposChequesPagos()} FROM {chequespagosT} cp LEFT JOIN formasdepago fp ON cp.idformadepago = fp.idformadepago WHERE folio IN ({string.Join(",", nombresParametros)})";

                    List<ChequePagoReporte> chequesPagos = context.Database.SqlQuery<ChequePagoReporte>(query, parametrosSql).ToList();

                    //SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    //var chequespagos = chequespagos_DAO.WhereIn("folio", folios);



                    //if (ConvertirMonedaExtrangera && (App.SRConfiguracion.vercambiomonex ?? false))
                    //{
                    //    foreach (var cheque in cheques)
                    //    {
                    //        if ((cheque.cancelado ?? false)) continue;

                    //        query = $"SELECT fp.tipodecambio FROM chequespagos cp inner JOIN formasdepago fp ON cp.idformadepago=fp.idformadepago WHERE cp.importe > 0 AND cp.tipodecambio <> 1 AND fp.tipo=1 and fp.idformadepago=@{nameof(App.SRConfiguracion.vercambioclavemonex)} AND cp.folio=@{nameof(cheque.folio)}";
                    //        decimal? tipoCambio = context.Database.SqlQuery<decimal?>(query,
                    //            new SqlParameter($"{nameof(App.SRConfiguracion.vercambioclavemonex)}", App.SRConfiguracion.vercambioclavemonex),
                    //            new SqlParameter($"{nameof(cheque.folio)}", cheque.folio)).Single();

                    //        if (!tipoCambio.HasValue) continue;


                    //    }
                    //}

                    nombresParametros.Clear();
                    valores = folios;
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
                        query = $"SELECT cp.idformadepago, fp.descripcion, fp.tipodecambio, SUM(cp.importe) AS importe, SUM(cp.propina) AS propina, fp.prioridadboton " +
                            $"FROM {chequespagosT} AS cp " +
                            $"INNER JOIN formasdepago AS fp " +
                            $"ON fp.idformadepago = cp.idformadepago " +
                            $"WHERE cp.folio IN ({string.Join(",", nombresParametros)}) " +
                            $"GROUP BY cp.idformadepago, fp.descripcion, fp.tipodecambio, fp.prioridadboton " +
                            $"ORDER BY fp.prioridadboton";

                        pagos = context.Database.SqlQuery<Pago>(query, parametrosSql).ToList();
                    }

                    decimal depositosEfectivo = 0;
                    decimal retirosEfectivo = 0;

                    if (!NoConsiderarDepositosRetiros)
                    {
                        query = $"SELECT ISNULL(sum(importe), 0) as importe FROM movtoscaja WHERE idempresa=@{nameof(App.ClaveEmpresa)} and idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND (CAST(cancelado as int)=0 or cancelado is null) and (CAST(pagodepropina as int)=0 or pagodepropina is null) and tipo=1";

                        depositosEfectivo = context.Database.SqlQuery<decimal>(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                            .Single();

                        query = $"SELECT ISNULL(sum(importe), 0) as importe FROM movtoscaja WHERE idempresa=@{nameof(App.ClaveEmpresa)} and idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND (CAST(cancelado as int)=0 or cancelado is null) and tipo=2";

                        retirosEfectivo = context.Database.SqlQuery<decimal>(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                            .Single();
                    }

                    decimal propinasPagadas = 0;

                    if (!NoConsiderarPropinas)
                    {
                        query = $"SELECT ISNULL(sum(importe), 0) as importe FROM movtoscaja WHERE idempresa=@{nameof(App.ClaveEmpresa)} and idturno in (SELECT idturno FROM turnos WHERE (apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) AND cierre is not null and idempresa=@{nameof(App.ClaveEmpresa)})  AND (CAST(cancelado as int)=0 or cancelado is null) and CAST(pagodepropina as int)=1 and tipo=1";

                        propinasPagadas = context.Database.SqlQuery<decimal>(query,
                            new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                            new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                            new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                            .Single();
                    }

                    query = $"SELECT CAST(ISNULL(SUM(cd.cantidad), 0) as float) as cantidad " +
                        $"FROM productos p " +
                        $"INNER JOIN {cheqdetT} cd " +
                        $"ON p.idproducto=cd.idproducto " +
                        $"INNER JOIN {chequesT} c " +
                        $"ON c.folio = cd.foliodet " +
                        $"INNER JOIN grupos g " +
                        $"ON g.idgrupo=p.idgrupo " +
                        $"WHERE c.idturno in (" +
                            $"SELECT t.idturno " +
                            $"FROM {turnosT} t " +
                            $"WHERE (t.apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) " +
                            $"AND t.cierre is not null " +
                            $"and t.idempresa=@{nameof(App.ClaveEmpresa)}) " +
                        $"AND CAST(c.cancelado as int)=0 " +
                        $"AND g.clasificacion=2";

                    double cantidadAlimentos = context.Database.SqlQuery<double>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    query = $"SELECT CAST(ISNULL(SUM(cd.cantidad), 0) as float) as cantidad " +
                        $"FROM productos p " +
                        $"INNER JOIN {cheqdetT} cd " +
                        $"ON p.idproducto=cd.idproducto " +
                        $"INNER JOIN {chequesT} c " +
                        $"ON c.folio = cd.foliodet " +
                        $"INNER JOIN grupos g " +
                        $"ON g.idgrupo=p.idgrupo " +
                        $"WHERE c.idturno in (" +
                            $"SELECT t.idturno " +
                            $"FROM {turnosT} t " +
                            $"WHERE (t.apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) " +
                            $"AND t.cierre is not null " +
                            $"and t.idempresa=@{nameof(App.ClaveEmpresa)}) " +
                        $"AND CAST(c.cancelado as int)=0 " +
                        $"AND g.clasificacion=1";

                    double cantidadBebidas = context.Database.SqlQuery<double>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    query = $"SELECT CAST(ISNULL(SUM(cd.cantidad), 0) as float) as cantidad " +
                        $"FROM productos p " +
                        $"INNER JOIN {cheqdetT} cd " +
                        $"ON p.idproducto=cd.idproducto " +
                        $"INNER JOIN {chequesT} c " +
                        $"ON c.folio = cd.foliodet " +
                        $"INNER JOIN grupos g " +
                        $"ON g.idgrupo=p.idgrupo " +
                        $"WHERE c.idturno in (" +
                            $"SELECT t.idturno " +
                            $"FROM {turnosT} t " +
                            $"WHERE (t.apertura BETWEEN @{nameof(FechaCorteInicio)} AND @{nameof(FechaCorteCierre)}) " +
                            $"AND t.cierre is not null " +
                            $"and t.idempresa=@{nameof(App.ClaveEmpresa)}) " +
                        $"AND CAST(c.cancelado as int)=0 " +
                        $"AND g.clasificacion=3";

                    double cantidadOtros = context.Database.SqlQuery<double>(query,
                        new SqlParameter($"{nameof(FechaCorteInicio)}", FechaCorteInicio),
                        new SqlParameter($"{nameof(FechaCorteCierre)}", FechaCorteCierre),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    nombresParametros.Clear();
                    valores = App.ConfiguracionSistema.ModificarVentasReales ? idsturnor : idsturnof;
                    parametrosSql = new object[valores.Length + 1];
                    parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT CAST(c.numcheque AS int) AS numcheque,fp.descripcion, cp.importe, cp.propina " +
                        $"FROM {chequesT} c " +
                        $"INNER JOIN {chequespagosT} cp " +
                        $"ON c.folio = cp.folio " +
                        $"INNER JOIN formasdepago fp " +
                        $"ON fp.idformadepago = cp.idformadepago " +
                        $"WHERE c.idturno in ({string.Join(",", nombresParametros)}) " +
                        $"and fp.tipo = 2 " +
                        $"AND (c.cancelado = 0 OR c.cancelado is null) " +
                        $"ORDER BY c.numcheque";

                    List<PagoTarjeta> pagosTarjeta = context.Database.SqlQuery<PagoTarjeta>(query, parametrosSql).ToList();

                    int anio = FechaCorteInicio.Year;
                    int mes = FechaCorteInicio.Month;

                    query = $"select sum(total) as total " +
                        $"from {chequesT} " +
                        $"where month(fecha)=@{nameof(mes)} " +
                        $"and year(fecha)=@{nameof(anio)} " +
                        $"and idempresa=@{nameof(App.ClaveEmpresa)}";

                    decimal acumuladoMesAnterior = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(mes)}", mes - 1),
                        new SqlParameter($"{nameof(anio)}", anio),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();

                    decimal acumuladoMesActual = context.Database.SqlQuery<decimal>(query,
                        new SqlParameter($"{nameof(mes)}", mes),
                        new SqlParameter($"{nameof(anio)}", anio),
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa))
                        .Single();


                    nombresParametros.Clear();
                    valores = idsturnor;
                    parametrosSql = new object[valores.Length + 1];
                    parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT ISNULL(SUM(TOTAL), 0) AS TOTAL FROM facturas WHERE idturno in ({string.Join(",", nombresParametros)}) AND CAST(cancelada as int)=0 and idempresa=@{nameof(App.ClaveEmpresa)}";

                    decimal ventaFacturada = context.Database.SqlQuery<decimal>(query, parametrosSql).Single();


                    nombresParametros.Clear();
                    valores = idsturnor;
                    parametrosSql = new object[valores.Length + 1];
                    parametrosSql[0] = new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa);

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i + 1] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT ISNULL(SUM(propina), 0) AS TOTAL FROM facturas WHERE idturno in ({string.Join(",", nombresParametros)}) AND CAST(cancelada as int)=0 and CAST(propinafacturada as int)=1 and idempresa=@{nameof(App.ClaveEmpresa)}";

                    decimal propinaFacturada = context.Database.SqlQuery<decimal>(query, parametrosSql).Single();


                    nombresParametros.Clear();
                    valores = cheques.OrderBy(x => x.folio).Select(x => (object)x.folio).ToArray();
                    parametrosSql = new object[valores.Length];

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT " +
                            $"CAST(cd.impuesto1 AS int) AS porcentaje, " +
                            $"sum(cd.preciosinimpuestos * cd.cantidad * (100 - cd.descuento) / 100 * (100 - c.descuento) / 100) AS venta, " +
                            $"sum(cd.preciosinimpuestos * cd.impuesto1 / 100 * cd.cantidad * (100 - cd.descuento) / 100 * (100 - c.descuento) / 100) AS impuesto " +
                        $"FROM {cheqdetT} cd " +
                        $"inner join {chequesT} c " +
                        $"on cd.foliodet = c.folio " +
                        $"WHERE c.folio IN ({string.Join(",", nombresParametros)}) " +
                        $"and c.descuento != 100 " +
                        $"GROUP by cd.impuesto1 " +
                        $"ORDER BY cd.impuesto1";

                    List<ImpuestoVenta> impuestosVentas = context.Database.SqlQuery<ImpuestoVenta>(query, parametrosSql).ToList();

                    decimal totalDeclarado = (turno.efectivo ?? 0) + (turno.tarjeta ?? 0) + (turno.vales ?? 0) + (turno.credito ?? 0);

                    if (!ConsiderarFondoinicial)
                    {
                        totalDeclarado -= (turno.fondo ?? 0);
                    }

                    ReporteCorte reporte = new ReporteCorte
                    {
                        Fecha = DateTime.Now,
                        TituloCorte = parametros.titulocortez,
                        FolioCorte = turno.idturno.Value,
                        FechaCorteInicio = FechaCorteInicio,
                        FechaCorteCierre = FechaCorteCierre,

                        EfectivoInicial = ConsiderarFondoinicial ? turno.fondo.Value : 0,
                        Efectivo = cheques.Sum(x => x.efectivo ?? 0),
                        Tarjeta = cheques.Sum(x => x.tarjeta ?? 0),
                        Vales = cheques.Sum(x => x.vales ?? 0),
                        Otros = cheques.Sum(x => x.otros ?? 0),
                        DepositosEfectivo = depositosEfectivo,
                        RetirosEfectivo = retirosEfectivo,
                        PropinasPagadas = propinasPagadas,

                        PAlimentos = cheques.Sum(x => x.totalalimentossindescuentos ?? 0),
                        PCantidadAlimentos = cantidadAlimentos,
                        PBebidas = cheques.Sum(x => x.totalbebidassindescuentos ?? 0),
                        PCantidadBebidas = cantidadBebidas,
                        POtros = cheques.Sum(x => x.totalotrossindescuentos ?? 0),
                        PCantidadOtros = cantidadOtros,

                        Comedor = cheques.Where(x => (x.tipodeservicio ?? 0) == 1).Sum(x => x.totalsindescuento ?? 0),
                        Domicilio = cheques.Where(x => (x.tipodeservicio ?? 0) == 2).Sum(x => x.totalsindescuento ?? 0),
                        Rapido = cheques.Where(x => (x.tipodeservicio ?? 0) == 3).Sum(x => x.totalsindescuento ?? 0),

                        Subtotal = cheques.Sum(x => x.totalsindescuento ?? 0),
                        Descuentos = cheques.Sum(x => (x.totaldescuentoycortesia ?? 0)),
                        VentaNeta = cheques.Sum(x => (x.subtotalcondescuento ?? 0)),

                        VentasConImpuesto = cheques.Sum(x => (x.total ?? 0)),

                        VentaFacturada = ventaFacturada,
                        PropinaFacturada = propinaFacturada,

                        CuentasNormales = cheques.Count(),
                        CuentasCanceladas = cuentasCanceladas,
                        CuentasConDescuento = cheques.Count(x => (x.descuento ?? 0) > 0 && (x.descuento ?? 0) < 100),
                        CuentasConDescuentoImporte = cheques.Where(x => (x.descuento ?? 0) > 0 && (x.descuento ?? 0) < 100).Sum(x => x.descuentoimporte ?? 0),
                        CuentasConCortesia = cheques.Count(x => (x.descuento ?? 0) == 100),
                        CuentasConCortesiaImporte = cheques.Sum(x => x.totalcortesias ?? 0),

                        CuentaPromedio = cheques.Sum(x => (x.subtotalcondescuento ?? 0)) / cheques.Count(),
                        ConsumoPromedio = cheques.Sum(x => (x.subtotalcondescuento ?? 0)) / cheques.Sum(x => (x.nopersonas ?? 0)),

                        Comensales = (int)cheques.Sum(x => x.nopersonas ?? 0),

                        Propinas = cheques.Sum(x => x.propina ?? 0),
                        Cargos = cheques.Sum(x => (x.cargo ?? 0)),
                        DescuentoMonedero = cheques.Sum(x => (x.descuentomonedero ?? 0)),

                        FolioInicial = (int)cheques.Min(x => (x.numcheque ?? 0)),
                        FolioFinal = (int)cheques.Max(x => x.numcheque ?? 0),

                        CortesiaAlimentos = cheques.Sum(x => (x.totalcortesiaalimentos ?? 0)),
                        CortesiaBebidas = cheques.Sum(x => (x.totalcortesiabebidas ?? 0)),
                        CortesiaOtros = cheques.Sum(x => (x.totalcortesiaotros ?? 0)),
                        TotalCortesias = cheques.Sum(x => x.totalcortesias ?? 0),

                        DescuentoAlimentos = cheques.Sum(x => (x.totaldescuentoalimentos ?? 0)),
                        DescuentoBebidas = cheques.Sum(x => (x.totaldescuentobebidas ?? 0)),
                        DescuentoOtros = cheques.Sum(x => (x.totaldescuentootros ?? 0)),
                        TotalDescuentos = cheques.Sum(x => x.totaldescuentos ?? 0),

                        TotalDeclarado = totalDeclarado,
                        AcumuladoMesAnterior = acumuladoMesAnterior,
                        AcumuladoMesActual = acumuladoMesActual,

                        Pagos = pagos,
                        ImpuestosVentas = impuestosVentas,
                        PagosTarjeta = pagosTarjeta,

                        NoConsiderarDepositosRetiros = NoConsiderarDepositosRetiros,
                        NoConsiderarPropinas = NoConsiderarPropinas,
                        ConsiderarFondoInicial = ConsiderarFondoinicial,
                        ReporteFiscal = !App.ConfiguracionSistema.ModificarVentasReales,
                        FiltroTurno = true,
                    };

                    turno.efectivo = reporte.Efectivo;
                    turno.Propina = propinasPagadas;
                    turno.Cargo = reporte.Cargos;
                    turno.Total = (turno.efectivo ?? 0) + (turno.tarjeta ?? 0) + (turno.vales ?? 0) + (turno.credito ?? 0) - turno.Propina;

                    reporte.PPorcentajeAlimentos = Math.Round(reporte.PAlimentos / reporte.Subtotal * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.PPorcentajeBebidas = Math.Round(reporte.PBebidas / reporte.Subtotal * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.PPorcentajeOtros = Math.Round(reporte.POtros / reporte.Subtotal * 100m, 0, MidpointRounding.AwayFromZero);

                    reporte.ComedorPorcentaje = Math.Round(reporte.Comedor / reporte.Subtotal * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.DomicilioPorcentaje = Math.Round(reporte.Domicilio / reporte.Subtotal * 100m, 0, MidpointRounding.AwayFromZero);
                    reporte.RapidoPorcentaje = Math.Round(reporte.Rapido / reporte.Subtotal * 100m, 0, MidpointRounding.AwayFromZero);

                    if (Reporte.TipoReporte == TipoReporte.RESUMIDO)
                    {
                        generarReporte.Resumido(reporte, tipoDestino);
                    }
                    else
                    {
                        reporte.ChequesReporte = cheques;

                        if (Reporte.TipoReporte == TipoReporte.DETALLADO_VERTICAL || Reporte.TipoReporte == TipoReporte.DETALLADO_HORIZONTAL)
                        {
                            reporte.TotalesChequesReporte = new List<ChequeReporte>
                            {
                                new ChequeReporte
                                {
                                    Totales = true,
                                    totaldescuentoycortesia = cheques.Sum(x => x.totaldescuentoycortesia ?? 0),
                                    propina = cheques.Sum(x => x.propina ?? 0),
                                    total = cheques.Sum(x => x.total ?? 0),
                                    cargo = cheques.Sum(x => x.cargo ?? 0),
                                    efectivo = cheques.Sum(x => x.efectivo ?? 0),
                                    tarjeta = cheques.Sum(x => x.tarjeta ?? 0),
                                    vales = cheques.Sum(x => x.vales ?? 0),
                                    otros = cheques.Sum(x => x.otros ?? 0)
                                }
                            };

                            reporte.Turnos = new List<TurnoReporte> { turno };
                        }

                        if (Reporte.TipoReporte == TipoReporte.DETALLADO_VERTICAL)
                        {
                            List<VentaRapida> ventasRapidas = cheques
                                .Where(x => (int)(x.tipodeservicio ?? 0) == 3 && (int)(x.tipoventarapida ?? 0) > 0)
                                .GroupBy(x => x.tipoventarapida ?? 0, x => x.total ?? 0, (tipoventarapida, total) => new VentaRapida
                                {
                                    Descripcion = tipoventarapida == 1 ? "" : (tipoventarapida == 2 ? "" : (tipoventarapida == 3 ? "" : "")),
                                    Total = total.Sum(x => x)
                                }).ToList();

                            reporte.VentasRapidas = ventasRapidas;

                            generarReporte.DetalladoVerticalPDF(reporte, tipoDestino);
                        }
                        else
                        {
                            if (Reporte.TipoReporte == TipoReporte.DETALLADO_HORIZONTAL)
                            {
                                generarReporte.DetalladoHorizontalPDF(reporte, tipoDestino);
                            }
                            else if (Reporte.TipoReporte == TipoReporte.DETALLADO_FORMAS_PAGO)
                            {
                                foreach (var cheque in cheques)
                                {
                                    cheque.ChequesPagos = chequesPagos.Where(x => x.folio == cheque.folio).ToList();
                                }

                                generarReporte.DetalladoFormasPagoPDF(reporte, tipoDestino);
                            }
                        }

                    }

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

        private DateTime corteInicio;

        public DateTime CorteInicio
        {
            get { return corteInicio; }
            set
            {
                corteInicio = value;
                OnPropertyChanged(nameof(CorteInicio));
            }
        }

        private DateTime corteCierre;

        public DateTime CorteCierre
        {
            get { return corteCierre; }
            set
            {
                corteCierre = value;
                OnPropertyChanged(nameof(CorteCierre));
            }
        }

        private DateTime fechaInicio;
        public DateTime FechaInicio
        {
            get { return fechaInicio; }
            set
            {
                fechaInicio = value;
                OnPropertyChanged(nameof(FechaInicio));
            }
        }

        private DateTime fechaCierre;
        public DateTime FechaCierre
        {
            get { return fechaCierre; }
            set
            {
                fechaCierre = value;
                OnPropertyChanged(nameof(FechaCierre));
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


        private bool convertirMonedaExtranjera;
        public bool ConvertirMonedaExtranjera
        {
            get { return convertirMonedaExtranjera; }
            set
            {
                convertirMonedaExtranjera = value;
                OnPropertyChanged(nameof(ConvertirMonedaExtranjera));
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

        private bool noConsiderarPropinas;

        public bool NoConsiderarPropinas
        {
            get { return noConsiderarPropinas; }
            set
            {
                noConsiderarPropinas = value;
                OnPropertyChanged(nameof(NoConsiderarPropinas));
            }
        }
    }
}
