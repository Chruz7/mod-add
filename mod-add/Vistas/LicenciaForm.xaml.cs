using DocumentFormat.OpenXml.Wordprocessing;
using mod_add.Enums;
using mod_add.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para LicenciaForm.xaml
    /// </summary>
    public partial class LicenciaForm : Window
    {
        private readonly LicenciaViewModel ViewModel;
        public LicenciaForm()
        {
            InitializeComponent();
            ViewModel = new LicenciaViewModel();

            DataContext = ViewModel;
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            App.HabilitarPrincipal(false);

            TipoRespuesta respuesta = TipoRespuesta.NADA;
            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Guardando cambios");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.Guardar();

                if (respuesta == TipoRespuesta.HECHO)
                {
                    ViewModel.ObtenerRegistrosLicencia();
                }

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();
                ViewModel.Licencia = "";

                if (respuesta == TipoRespuesta.HECHO)
                {
                    MessageBox.Show("La licencia se guardó con exito", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta == TipoRespuesta.LICENCIA_INCORRECTA)
                {
                    MessageBox.Show("La licencia es incorrecta", "Licencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta == TipoRespuesta.EXITE)
                {
                    MessageBox.Show("La licencia ya existe", "Licencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar guardar la liencia, por favor intentelo de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
