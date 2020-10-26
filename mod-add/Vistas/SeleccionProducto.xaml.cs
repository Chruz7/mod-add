using mod_add.Helpers;
using mod_add.ViewModels;
using SRLibrary.SR_DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para SeleccionProducto.xaml
    /// </summary>
    public partial class SeleccionProducto : Window
    {
        private readonly SeleccionProductoViewModel ViewModel;
        private SR_productos Producto { get; set; }
        public SeleccionProducto()
        {
            InitializeComponent();
            ViewModel = new SeleccionProductoViewModel();

            DataContext = ViewModel;
        }

        private void Grupos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            if (!(comboBox.SelectedItem is SR_grupos grupo)) return;

            var productos = grupo.Productos;

            ViewModel.ObtenerProductosSR(productos);
        }

        private void Buscador_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.Filtrar(Buscador.Text);
            }
        }

        private void Productos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            if (!(dataGrid.CurrentItem is SR_productos producto)) return;

            Producto = producto;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Producto = null;
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Messenger.Default.Send(Producto);
        }
    }
}
