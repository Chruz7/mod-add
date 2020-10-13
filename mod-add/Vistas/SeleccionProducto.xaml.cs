using mod_add.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        }

        private void Buscador_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Filtar(Buscador.Text);
            }
        }

        public void Filtar(string texto)
        {

        }
    }
}
