using mod_add.Datos.ModelosPersonalizados;
using mod_add.Helpers;
using mod_add.ViewModels;
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
        public SeleccionProducto()
        {
            InitializeComponent();
            ViewModel = new SeleccionProductoViewModel();

            DataContext = ViewModel;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

            if (!(dataGrid.CurrentItem is ProductoSeleccion productoSeleccion)) return;

            Messenger.Default.Send(productoSeleccion);

            Close();
        }
    }
}
