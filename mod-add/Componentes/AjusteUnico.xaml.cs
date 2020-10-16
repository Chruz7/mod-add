using mod_add.Datos.ModelosPersonalizados;
using mod_add.Helpers;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
using SR.Datos;
using SRLibrary.SR_DTO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {

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
                if (ViewModel.ObtenerCheque(long.Parse(Folio.Text)) == 0)
                {
                    MessageBox.Show("No se encontró el cheque", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void DetallesCheque_CurrentCellChanged(object sender, System.EventArgs e)
        {
            
        }

        public void ProductoCambio(SR_productos producto)
        {
            if (!(DetallesCheque.CurrentItem is SR_cheqdet cheqdet)) return;

            ViewModel.Cambiar(cheqdet, producto);

            DetallesCheque.Items.Refresh();

            Messenger.Default.Unregister(this);
        }

        private void DetallesCheque_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
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
    }
}
