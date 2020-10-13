using mod_add.Helpers;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            ViewModel = new ConfiguracionViewModel();
            Habilitar("P1", false);
            Habilitar("P2", false);
            Habilitar("P3", false);

            DataContext = ViewModel;

            ModificarVentasReales.SelectedItem = ViewModel.Condicionales.Where(x => x.Valor == ViewModel.ModificarVentasReales).FirstOrDefault();
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

            Messenger.Default.Register<dynamic>(this, ProductoSeleccionado);

            SeleccionProducto window = new SeleccionProducto();
            window.ShowDialog();

            
        }

        public void ProductoSeleccionado(dynamic producto)
        {
            switch (TagSeleccionado)
            {
                case "P1":
                    ViewModel.P1_Clave = producto.idproducto;
                    ViewModel.P1_Nombre = "prod1";
                    ViewModel.P1_Precio = string.Format("{0:C}", 10);
                    break;

                case "P2":
                    ViewModel.P2_Clave = producto.idproducto;
                    ViewModel.P2_Nombre = "prod2";
                    ViewModel.P2_Precio = string.Format("{0:C}", 20);
                    break;

                case "P3":
                    ViewModel.P3_Clave = producto.idproducto;
                    ViewModel.P3_Nombre = "prod3";
                    ViewModel.P3_Precio = string.Format("{0:C}", 30);
                    break;
                default:
                    break;
            }
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

                    ViewModel.P1_Clave = "";
                    ViewModel.P1_Porcentaje = 0;
                    ViewModel.P1_Precio = "";
                    ViewModel.P1_Nombre = "";
                    break;

                case "P2":
                    P2_Clave.IsEnabled = habilitar;
                    P2_Buscar.IsEnabled = habilitar;
                    P2_Porcentaje.IsEnabled = habilitar;
                    P2_Nombre.IsEnabled = habilitar;
                    P2_Precio.IsEnabled = habilitar;

                    ViewModel.P2_Clave = "";
                    ViewModel.P2_Porcentaje = 0;
                    ViewModel.P2_Precio = "";
                    ViewModel.P2_Nombre = "";
                    break;

                case "P3":
                    P3_Clave.IsEnabled = habilitar;
                    P3_Buscar.IsEnabled = habilitar;
                    P3_Porcentaje.IsEnabled = habilitar;
                    P3_Nombre.IsEnabled = habilitar;
                    P3_Precio.IsEnabled = habilitar;

                    ViewModel.P3_Clave = "";
                    ViewModel.P3_Porcentaje = 0;
                    ViewModel.P3_Precio = "";
                    ViewModel.P3_Nombre = "";
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
                if (ViewModel.Guardar() == 1)
                {
                    MessageBox.Show("La configuación se guardó con exito", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al intentar guardar la información, por favor intentelo de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
