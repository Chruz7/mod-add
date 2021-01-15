using mod_add.Enums;
using mod_add.Selectores;
using mod_add.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para ReimpresionFoliosForm.xaml
    /// </summary>
    public partial class ReimpresionFoliosForm : Window
    {
        private readonly ReimpresionFoliosViewModel ViewModel;
        public ReimpresionFoliosForm()
        {
            InitializeComponent();
            FechaInicio.DisplayDateEnd = DateTime.Today;
            FechaCierre.DisplayDateEnd = FechaInicio.DisplayDateEnd;

            HabilitarCampos();
            ImprimirSoloModificados.IsEnabled = false;

            ViewModel = new ReimpresionFoliosViewModel();
            DataContext = ViewModel;
        }

        private void ListarFolios_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            IsEnabled = false;

            Respuesta respuesta = new Respuesta
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Buscando folios");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.ObtenerCuentas();

            }).ContinueWith(task =>
            {
                loading.Close();
                IsEnabled = true;
                if (respuesta.TipoRespuesta == TipoRespuesta.HECHO)
                {
                    HabilitarCampos(false);

                    if (ViewModel.BusquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.FISCAL)
                    {
                        ImprimirSoloModificados.IsEnabled = true;
                    }

                    ViewModel.CargarResultados(respuesta);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.FECHA_INACCESIBLE)
                {
                    MessageBox.Show(respuesta.Mensaje, "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("No hay folios para listar", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar listar los folios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LimpiarLista_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LimpiarCampos();
            HabilitarCampos();
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ImprimirEnArchivo)
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        if (!string.IsNullOrWhiteSpace(fbd.SelectedPath) && Directory.Exists(fbd.SelectedPath))
                        {
                            ViewModel.DirectorioExportacion = fbd.SelectedPath;
                        }
                        else
                        {
                            MessageBox.Show("Por favor seleccione un directorio valido", "Directorio", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Respuesta respuesta = new Respuesta
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Imprimiendo");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.Generar();

            }).ContinueWith(task =>
            {
                loading.Close();
                IsEnabled = true;

                if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar imprimir los folios", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BusquedaRegistros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            if (!(comboBox.SelectedItem is BusquedaRegistro busquedaRegistro)) return;

            if (busquedaRegistro.TipoBusquedaRegistro == TipoBusquedaRegistro.FECHA)
            {
                FolioMin.Visibility = Visibility.Collapsed;
                FolioMax.Visibility = Visibility.Collapsed;

                FechaInicio.Visibility = Visibility.Visible;
                FechaCierre.Visibility = Visibility.Visible;
                HoraInicio.Visibility = Visibility.Visible;
                HoraCierre.Visibility = Visibility.Visible;
            }
            if (busquedaRegistro.TipoBusquedaRegistro == TipoBusquedaRegistro.FOLIO)
            {
                FechaInicio.Visibility = Visibility.Collapsed;
                FechaCierre.Visibility = Visibility.Collapsed;
                HoraInicio.Visibility = Visibility.Collapsed;
                HoraCierre.Visibility = Visibility.Collapsed;

                FolioMin.Visibility = Visibility.Visible;
                FolioMax.Visibility = Visibility.Visible;
            }
        }

        private void FechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaCierre.DisplayDateStart = ViewModel.FechaInicio;
        }

        private void FechaCierre_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaInicio.DisplayDateEnd = ViewModel.FechaCierre;
        }

        private void HabilitarCampos(bool habilitar = true)
        {
            ImpresionCuentas.IsEnabled = habilitar;
            BusquedaCuentas.IsEnabled = habilitar;
            BusquedaRegistros.IsEnabled = habilitar;
            FechaInicio.IsEnabled = habilitar;
            FechaCierre.IsEnabled = habilitar;
            HoraInicio.IsEnabled = habilitar;
            HoraCierre.IsEnabled = habilitar;
            FolioMin.IsEnabled = habilitar;
            FolioMax.IsEnabled = habilitar;

            ListarFolios.IsEnabled = habilitar;
            LimpiarLista.IsEnabled = !habilitar;
            //ImprimirSoloModificados.IsEnabled = !habilitar;
            ImprimirEnArchivo.IsEnabled = !habilitar;
            Imprimir.IsEnabled = !habilitar;
        }

        public bool Validar()
        {
            string titulo = "";
            string mensaje = "";

            var corteInicio = ViewModel.FechaInicio.AddSeconds(ViewModel.HoraInicio.TimeOfDay.TotalSeconds);
            var corteCierre = ViewModel.FechaCierre.AddSeconds(ViewModel.HoraCierre.TimeOfDay.TotalSeconds);

            if (corteInicio > corteCierre)
            {
                mensaje = "La fecha de inicio no puede ser mayor a la fecha de cierre";
                titulo = "Fecha inicio";

                MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.FolioMin > ViewModel.FolioMax)
            {
                mensaje = "El folio mínimo no debe ser mayor al folio máximo";
                titulo = "Folio mínimo";

                MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }


            return true;
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

        //private void BusquedaCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (!(sender is ComboBox comboBox)) return;
        //    if (!(comboBox.SelectedItem is BusquedaCuenta busquedaCuenta)) return;

        //    if (busquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.FISCAL)
        //    {
        //        ImprimirSoloModificados.IsEnabled = true;
        //    }
        //    else if (busquedaCuenta.TipoBusquedaCuenta == TipoBusquedaCuenta.NORMAL)
        //    {
        //        ViewModel.ImprimirSoloModificados = false;
        //        ImprimirSoloModificados.IsEnabled = false;
        //    }
        //}
    }
}
