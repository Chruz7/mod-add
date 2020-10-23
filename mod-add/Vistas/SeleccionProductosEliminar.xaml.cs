using mod_add.Datos.Modelos;
using mod_add.Enums;
using mod_add.Helpers;
using mod_add.ViewModels;
using SRLibrary.SR_DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para SeleccionProductosEliminar.xaml
    /// </summary>
    public partial class SeleccionProductosEliminar : Window
    {
        private readonly EliminarProductosViewModel ViewModel;
        public SeleccionProductosEliminar()
        {
            InitializeComponent();
            ViewModel = new EliminarProductosViewModel();

            DataContext = ViewModel;
        }

        private void SeleccionarGrupo_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<SR_grupos>(this, GrupoSeleccionado);

            SeleccionGrupo window = new SeleccionGrupo();
            window.ShowDialog();
        }

        public void GrupoSeleccionado(SR_grupos grupo)
        {
            foreach (var productoElimnar in ViewModel.ProductosEliminar)
            {
                if (productoElimnar.Grupo == grupo.idgrupo)
                {
                    productoElimnar.Eliminar = true;
                }
            }

            ProductosEliminar.Items.Refresh();

            Messenger.Default.Unregister(this);
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            Respuesta respuesta = Respuesta.NADA;
            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Guardando cambios");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.Guardar();

            }).ContinueWith(task =>
            {
                loading.Close();
                IsEnabled = true;
                if (respuesta == Respuesta.HECHO)
                {
                    MessageBox.Show("La configuación se guardó con exito", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Error al intentar guardar la información, por favor intentelo de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
