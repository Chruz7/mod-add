using mod_add.Enums;
using mod_add.ViewModels;
using System.Windows;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para CambiarContrasena.xaml
    /// </summary>
    public partial class CambiarContrasena : Window
    {
        private readonly CambioContrasenaViewModel ViewModel;
        public CambiarContrasena()
        {
            InitializeComponent();
            ViewModel = new CambioContrasenaViewModel();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ContrasenaActual = ContrasenaActual.Password;
            ViewModel.ContrasenaNueva = ContrasenaNueva.Password;

            TipoRespuesta respuesta = ViewModel.CambiarContrasena();
            if (respuesta == TipoRespuesta.HECHO)
            {
                MessageBox.Show("La contraseña se actualizó correctamente", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else if (respuesta == TipoRespuesta.CONTRASENA_INCORRECTA)
            {
                MessageBox.Show("La contraseña actual es incorrecta", "Contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (respuesta == TipoRespuesta.LONGITUD_INCORRECTA)
            {
                MessageBox.Show("La contraseña nueva debe contener entre 8 y 20 caracteres", "Contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (respuesta == TipoRespuesta.ERROR)
            {
                MessageBox.Show("Hubo un error al intentar actualizar la contraseña, por favor intentalo de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
