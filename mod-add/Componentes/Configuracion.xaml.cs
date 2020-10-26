using mod_add.Enums;
using mod_add.Helpers;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
using SRLibrary.SR_DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace mod_add.Componentes
{
    /// <summary>
    /// Lógica de interacción para Configuracion.xaml
    /// </summary>
    public partial class Configuracion : UserControl
    {
        private readonly ConfiguracionViewModel ViewModel;
        private string TagSeleccionado { get; set; }

        public Configuracion()
        {
            InitializeComponent();
            AbrirSeleccionProductos.IsEnabled = false;
            Habilitar("P1", false);
            Habilitar("P2", false);
            Habilitar("P3", false);
            Habilitar("P4", false);
            Habilitar("P5", false);

            ViewModel = new ConfiguracionViewModel();

            DataContext = ViewModel;

            ModificarVentasReales.SelectedItem = ViewModel.Condicionales.Where(x => x.Valor == ViewModel.ModificarVentasReales).FirstOrDefault();
        }

        public void Refrescar()
        {
            ViewModel.Inicializar();
        }

        private void ModificarVentasReales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            if (!(comboBox.SelectedItem is Condicional condicional)) return;

            ViewModel.ModificarVentasReales = condicional.Valor;
        }

        private void EliminarProductosSeleccionados_Checked(object sender, RoutedEventArgs e)
        {
            AbrirSeleccionProductos.IsEnabled = true;
        }

        private void EliminarProductosSeleccionados_Unchecked(object sender, RoutedEventArgs e)
        {
            AbrirSeleccionProductos.IsEnabled = false;
        }

        private void AbrirSeleccionProductos_Click(object sender, RoutedEventArgs e)
        {
            SeleccionProductosEliminar window = new SeleccionProductosEliminar();
            window.ShowDialog();
        }

        private void BuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;

            TagSeleccionado = button.Tag.ToString();

            Messenger.Default.Register<SR_productos>(this, ProductoSeleccionado);

            SeleccionProducto window = new SeleccionProducto();
            window.ShowDialog();
        }

        public void ProductoSeleccionado(SR_productos producto)
        {
            if (producto != null)
            {
                switch (TagSeleccionado)
                {
                    case "P1":
                        ViewModel.P1_Clave = producto.idproducto;
                        ViewModel.P1_Nombre = producto.descripcion;
                        ViewModel.P1_Precio = producto.Detalle.Display_precio;
                        break;

                    case "P2":
                        ViewModel.P2_Clave = producto.idproducto;
                        ViewModel.P2_Nombre = producto.descripcion;
                        ViewModel.P2_Precio = producto.Detalle.Display_precio;
                        break;

                    case "P3":
                        ViewModel.P3_Clave = producto.idproducto;
                        ViewModel.P3_Nombre = producto.descripcion;
                        ViewModel.P3_Precio = producto.Detalle.Display_precio;
                        break;

                    case "P4":
                        ViewModel.P4_Clave = producto.idproducto;
                        ViewModel.P4_Nombre = producto.descripcion;
                        ViewModel.P4_Precio = producto.Detalle.Display_precio;
                        break;

                    case "P5":
                        ViewModel.P5_Clave = producto.idproducto;
                        ViewModel.P5_Nombre = producto.descripcion;
                        ViewModel.P5_Precio = producto.Detalle.Display_precio;
                        break;
                    default:
                        break;
                }
            }

            Messenger.Default.Unregister(this);
        }

        private void Reemplazar_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) return;

            Habilitar(checkBox.Tag.ToString());
        }

        private void Reemplazar_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) return;

            Habilitar(checkBox.Tag.ToString(), false);
        }

        private void Habilitar(string tag, bool habilitar = true)
        {
            switch (tag)
            {
                case "P1":
                    P1_Clave.IsEnabled = habilitar;
                    P1_Buscar.IsEnabled = habilitar;
                    P1_Porcentaje.IsEnabled = habilitar;
                    P1_Nombre.IsEnabled = habilitar;
                    P1_Precio.IsEnabled = habilitar;
                    break;

                case "P2":
                    P2_Clave.IsEnabled = habilitar;
                    P2_Buscar.IsEnabled = habilitar;
                    P2_Porcentaje.IsEnabled = habilitar;
                    P2_Nombre.IsEnabled = habilitar;
                    P2_Precio.IsEnabled = habilitar;
                    break;

                case "P3":
                    P3_Clave.IsEnabled = habilitar;
                    P3_Buscar.IsEnabled = habilitar;
                    P3_Porcentaje.IsEnabled = habilitar;
                    P3_Nombre.IsEnabled = habilitar;
                    P3_Precio.IsEnabled = habilitar;
                    break;

                case "P4":
                    P4_Clave.IsEnabled = habilitar;
                    P4_Buscar.IsEnabled = habilitar;
                    P4_Porcentaje.IsEnabled = habilitar;
                    P4_Nombre.IsEnabled = habilitar;
                    P4_Precio.IsEnabled = habilitar;
                    break;

                case "P5":
                    P5_Clave.IsEnabled = habilitar;
                    P5_Buscar.IsEnabled = habilitar;
                    P5_Porcentaje.IsEnabled = habilitar;
                    P5_Nombre.IsEnabled = habilitar;
                    P5_Precio.IsEnabled = habilitar;
                    break;

                default:
                    break;
            }
        }

        private bool Validar()
        {
            if (ViewModel.P1_Reemplazar && string.IsNullOrEmpty(ViewModel.P1_Clave))
            {
                MessageBox.Show("Por favor, seleccione el 1er producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P2_Reemplazar && string.IsNullOrEmpty(ViewModel.P2_Clave))
            {
                MessageBox.Show("Por favor, seleccione el 2do producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P3_Reemplazar && string.IsNullOrEmpty(ViewModel.P3_Clave))
            {
                MessageBox.Show("Por favor, seleccione el 3er producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P4_Reemplazar && string.IsNullOrEmpty(ViewModel.P4_Clave))
            {
                MessageBox.Show("Por favor, seleccione el 4to producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P5_Reemplazar && string.IsNullOrEmpty(ViewModel.P5_Clave))
            {
                MessageBox.Show("Por favor, seleccione el 5to producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }



            if (ViewModel.P1_Reemplazar && ViewModel.P1_Porcentaje == 0)
            {
                MessageBox.Show("Por favor, ingrese el porcentaje del 1er producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P2_Reemplazar && ViewModel.P2_Porcentaje == 0)
            {
                MessageBox.Show("Por favor, ingrese el porcentaje del 2do producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P3_Reemplazar && ViewModel.P3_Porcentaje == 0)
            {
                MessageBox.Show("Por favor, ingrese el porcentaje del 3er producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P4_Reemplazar && ViewModel.P4_Porcentaje == 0)
            {
                MessageBox.Show("Por favor, ingrese el porcentaje del 4to producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ViewModel.P5_Reemplazar && ViewModel.P5_Porcentaje == 0)
            {
                MessageBox.Show("Por favor, ingrese el porcentaje del 5to producto a reemplazar", "Producto reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }



            int porcentajeTotal = 0;
            bool reemplazar = false;

            if (ViewModel.P1_Reemplazar)
            {
                porcentajeTotal += ViewModel.P1_Porcentaje;
                reemplazar = true;
            }

            if (ViewModel.P2_Reemplazar)
            {
                porcentajeTotal += ViewModel.P2_Porcentaje;
                reemplazar = true;
            }

            if (ViewModel.P3_Reemplazar)
            {
                porcentajeTotal += ViewModel.P3_Porcentaje;
                reemplazar = true;
            }

            if (ViewModel.P4_Reemplazar)
            {
                porcentajeTotal += ViewModel.P4_Porcentaje;
                reemplazar = true;
            }

            if (ViewModel.P5_Reemplazar)
            {
                porcentajeTotal += ViewModel.P5_Porcentaje;
                reemplazar = true;
            }

            if (reemplazar && porcentajeTotal != 100)
            {
                MessageBox.Show("El porcentaje total de los productos a reemplazar debe ser igual 100%", "Porcentaje de reemplazo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (Validar())
            {
                App.HabilitarPrincipal(false);

                TipoRespuesta respuesta = TipoRespuesta.NADA;
                LoadingWindow loading = new LoadingWindow();
                loading.AgregarMensaje("Guardando cambios");
                loading.Show();

                Task.Factory.StartNew(() =>
                {
                    respuesta = ViewModel.Guardar();

                }).ContinueWith(task =>
                {
                    loading.Close();
                    App.HabilitarPrincipal();

                    if (respuesta == TipoRespuesta.HECHO)
                    {
                        MessageBox.Show("La configuación se guardó con exito", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (respuesta == TipoRespuesta.ERROR)
                    {
                        MessageBox.Show("Hubo un error al intentar guardar la información, por favor intentelo de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Porcentaje_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
        }
    }
}
