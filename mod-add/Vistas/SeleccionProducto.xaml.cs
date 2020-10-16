using mod_add.Datos.ModelosPersonalizados;
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

            if (!(dataGrid.CurrentItem is SR_productos producto)) return;

            Messenger.Default.Send(producto);

            Close();
        }

        private void Grupos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            if (!(comboBox.SelectedItem is SR_grupos grupo)) return;

            var productos = grupo.Productos;

            ViewModel.ObtenerProductosSR(grupo.Productos);
        }
    }
}
