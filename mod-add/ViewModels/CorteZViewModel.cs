using mod_add.Enums;
using mod_add.Selectores;
using mod_add.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

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
        }

        public Respuesta GenerarReporte(TipoDestino tipoDestino)
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

                //codigo

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

    }
}
