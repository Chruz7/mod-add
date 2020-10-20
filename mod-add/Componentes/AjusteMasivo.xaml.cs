using mod_add.ViewModels;
using System;
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

            ViewModel = new AjusteMasivoViewModel();
            DataContext = ViewModel;
        }

        private void Aplicar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ObtenerInformacionSR();
            //ViewModel.AjustarCheques();
        }

        private void Busqueda_Click(object sender, RoutedEventArgs e)
        {
            //HabilitarControles();
            ViewModel.InicializarControles();
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

        private void HabilitarControles(bool habilitar = true)
        {
            Turno.IsEnabled = habilitar;
            Periodo.IsEnabled = habilitar;
            FechaInicio.IsEnabled = habilitar;
            FechaCierre.IsEnabled = habilitar;
            ImporteAjuste.IsEnabled = habilitar;
            PorcentajeObjetivo.IsEnabled = habilitar;
            ImporteObjetivo.IsEnabled = habilitar;
            CuentaPagadaTarjerta.IsEnabled = habilitar;
            CuentaPagadaVales.IsEnabled = habilitar;
            CuentaPagadaOtros.IsEnabled = habilitar;
            CuentaFacturada.IsEnabled = habilitar;
            CuentaNotaConsumo.IsEnabled = habilitar;
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

        private void FechaInicio_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
