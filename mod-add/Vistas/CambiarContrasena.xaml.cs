using mod_add.Enums;
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

            Respuesta respuesta = ViewModel.CambiarContrasena();
            if (respuesta == Respuesta.HECHO)
            {
                MessageBox.Show("La contraseña se actualizó correctamente", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else if (respuesta == Respuesta.CONTRASENA_INCORRECTA)
            {
                MessageBox.Show("La contraseña actual es incorrecta", "Contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (respuesta == Respuesta.LONGITUD_INCORRECTA)
            {
                MessageBox.Show("La contraseña nueva debe contener entre 8 y 20 caracteres", "Contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (respuesta == Respuesta.ERROR)
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
