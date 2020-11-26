using mod_add.Enums;
using mod_add.Selectores;
using mod_add.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para CorteZForm.xaml
    /// </summary>
    public partial class CorteZForm : Window
    {
        private readonly CorteZViewModel ViewModel;
        private TipoCorte TipoCorte { get; set; }
        public CorteZForm(TipoCorte tipoCorte)
        {
            InitializeComponent();
            TipoCorte = tipoCorte;
            
            if (TipoCorte == TipoCorte.TURNO)
            {
                LabelFechaInicio.Content = "Fecha:";
                CorteInicio.Visibility = Visibility.Collapsed;
                LabelFechaCierre.Visibility = Visibility.Collapsed;
                FechaCierre.Visibility = Visibility.Collapsed;
                CorteCierre.Visibility = Visibility.Collapsed;
                HorarioTurno.Visibility = Visibility.Visible;

                FechaInicio.DisplayDateEnd = DateTime.Today.AddDays(-1);
            }
            if (TipoCorte == TipoCorte.PERIODO)
            {
                FechaInicio.DisplayDateEnd = DateTime.Today;
            }

            FechaCierre.DisplayDateEnd = DateTime.Today;

            ViewModel = new CorteZViewModel(tipoCorte);
            DataContext = ViewModel;
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            Generar(TipoDestino.IMPRESION);
        }

        private void ExportarTXT_Click(object sender, RoutedEventArgs e)
        {
            Generar(TipoDestino.EXPORTAR);
        }

        private void ExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            App.HabilitarPrincipal(false);
            IsEnabled = false;

            Respuesta respuesta = new Respuesta
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Generando reporte");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.GenerarExcel();

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();
                IsEnabled = true;

                if (respuesta.TipoRespuesta == TipoRespuesta.FECHA_INACCESIBLE)
                {
                    MessageBox.Show($"No cuenta con la licencia para generar reportes en el mes de {respuesta.Mensaje}", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("No hay información para generar el reporte", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar generar el reporte", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Generar(TipoDestino tipoDestino)
        {
            if (!Validar()) return;

            App.HabilitarPrincipal(false);
            IsEnabled = false;

            Respuesta respuesta = new Respuesta
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Generando reporte");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.Generar(tipoDestino);

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();
                IsEnabled = true;

                if (respuesta.TipoRespuesta == TipoRespuesta.FECHA_INACCESIBLE)
                {
                    MessageBox.Show($"No cuenta con la licencia para generar reportes en el mes de {respuesta.Mensaje}", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("No hay información para generar el reporte", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar generar el reporte", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Reportes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            if (!(comboBox.SelectedItem is Reporte reporte)) return;

            ExportarExcel.IsEnabled = (reporte.TipoReporte == TipoReporte.DETALLADO_VERTICAL || reporte.TipoReporte == TipoReporte.DETALLADO_HORIZONTAL);
        }

        private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Button button)) return;

            if (!(button.Content is Grid grid)) return;

            if (button.IsEnabled)
                grid.Children[0].Opacity = 1d;
            else
                grid.Children[0].Opacity = 0.5d;
        }

        private void FechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TipoCorte == TipoCorte.TURNO)
            {
                ViewModel.AjustarFechaCierre();
            }
            else if (TipoCorte == TipoCorte.PERIODO)
            {
                FechaCierre.DisplayDateStart = ViewModel.FechaInicio;
            }
        }

        private void FechaCierre_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TipoCorte == TipoCorte.PERIODO)
            {
                FechaInicio.DisplayDateEnd = ViewModel.FechaCierre;
            }
        }

        public bool Validar()
        {
            var corteInicio = ViewModel.FechaInicio.AddSeconds(ViewModel.CorteInicio.TimeOfDay.TotalSeconds);
            var corteCierre = ViewModel.FechaCierre.AddSeconds(ViewModel.CorteCierre.TimeOfDay.TotalSeconds);

            if (corteInicio > corteCierre)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha de cierre", "Fecha inicio", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
