using System.Windows;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para Autenticacion.xaml
    /// </summary>
    public partial class Autenticacion : Window
    {
        public Autenticacion()
        {
            InitializeComponent();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarContrasena())
            {
                App.IrPrincipal();
                Close();
            }
        }

        public bool ValidarContrasena()
        {
            if (string.IsNullOrWhiteSpace(Contrasena.Text))
            {
                MessageBox.Show("Por favor, ingrese su contreseña", "Contraseña", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
