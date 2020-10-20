using mod_add.Componentes;
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
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        private AjusteUnico AjusteUnico { get; set; }
        private AjusteMasivo AjusteMasivo { get; set; }
        private Bitacora Bitacora { get; set; }
        private Configuracion Configuracion { get; set; }

        public Principal()
        {
            InitializeComponent();
        }

        private void AbrirAjusteUnico_Click(object sender, RoutedEventArgs e)
        {
            if (AjusteUnico == null) AjusteUnico = new AjusteUnico();

            CargarComponente(AjusteUnico);
        }

        private void AbrirAjusteMasivo_Click(object sender, RoutedEventArgs e)
        {
            if (AjusteMasivo == null) AjusteMasivo = new AjusteMasivo();

            CargarComponente(AjusteMasivo);
        }

        private void AbrirBitacora_Click(object sender, RoutedEventArgs e)
        {
            if (Bitacora == null) Bitacora = new Bitacora();

            CargarComponente(Bitacora);
        }

        private void AbrirConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            if (Configuracion == null) Configuracion = new Configuracion();
            Configuracion.Refrescar();
            CargarComponente(Configuracion);
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("¿Salir del sistema?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

        public void CargarComponente(UserControl userControl)
        {
            Contenido.Children.Clear();
            Contenido.Children.Add(userControl);
        }

        private void AbrirCambioContrasena_Click(object sender, RoutedEventArgs e)
        {
            CambiarContrasena window = new CambiarContrasena();
            window.ShowDialog();
        }
    }
}
