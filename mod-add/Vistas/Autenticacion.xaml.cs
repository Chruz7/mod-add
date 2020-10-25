using mod_add.Enums;
using mod_add.ViewModels;
using System.Windows;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para Autenticacion.xaml
    /// </summary>
    public partial class Autenticacion : Window
    {
        private readonly AutenticacionViewModel ViewModel;

        public Autenticacion()
        {
            InitializeComponent();
            ViewModel = new AutenticacionViewModel();
            DataContext = ViewModel;
#if DEBUG
            Contrasena.Password = "Ok123456";
#endif
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Contrasena = Contrasena.Password;
            TipoRespuesta respuesta = ViewModel.Autenticar();

            if (respuesta == TipoRespuesta.HECHO)
            {
                App.IrPrincipal();
                Close();
            }
            else if (respuesta == TipoRespuesta.CONTRASENA_INCORRECTA)
            {
                MessageBox.Show("La contraseña es incorrecta", "Contraseña", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
