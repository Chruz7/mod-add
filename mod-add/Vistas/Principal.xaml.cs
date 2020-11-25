using mod_add.Componentes;
using mod_add.Enums;
using System.Windows;
using System.Windows.Controls;

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

        private void AbrirFormCortePeriodo_Click(object sender, RoutedEventArgs e)
        {
            CorteZForm form = new CorteZForm(TipoCorte.PERIODO)
            {
                Title = "Corte por periódo"
            };
            form.ShowDialog();
        }

        private void AbrirFormCorteZ_Click(object sender, RoutedEventArgs e)
        {
            CorteZForm form = new CorteZForm(TipoCorte.TURNO)
            {
                Title = "Corte Z"
            };
            form.ShowDialog();
        }

        private void AbrirFormReimpresionFolios_Click(object sender, RoutedEventArgs e)
        {
            ReimpresionFoliosForm form = new ReimpresionFoliosForm();
            form.ShowDialog();
        }

        private void AbrirBitacora_Click(object sender, RoutedEventArgs e)
        {
            if (Bitacora == null) Bitacora = new Bitacora();
            Bitacora.Refrescar();
            CargarComponente(Bitacora);
        }

        private void AbrirConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            if (Configuracion == null) Configuracion = new Configuracion();
            Configuracion.Refrescar();
            CargarComponente(Configuracion);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("¿Salir del sistema?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void AbrirCambioContrasena_Click(object sender, RoutedEventArgs e)
        {
            CambiarContrasena window = new CambiarContrasena();
            window.ShowDialog();
        }

        private void AbrirLicencia_Click(object sender, RoutedEventArgs e)
        {
            LicenciaForm window = new LicenciaForm();
            window.ShowDialog();
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void CargarComponente(UserControl userControl)
        {
            Contenido.Children.Clear();
            Contenido.Children.Add(userControl);
        }
    }
}
