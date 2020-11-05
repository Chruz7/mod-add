using mod_add.Enums;
using mod_add.Helpers;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
using SR.Datos;
using SRLibrary.SR_DTO;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace mod_add.Componentes
{
    /// <summary>
    /// Lógica de interacción para AjusteUnico.xaml
    /// </summary>
    public partial class AjusteUnico : UserControl
    {
        private readonly AjusteUnicoViewModel ViewModel;

        public AjusteUnico()
        {
            InitializeComponent();
            ViewModel = new AjusteUnicoViewModel();

            DataContext = ViewModel;

            //CambiarPrecios.SelectedItem = ViewModel.Condicionales.Where(x => x.Valor == ViewModel.CambiarPrecio).FirstOrDefault();

            HabilitarComponentes(false);
        }

        private void Aniadir_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<SR_productos>(this, ProductoSelecionado);

            SeleccionProducto window = new SeleccionProducto();
            window.ShowDialog();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (!(DetallesCheque.SelectedItem is SR_cheqdet cheqdet)) return;

            ViewModel.EliminarProducto(cheqdet);

            DetallesCheque.Items.Refresh();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
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
                    //loading.AgregarMensaje("Registrando bitácora");
                    //ViewModel.ResgistrarBitacora();
                }

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();

                if (respuesta == TipoRespuesta.HECHO)
                {
                    MessageBox.Show("Los cambios se guardaron correctamente.", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar guardar los cambios, por favor intente de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.InicializarControles();

            //CambiarPrecios.SelectedItem = ViewModel.Condicionales.Where(x => x.Valor == ViewModel.CambiarPrecio).FirstOrDefault();

            HabilitarComponentes(false);
        }

        //private void CambiarPrecios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (!(sender is ComboBox comboBox)) return;

        //    if (!(comboBox.SelectedItem is Condicional condicional)) return;

        //    ViewModel.CambiarPrecio = condicional.Valor;
        //}

        private void Folio_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(Folio.Text))
            {
                App.HabilitarPrincipal(false);

                var respuesta = new Respuesta
                {
                    TipoRespuesta = TipoRespuesta.NADA
                };
                LoadingWindow loading = new LoadingWindow();
                loading.AgregarMensaje("Buscando cuenta");
                loading.Show();

                Task.Factory.StartNew(() =>
                {
                    respuesta = ViewModel.ObtenerChequeSR();

                }).ContinueWith(task =>
                {
                    loading.Close();
                    App.HabilitarPrincipal();

                    if (respuesta.TipoRespuesta == TipoRespuesta.HECHO)
                    {
                        if (respuesta.MultipleFormaPago)
                        {
                            MessageBox.Show("Cuenta fue cerrada con multiple forma de pago. Al ajustarse la cuenta se guardará solo con la forma de pago en efectivo.", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        HabilitarComponentes();
                    }
                    else if (respuesta.TipoRespuesta == TipoRespuesta.REGISTRO_NO_ENCONTRADO)
                    {
                        MessageBox.Show("No se encontró la cuenta", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (respuesta.TipoRespuesta == TipoRespuesta.CHEQUE_CANCELADO)
                    {
                        MessageBox.Show("Cuenta cancelada.", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                    {
                        MessageBox.Show("La cuenta no tiene detalles", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (respuesta.TipoRespuesta == TipoRespuesta.CHEQUE_SIN_FORMA_PAGO)
                    {
                        MessageBox.Show("La cuenta no tiene forma de pago.", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (respuesta.TipoRespuesta == TipoRespuesta.FECHA_INACCESIBLE)
                    {
                        MessageBox.Show($"No cuenta con la licencia para la busqueda de cuentas en el mes de {respuesta.Mensaje}", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                    {
                        MessageBox.Show("Error al intentar buscar la cuenta.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Folio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }
        
        private void DetallesCheque_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(DetallesCheque.CurrentColumn is DataGridColumn dataGridColumn)) return;

            string header = dataGridColumn.Header.ToString();

            if (header.Equals("Clave"))
            {
                if (!(DetallesCheque.CurrentItem is SR_cheqdet cheqdet)) return;

                bool modificador = cheqdet.modificador ?? false;

                if (modificador) return;

                Messenger.Default.Register<SR_productos>(this, ProductoCambio);
                SeleccionProducto window = new SeleccionProducto();
                window.ShowDialog();
            }
        }

        private void DetallesCheque_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DetallesCheque.Dispatcher.BeginInvoke(new Action(() => RefrescarControles()), DispatcherPriority.Background);
        }

        private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Button button)) return;

            if (!(button.Content is Grid grid)) return;

            if (button.IsEnabled)
                grid.Children[0].Opacity = 1d;
            else
                grid.Children[0].Opacity = 0.5d;
        }

        private string PreviousText = "";
        private int PreviousCaretIndex = 0;
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PreviousText = ((TextBox)sender).Text;
            PreviousCaretIndex = ((TextBox)sender).CaretIndex;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(((TextBox)sender).Text, @"^\d{0,15}\.?\d{0,4}$"))
            {
                ((TextBox)sender).Text = PreviousText;
                ((TextBox)sender).CaretIndex = PreviousCaretIndex;
                e.Handled = true;
            }
        }

        private void TextBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PreviousText = ((TextBox)sender).Text;
            PreviousCaretIndex = ((TextBox)sender).CaretIndex;
        }

        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(((TextBox)sender).Text, @"^\d{0,8}\.?\d{0,6}$"))
            {
                ((TextBox)sender).Text = PreviousText;
                ((TextBox)sender).CaretIndex = PreviousCaretIndex;
                e.Handled = true;
            }
        }

        private void AniadirCliente_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Register<SR_clientes>(this, ClienteSeleccionado);

            SeleccionClientes window = new SeleccionClientes();
            window.ShowDialog();
        }

        public void ClienteSeleccionado(SR_clientes cliente)
        {
            if (cliente != null)
                ViewModel.AniadirCliente(cliente);

            Messenger.Default.Unregister(this);
        }

        public void ProductoCambio(SR_productos producto)
        {
            if (producto != null)
            {
                if (!(DetallesCheque.CurrentItem is SR_cheqdet cheqdet)) return;

                ViewModel.CambiarProducto(cheqdet, producto);

                DetallesCheque.Items.Refresh();
            }

            Messenger.Default.Unregister(this);
        }

        private void ProductoSelecionado(SR_productos producto)
        {
            if (producto != null)
            {
                ViewModel.AniadirProducto(producto);

                DetallesCheque.Items.Refresh();
            }

            Messenger.Default.Unregister(this);
        }

        public void RefrescarControles()
        {
            DetallesCheque.IsReadOnly = true;
            ViewModel.AjustarCheque();
            DetallesCheque.Items.Refresh();
            DetallesCheque.IsReadOnly = false;
        }

        private void HabilitarComponentes(bool habilitar = true)
        {
            AniadirCliente.IsEnabled = habilitar;
            Aniadir.IsEnabled = habilitar;
            Eliminar.IsEnabled = habilitar;
            Aceptar.IsEnabled = habilitar;
            Cancelar.IsEnabled = habilitar;

            CambiarPrecios.IsEnabled = habilitar;
            Fecha.IsEnabled = habilitar;
            Personas.IsEnabled = habilitar;
            ClaveCliente.IsEnabled = habilitar;
            NombreCliente.IsEnabled = habilitar;
            Descuento.IsEnabled = habilitar;
            Propina.IsEnabled = habilitar;
            Subtotal.IsEnabled = habilitar;
            Total.IsEnabled = habilitar;
        }

        private async void Descuento_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            await Task.Run(() =>
            {
                ViewModel.AjustarCheque();
            });
        }

        //private void Propina_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    var r = new Regex("[0-9]").IsMatch(e.Text);
        //    e.Handled = !r;
        //}

        //private void Propina_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.OemPeriod)
        //    {
        //        bool valido = true;

        //        if (e.Key == Key.OemPeriod && Propina.Text.Length == 0) valido = false;

        //        if (e.Key == Key.OemPeriod && Propina.Text.Contains(".")) valido = false;

        //        var result = Propina.Text.Split('.');

        //        if (result.Length == 2)
        //        {
        //            valido = result[0].Length > 0 && result[1].Length < 4;
        //        }

        //        if (valido)
        //            e.Handled = false;
        //        else
        //            e.Handled = true;
        //    }
        //    else
        //        e.Handled = true;
        //}
    }
}
