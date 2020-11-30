using mod_add.Enums;
using mod_add.Modelos;
using mod_add.Selectores;
using mod_add.Utils;
using SR.Datos;
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
    public class ReimpresionFoliosViewModel : ViewModelBase
    {
        public ReimpresionFoliosViewModel()
        {
            ImpresionCuentas = new List<ImpresionCuenta>
            {
                new ImpresionCuenta
                {
                    DisplayText = "Cuentas",
                    TipoImpresionCuenta = TipoImpresionCuenta.CUENTA
                },
                new ImpresionCuenta
                {
                    DisplayText = "Notas de consumo",
                    TipoImpresionCuenta = TipoImpresionCuenta.NOTACONSUMO
                },
                new ImpresionCuenta
                {
                    DisplayText = "Cuentas y notas de consumo",
                    TipoImpresionCuenta = TipoImpresionCuenta.CUENTA_NOTACONSUMO
                }
            };

            BusquedaCuentas = new List<BusquedaCuenta>
            {
                new BusquedaCuenta
                {
                    DisplayText = "Normal",
                    TipoBusquedaCuenta = TipoBusquedaCuenta.NORMAL
                },
                new BusquedaCuenta
                {
                    DisplayText = "Fiscal",
                    TipoBusquedaCuenta = TipoBusquedaCuenta.FISCAL
                },
            };

            BusquedaRegistros = new List<BusquedaRegistro>
            {
                new BusquedaRegistro
                {
                    DisplayText = "Por fechas",
                    TipoBusquedaRegistro = TipoBusquedaRegistro.FECHA,
                },
                new BusquedaRegistro
                {
                    DisplayText = "Por folios",
                    TipoBusquedaRegistro = TipoBusquedaRegistro.FOLIO,
                }
            };

            ImpresionCuenta = ImpresionCuentas.Find(x => x.TipoImpresionCuenta == TipoImpresionCuenta.CUENTA_NOTACONSUMO);
            BusquedaCuenta = BusquedaCuentas.Find(x => x.TipoBusquedaCuenta == TipoBusquedaCuenta.FISCAL);
            BusquedaRegistro = BusquedaRegistros.Find(x => x.TipoBusquedaRegistro == TipoBusquedaRegistro.FECHA);


            if (App.SRConfiguracion.CorteInicio > App.SRConfiguracion.CorteCierre)
            {
                FechaInicio = DateTime.Today.AddDays(-1);
                FechaCierre = FechaInicio.AddDays(1);
            }
            else
            {
                FechaInicio = DateTime.Today;
                FechaCierre = FechaInicio;
            }

            HoraInicio = FechaInicio.AddSeconds(App.SRConfiguracion.CorteInicio.TotalSeconds);
            HoraCierre = FechaInicio.AddSeconds(App.SRConfiguracion.CorteCierre.TotalSeconds);

            FolioMin = 0;
            FolioMax = 0;

            ImprimirSoloModificados = false;
            ImprimirEnArchivo = false;

            Cheques = new List<SR_cheques>();
        }

        private string CamposCheques()
        {
            List<string> campos = new List<string>
            {
                "folio",
                "CAST(c.numcheque AS Decimal) AS numcheque",
                "c.cierre",
                "c.mesa",
                "m.nombre AS idmesero",
                "CAST(c.nopersonas AS Decimal) AS nopersonas",
                "CAST(c.orden AS Decimal) AS orden",
                "c.subtotal",
                "c.totalimpuesto1",
                "c.total",
                "c.efectivo",
                "c.tarjeta",
                "c.cambio",
                "c.modificado",
            };

            return string.Join(",", campos);
        }

        private string CamposCheqdet()
        {
            List<string> campos = new List<string>
            {
                "cd.foliodet",
                "cd.movimiento",
                "cd.cantidad",
                "p.descripcion AS idproducto",
                "cd.precio",
            };

            return string.Join(",", campos);
        }

        public Respuesta ObtenerCuentas()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    DateTime FechaCorteInicio = FechaInicio.AddSeconds(HoraInicio.TimeOfDay.TotalSeconds);
                    DateTime FechaCorteCierre = FechaCierre.AddSeconds(HoraCierre.TimeOfDay.TotalSeconds);

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

                    SqlParameter parametroInicio = null;
                    SqlParameter parametroFin = null;
                    string query;
                    string campo = "";
                    string tablacheques = "";

                    if (BusquedaRegistro.TipoBusquedaRegistro == TipoBusquedaRegistro.FECHA)
                    {
                        campo = "fecha";
                        parametroInicio = new SqlParameter("inicio", FechaCorteInicio);
                        parametroFin = new SqlParameter("fin", FechaCorteCierre);
                    }
                    else if (BusquedaRegistro.TipoBusquedaRegistro == TipoBusquedaRegistro.FOLIO)
                    {
                        campo = "numcheque";
                        parametroInicio = new SqlParameter("inicio", FolioMin);
                        parametroFin = new SqlParameter("fin", FolioMax);
                    }

                    if (BusquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.NORMAL)
                    {
                        tablacheques = "cheques";
                    }
                    else if (BusquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.FISCAL)
                    {
                        tablacheques = "chequesf";
                    }

                    query = $"SELECT {CamposCheques()} FROM {tablacheques} c INNER JOIN meseros m ON c.idmesero = m.idmesero WHERE ({campo} BETWEEN @inicio AND @fin) AND c.idempresa = @{nameof(App.ClaveEmpresa)} ORDER BY c.numcheque";

                    List<SR_cheques> cheques = context.Database.SqlQuery<SR_cheques>(
                        query, 
                        parametroInicio, 
                        parametroFin,
                        new SqlParameter($"{nameof(App.ClaveEmpresa)}", App.ClaveEmpresa)).ToList();

                    if (cheques.Count == 0)
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                        };
                    }

                    return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.HECHO,
                        Cheques = cheques
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

        public void CargarResultados(Respuesta respuesta)
        {
            Cheques = respuesta.Cheques;
        }

        public Respuesta Generar()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    var chequesImpresion = Cheques.Where(x => ImprimirSoloModificados ? (x.modificado == 1) : true);

                    SR_parametros_DAO parametros_DAO = new SR_parametros_DAO(context);
                    var parametros = parametros_DAO.GetAll().FirstOrDefault();

                    string query;
                    string tablacheqdet = "";

                    if (BusquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.NORMAL)
                    {
                        tablacheqdet = "cheqdet";
                    }
                    else if (BusquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.FISCAL)
                    {
                        tablacheqdet = "cheqdetf";
                    }

                    List<string> nombresParametros = new List<string>();
                    object[] valores;
                    object[] parametrosSql;

                    valores = chequesImpresion.Select(x => (object)x.folio).ToArray();
                    parametrosSql = new object[valores.Length];

                    for (int i = 0; i < valores.Length; i++)
                    {
                        string nombreParametro = $"p{i + 1}";
                        nombresParametros.Add($"@{nombreParametro}");
                        parametrosSql[i] = new SqlParameter(nombreParametro, valores[i]);
                    }

                    query = $"SELECT {CamposCheqdet()} FROM {tablacheqdet} cd INNER JOIN productos p ON cd.idproducto = p.idproducto WHERE foliodet IN ({string.Join(",", nombresParametros)})";

                    List<SR_cheqdet> cheqdet = context.Database.SqlQuery<SR_cheqdet>(query, parametrosSql).ToList();

                    GenerarReporte generar = new GenerarReporte
                    {
                        PathReimpresionFolios = DirectorioExportacion,
                        ImprimirEnArchivo = ImprimirEnArchivo
                    };

                    foreach (var cheque in chequesImpresion)
                    {
                        generar.Folios(new ReporteFolios
                        {
                            Cheque = cheque,
                            CheqDet = cheqdet.Where(x => (x.foliodet ?? 0) == cheque.folio).OrderBy(x => x.movimiento).ToList()
                        });
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

        public void LimpiarCampos()
        {
            Cheques = new List<SR_cheques>();
            ImprimirSoloModificados = false;
            ImprimirEnArchivo = false;
        }

        public string DirectorioExportacion { get; set; }

        private List<ImpresionCuenta> impresionCuentas;
        public List<ImpresionCuenta> ImpresionCuentas
        {
            get { return impresionCuentas; }
            set
            {
                impresionCuentas = value;
                OnPropertyChanged(nameof(ImpresionCuentas));
            }
        }

        private ImpresionCuenta impresionCuenta;
        public ImpresionCuenta ImpresionCuenta
        {
            get { return impresionCuenta; }
            set
            {
                impresionCuenta = value;
                OnPropertyChanged(nameof(ImpresionCuenta));
            }
        }

        private List<BusquedaCuenta> busquedaCuentas;
        public List<BusquedaCuenta> BusquedaCuentas
        {
            get { return busquedaCuentas; }
            set
            {
                busquedaCuentas = value;
                OnPropertyChanged(nameof(BusquedaCuentas));
            }
        }

        private BusquedaCuenta busquedaCuenta;
        public BusquedaCuenta BusquedaCuenta
        {
            get { return busquedaCuenta; }
            set
            {
                busquedaCuenta = value;
                OnPropertyChanged(nameof(ImpresionCuenta));
            }
        }

        private List<BusquedaRegistro> busquedaRegistros;
        public List<BusquedaRegistro> BusquedaRegistros
        {
            get { return busquedaRegistros; }
            set
            {
                busquedaRegistros = value;
                OnPropertyChanged(nameof(BusquedaRegistros));
            }
        }

        private BusquedaRegistro busquedaRegistro;
        public BusquedaRegistro BusquedaRegistro
        {
            get { return busquedaRegistro; }
            set
            {
                busquedaRegistro = value;
                OnPropertyChanged(nameof(BusquedaRegistro));
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

        private DateTime horaInicio;
        public DateTime HoraInicio
        {
            get { return horaInicio; }
            set
            {
                horaInicio = value;
                OnPropertyChanged(nameof(HoraInicio));
            }
        }

        private DateTime horaCierre;
        public DateTime HoraCierre
        {
            get { return horaCierre; }
            set
            {
                horaCierre = value;
                OnPropertyChanged(nameof(HoraCierre));
            }
        }

        private long folioMin;
        public long FolioMin
        {
            get { return folioMin; }
            set
            {
                folioMin = value;
                OnPropertyChanged(nameof(FolioMin));
            }
        }

        private long folioMax;

        public long FolioMax
        {
            get { return folioMax; }
            set
            {
                folioMax = value;
                OnPropertyChanged(nameof(FolioMax));
            }
        }

        private bool imprimirSoloModificados;
        public bool ImprimirSoloModificados
        {
            get { return imprimirSoloModificados; }
            set
            {
                imprimirSoloModificados = value;
                OnPropertyChanged(nameof(ImprimirSoloModificados));
            }
        }

        private bool imprimirEnArchivo;
        public bool ImprimirEnArchivo
        {
            get { return imprimirEnArchivo; }
            set
            {
                imprimirEnArchivo = value;
                OnPropertyChanged(nameof(ImprimirEnArchivo));
            }
        }

        private List<SR_cheques> cheques;
        public List<SR_cheques> Cheques
        {
            get { return cheques; }
            set
            {
                cheques = value;
                OnPropertyChanged(nameof(Cheques));
            }
        }

    }
}
