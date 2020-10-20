using mod_add.Enums;
using mod_add.Helpers;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
using SR.Datos;
using SRLibrary.SR_DTO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace mod_add.Componentes
{
    /// <summary>
    /// Lógica de interacción para AjusteUnico.xaml
    /// </summary>
    public partial class AjusteUnico : UserControl
    {
        private readonly AjusteUnicoViewModel ViewModel;

        public AjusteUnico()
        {
            InitializeComponent();
            ViewModel = new AjusteUnicoViewModel();

            DataContext = ViewModel;

            CambiarPrecios.SelectedItem = ViewModel.Condicionales.Where(x => x.Valor == ViewModel.CambiarPrecio).FirstOrDefault();

            HabilitarComponentes(false);
        }

        private void Aniadir_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<SR_productos>(this, ProductoSelecionado);

            SeleccionProducto window = new SeleccionProducto();
            window.ShowDialog();
        }

        private void ProductoSelecionado(SR_productos producto)
        {
            ViewModel.Aniadir(producto);

            DetallesCheque.Items.Refresh();

            Messenger.Default.Unregister(this);
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (!(DetallesCheque.SelectedItem is SR_cheqdet cheqdet)) return;

            ViewModel.Eliminar(cheqdet);

            DetallesCheque.Items.Refresh();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Guardar() == 1)
            {
                MessageBox.Show("El ajuste se realizó con exito.", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ocurrió un error al internar realizar el ajuste, por favor intente de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Inicializar();

            CambiarPrecios.SelectedItem = ViewModel.Condicionales.Where(x => x.Valor == ViewModel.CambiarPrecio).FirstOrDefault();

            HabilitarComponentes(false);
        }

        private void CambiarPrecios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            if (!(comboBox.SelectedItem is Condicional condicional)) return;

            ViewModel.CambiarPrecio = condicional.Valor;
        }

        private void Folio_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(Folio.Text))
            {
                var respuesta = ViewModel.ObtenerCheque();

                if (respuesta == Respuesta.HECHO)
                {
                    HabilitarComponentes();
                }
                else if (respuesta == Respuesta.CHEQUE_NO_ENCONTRADO)
                {
                    MessageBox.Show("No se encontró el cheque.", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta == Respuesta.CHEQUE_CANCELADO)
                {
                    MessageBox.Show("Cheque cancelado.", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta == Respuesta.CHEQUE_CON_MULTIPLE_FORMA_PAGO)
                {
                    MessageBox.Show("No se puede procesar el cheque por que tiene más de una forma de pago.", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Error al intentar buscar el cheque.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Folio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void Propina_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.OemPeriod)
            {
                bool valido = true;

                if (e.Key == Key.OemPeriod && Propina.Text.Length == 0) valido = false;

                if (e.Key == Key.OemPeriod && Propina.Text.Contains(".")) valido = false;

                var result = Propina.Text.Split('.');

                if (result.Length == 2)
                {
                    valido = result[0].Length > 0 && result[1].Length < 4;
                }

                if (valido)
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            else
                e.Handled = true;
        }

        

        public void ProductoCambio(SR_productos producto)
        {
            if (!(DetallesCheque.CurrentItem is SR_cheqdet cheqdet)) return;

            ViewModel.Cambiar(cheqdet, producto);

            DetallesCheque.Items.Refresh();

            Messenger.Default.Unregister(this);
        }

        private void DetallesCheque_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string header = DetallesCheque.CurrentColumn.Header.ToString();

            if (header.Equals("Clave"))
            {
                Messenger.Default.Register<SR_productos>(this, ProductoCambio);
                SeleccionProducto window = new SeleccionProducto();
                window.ShowDialog();
            }
        }

        private void HabilitarComponentes(bool habilitar = true)
        {
            Aniadir.IsEnabled = habilitar;
            Eliminar.IsEnabled = habilitar;
            Aceptar.IsEnabled = habilitar;
            Cancelar.IsEnabled = habilitar;

            CambiarPrecios.IsEnabled = habilitar;
            Fecha.IsEnabled = habilitar;
            Personas.IsEnabled = habilitar;
            Cliente.IsEnabled = habilitar;
            Descuento.IsEnabled = habilitar;
            Propina.IsEnabled = habilitar;
            Subtotal.IsEnabled = habilitar;
            Total.IsEnabled = habilitar;
        }

        private void DetallesCheque_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DetallesCheque.Dispatcher.BeginInvoke(new Action(() => RefrescarControles()), DispatcherPriority.Background);
        }

        public void RefrescarControles()
        {
            ViewModel.AjustarCheque();
            DetallesCheque.Items.Refresh();
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
