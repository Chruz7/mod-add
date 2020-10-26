using mod_add.Enums;
using mod_add.Helpers;
using mod_add.ViewModels;
using SR.Datos;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para SeleccionClientes.xaml
    /// </summary>
    public partial class SeleccionClientes : Window
    {
        private readonly SeleccionClienteViewModel ViewModel;
        private SR_clientes Cliente { get; set; }
        public SeleccionClientes()
        {
            InitializeComponent();
            ViewModel = new SeleccionClienteViewModel();

            DataContext = ViewModel;
            Cliente = null;
        }

        private void Buscador_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TipoRespuesta respuesta = ViewModel.ObtenerClientesSR();

                if (respuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("Sin resultados", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Clientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            if (!(dataGrid.CurrentItem is SR_clientes cliente)) return;

            Cliente = cliente;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Cliente = null;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Messenger.Default.Send(Cliente);
        }
    }
}
