using mod_add.Helpers;
using mod_add.ViewModels;
using SRLibrary.SR_DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para SeleccionGrupo.xaml
    /// </summary>
    public partial class SeleccionGrupo : Window
    {
        private readonly SeleccionGrupoViewModel ViewModel;
        private SR_grupos Grupo { get; set; }
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

            Grupo = grupo;
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
            Grupo = null;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Send(Grupo);
        }
    }
}
