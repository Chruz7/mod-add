using mod_add.Enums;
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
            //HabilitarControles(false);

            FechaCierre.DisplayDateEnd = DateTime.Today;
            FechaInicio.DisplayDateEnd = DateTime.Today;

            Aplicar.IsEnabled = false;
            Cancelar.IsEnabled = false;

            ViewModel = new AjusteMasivoViewModel();
            DataContext = ViewModel;
        }

        private void Aplicar_Click(object sender, RoutedEventArgs e)
        {
            App.HabilitarPrincipal(false);

            Respuesta respuesta = Respuesta.NADA;

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Guardando cambios");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.GuardarCambios();

                if (respuesta == Respuesta.HECHO)
                {
                    loading.AgregarMensaje("Registrando bitácora");
                    ViewModel.ResgistrarBitacora();

                    MessageBox.Show("Se guardaron los cambios correctamente", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta == Respuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar guardar los cambios, por favor intente de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void NuevaBusqueda_Click(object sender, RoutedEventArgs e)
        {
            App.HabilitarPrincipal(false);

            Respuesta respuesta = Respuesta.NADA;

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Buscando cuentas");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.ObtenerChequesSR();

                if (respuesta == Respuesta.HECHO)
                {
                    loading.AgregarMensaje("Procesando información");
                    ViewModel.Proceso();
                }
                else if (respuesta == Respuesta.CHEQUE_NO_ENCONTRADO)
                {
                    MessageBox.Show("No se encontraron cuentas", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta == Respuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar buscar las cuentas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();

                if (respuesta == Respuesta.HECHO)
                {
                    ViewModel.CargarResultados();
                    DetalleModificacionCheques.Items.Refresh();
                    Aplicar.IsEnabled = true;
                    Cancelar.IsEnabled = true;
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            //HabilitarControles(false);
            ViewModel.InicializarControles();
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

        //private void HabilitarControles(bool habilitar = true)
        //{
        //    Turno.IsEnabled = habilitar;
        //    Periodo.IsEnabled = habilitar;
        //    FechaInicio.IsEnabled = habilitar;
        //    FechaCierre.IsEnabled = habilitar;
        //    ImporteMinimoAjustable.IsEnabled = habilitar;
        //    PorcentajeObjetivo.IsEnabled = habilitar;
        //    ImporteObjetivo.IsEnabled = habilitar;
        //    CuentaPagadaTarjerta.IsEnabled = habilitar;
        //    CuentaPagadaVales.IsEnabled = habilitar;
        //    CuentaPagadaOtros.IsEnabled = habilitar;
        //    CuentaFacturada.IsEnabled = habilitar;
        //    CuentaNotaConsumo.IsEnabled = habilitar;
        //}

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
            ViewModel.RefrescarControles();
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
    }
}
