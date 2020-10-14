using mod_add.Helpers;
using mod_add.ViewModels;
using SRLibrary.SR_DTO;
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
    /// Lógica de interacción para SeleccionGrupo.xaml
    /// </summary>
    public partial class SeleccionGrupo : Window
    {
        private readonly SeleccionGrupoViewModel ViewModel;
        public SeleccionGrupo()
        {
            InitializeComponent();
            ViewModel = new SeleccionGrupoViewModel();
            DataContext = ViewModel;
        }

        private void Grupos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            if (!(dataGrid.CurrentItem is SR_grupos grupo)) return;

            Messenger.Default.Send(grupo);
            Close();
        }

        private void Buscador_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.Filtrar(Buscador.Text);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
