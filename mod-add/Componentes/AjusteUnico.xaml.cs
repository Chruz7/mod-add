using mod_add.Selectores;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
