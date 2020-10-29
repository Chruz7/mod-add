using mod_add.Datos.Enums;
using mod_add.Enums;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mod_add.Componentes
{
    /// <summary>
    /// Lógica de interacción para AjusteMasivo.xaml
    /// </summary>
    public partial class AjusteMasivo : UserControl
    {
        private readonly AjusteMasivoViewModel ViewModel;
        public AjusteMasivo()
        {
            InitializeComponent();

            HabilitarControles(false);
            Aplicar.IsEnabled = false;
            Cancelar.IsEnabled = false;

            FechaInicio.DisplayDateEnd = App.FechaMaxima;
            FechaCierre.DisplayDateEnd = App.FechaMaxima;

            ViewModel = new AjusteMasivoViewModel();
            DataContext = ViewModel;
        }

        private void NuevaBusqueda_Click(object sender, RoutedEventArgs e)
        {
            HabilitarControles();
            ViewModel.InicializarControles();
        }

        private void GenerarVistaPrevia_Click(object sender, RoutedEventArgs e)
        {
            App.HabilitarPrincipal(false);

            RespuestaBusquedaMasiva respuesta = new RespuestaBusquedaMasiva
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Buscando cuentas");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.ObtenerChequesSR();

                if (respuesta.TipoRespuesta == TipoRespuesta.HECHO)
                {
                    loading.AgregarMensaje("Creando registros temporales");
                    ViewModel.CrearRegistrosTemporales(respuesta);

                    loading.AgregarMensaje("Procesando información");
                    ViewModel.GenerarVistaPrevia();
                }

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();

                if (respuesta.TipoRespuesta == TipoRespuesta.HECHO)
                {
                    ViewModel.CargarResultados();
                    DetalleModificacionCheques.Items.Refresh();
                    Aplicar.IsEnabled = true;
                    Cancelar.IsEnabled = true;
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("No se encontraron cuentas", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar buscar las cuentas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Aplicar_Click(object sender, RoutedEventArgs e)
        {
            App.HabilitarPrincipal(false);

            TipoRespuesta respuesta = TipoRespuesta.NADA;

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Guardando cambios");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.Guardar();

                if (respuesta == TipoRespuesta.HECHO)
                {
                    loading.AgregarMensaje("Registrando bitácora");
                    ViewModel.ResgistrarBitacora();
                }
            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();
                if (respuesta == TipoRespuesta.HECHO)
                {
                    Aplicar.IsEnabled = false;
                    MessageBox.Show("Se guardaron los cambios correctamente", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar guardar los cambios, por favor intente de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            //HabilitarControles(false);
            ViewModel.InicializarControles();
            GenerarVistaPrevia.IsEnabled = false;
            Aplicar.IsEnabled = false;
            Cancelar.IsEnabled = false;
        }

        private void Turno_Checked(object sender, RoutedEventArgs e)
        {
            Lbl_FechaCierre.Visibility = Visibility.Hidden;
            FechaCierre.Visibility = Visibility.Hidden;
            HorarioTurno.Visibility = Visibility.Visible;

            FechaInicio.DisplayDateEnd = DateTime.Today;
        }

        private void Turno_Unchecked(object sender, RoutedEventArgs e)
        {
            Lbl_FechaCierre.Visibility = Visibility.Visible;
            FechaCierre.Visibility = Visibility.Visible;
            HorarioTurno.Visibility = Visibility.Hidden;

            FechaCierre.DisplayDateStart = ViewModel.FechaCierre;
            FechaCierre.DisplayDateEnd = DateTime.Today;
            ViewModel.FechaCierre = DateTime.Today;
        }

        private void HabilitarControles(bool habilitar = true)
        {
            Turno.IsEnabled = habilitar;
            Periodo.IsEnabled = habilitar;
            FechaInicio.IsEnabled = habilitar;
            FechaCierre.IsEnabled = habilitar;
            Procesos.IsEnabled = habilitar;
            ImporteMinimoAjustable.IsEnabled = habilitar;
            PorcentajeObjetivo.IsEnabled = habilitar;
            ImporteObjetivo.IsEnabled = habilitar;
            CuentaPagadaTarjerta.IsEnabled = habilitar;
            CuentaPagadaVales.IsEnabled = habilitar;
            CuentaPagadaOtros.IsEnabled = habilitar;
            CuentaFacturada.IsEnabled = habilitar;
            CuentaNotaConsumo.IsEnabled = habilitar;
            GenerarVistaPrevia.IsEnabled = habilitar;
        }

        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void FechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaCierre.DisplayDateStart = ViewModel.FechaInicio;
        }

        private void FechaCierre_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FechaInicio.DisplayDateEnd = ViewModel.FechaCierre;
        }

        private void PorcentajeObjetivo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
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

        private void Procesos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (!(comboBox.SelectedItem is Proceso proceso)) return;
            //ViewModel.Proceso = proceso;
            if (proceso.TipoProceso == TipoProceso.FOLIOS)
                DetalleModificacionCheques.Columns[13].Header = "Eliminar";
            else if (proceso.TipoProceso == TipoProceso.PRODUCTOS)
                DetalleModificacionCheques.Columns[13].Header = "Modificar";
        }
    }
}
