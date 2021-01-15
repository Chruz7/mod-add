using mod_add.Enums;
using mod_add.ViewModels;
using mod_add.Vistas;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mod_add.Componentes
{
    /// <summary>
    /// Lógica de interacción para Bitacora.xaml
    /// </summary>
    public partial class Bitacora : UserControl
    {
        private readonly BitacoraViewModel ViewModel;
        public Bitacora()
        {
            InitializeComponent();
            ViewModel = new BitacoraViewModel();

            DataContext = ViewModel;
        }

        public void Refrescar()
        {
            ViewModel.ObtenerBitacora();
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            App.HabilitarPrincipal(false);

            TipoRespuesta respuesta = TipoRespuesta.NADA;
            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Exportando registros");
            loading.Show();

            Task.Factory.StartNew(() =>
            {

                respuesta = ViewModel.ExportarExcel();

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();

                if (respuesta == TipoRespuesta.HECHO)
                {
                    MessageBox.Show("La bitácora se exportó corretamente", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (respuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("La bitácora no tiene registros", "Bitácora", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar exportar la botácora, por favor intentelo de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
