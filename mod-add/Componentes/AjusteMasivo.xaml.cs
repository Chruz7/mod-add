﻿using mod_add.Datos.Enums;
using mod_add.Enums;
using mod_add.Selectores;
using mod_add.ViewModels;
using mod_add.Vistas;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mod_add.Componentes
{
    /// <summary>
    /// Lógica de interacción para AjusteMasivo.xaml
    /// </summary>
    public partial class AjusteMasivo : UserControl
    {
        private readonly AjusteMasivoViewModel ViewModel;
        public AjusteMasivo()
        {
            InitializeComponent();

            HabilitarControles(false);
            Aplicar.IsEnabled = false;
            Cancelar.IsEnabled = false;
                        
            //FechaInicio.DisplayDateEnd = DateTime.Today.AddDays(-1);
            //FechaCierre.DisplayDateEnd = DateTime.Today.AddDays(-1);

            //Procesos.IsEnabled = false;

            ViewModel = new AjusteMasivoViewModel();
            DataContext = ViewModel;

            //if (ViewModel.Turno)
            //{
            //    FechaInicio.DisplayDateEnd = DateTime.Today.AddDays(-1);
            //}
            //else if (ViewModel.Periodo)
            //{
            //    FechaInicio.DisplayDateEnd = DateTime.Today;
            //}

            FechaCierre.DisplayDateEnd = DateTime.Today;
        }

        private void NuevaBusqueda_Click(object sender, RoutedEventArgs e)
        {
            HabilitarControles();
            ViewModel.InicializarControles();
        }

        private void GenerarVistaPrevia_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            App.HabilitarPrincipal(false);

            Respuesta respuesta = new Respuesta
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Buscando cuentas");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.ObtenerChequesSR();

                if (respuesta.TipoRespuesta == TipoRespuesta.HECHO)
                {
                    MessageBoxResult result = MessageBoxResult.Yes;
                    if (respuesta.RegistrosProcesados)
                    {
                        result = MessageBox.Show($"Ya se han procesado uno o varios registros de la busqueda. ¿Desea reprocesarlos?", "Busqueda", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }

                    if (result == MessageBoxResult.Yes)
                    {
                        loading.AgregarMensaje("Creando registros temporales");
                        ViewModel.CrearRegistrosTemporales(respuesta);

                        loading.AgregarMensaje("Procesando información");
                        ViewModel.GenerarVistaPrevia();
                    }
                    else
                    {
                        respuesta.TipoRespuesta = TipoRespuesta.NADA;
                    }
                }

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();

                if (respuesta.TipoRespuesta == TipoRespuesta.HECHO)
                {
                    ViewModel.CargarResultados();
                    DetalleModificacionCheques.Items.Refresh();
                    HabilitarControles(false);
                    NuevaBusqueda.IsEnabled = false;
                    Aplicar.IsEnabled = true;
                    Cancelar.IsEnabled = true;
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.FECHA_INACCESIBLE)
                {
                    MessageBox.Show($"No cuenta con la licencia para la busqueda de cuentas en el mes de {respuesta.Mensaje}", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("No se encontraron cuentas", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar buscar las cuentas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Aplicar_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.DetalleModificacionCheques.Where(x => x.RealizarAccion).Count() == 0)
            {
                MessageBox.Show("No hay cambios", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

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
                    NuevaBusqueda.IsEnabled = true;
                    Cancelar.IsEnabled = false;
                    Aplicar.IsEnabled = false;
                    MessageBox.Show("Se guardaron los cambios correctamente", "Listo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (respuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar guardar los cambios, por favor intente de nuevo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.InicializarControles();
            GenerarVistaPrevia.IsEnabled = true;
            Aplicar.IsEnabled = false;
            Cancelar.IsEnabled = false;
        }

        private void Turno_Checked(object sender, RoutedEventArgs e)
        {
            Lbl_FechaCierre.Visibility = Visibility.Collapsed;
            FechaCierre.Visibility = Visibility.Collapsed;
            HoraInicio.Visibility = Visibility.Collapsed;
            HoraCierre.Visibility = Visibility.Collapsed;

            HorarioTurno.Visibility = Visibility.Visible;
            ViewModel.AjustarFechaCierre();
            ViewModel.InicializarHorarioTurno();

            FechaInicio.DisplayDateEnd = DateTime.Today.AddDays(-1);
        }

        private void Turno_Unchecked(object sender, RoutedEventArgs e)
        {
            Lbl_FechaCierre.Visibility = Visibility.Visible;
            FechaCierre.Visibility = Visibility.Visible;
            HoraInicio.Visibility = Visibility.Visible;
            HoraCierre.Visibility = Visibility.Visible;

            HorarioTurno.Visibility = Visibility.Collapsed;

            FechaInicio.DisplayDateEnd = DateTime.Today;

            //Lbl_FechaCierre.Visibility = Visibility.Visible;
            //FechaCierre.Visibility = Visibility.Visible;
            //HorarioTurno.Visibility = Visibility.Hidden;

            //FechaCierre.DisplayDateStart = ViewModel.FechaInicio;
            //FechaCierre.DisplayDateEnd = DateTime.Today.AddDays(-1);
            //ViewModel.FechaCierre = DateTime.Today.AddDays(-1);
        }

        private void HabilitarControles(bool habilitar = true)
        {
            Turno.IsEnabled = habilitar;
            Periodo.IsEnabled = habilitar;
            FechaInicio.IsEnabled = habilitar;
            FechaCierre.IsEnabled = habilitar;
            Procesos.IsEnabled = habilitar;
            ImporteMinimoAjustable.IsEnabled = habilitar;
            PorcentajeObjetivo.IsEnabled = habilitar;
            ImporteObjetivo.IsEnabled = habilitar;
            CuentaPagadaTarjerta.IsEnabled = habilitar;
            CuentaPagadaVales.IsEnabled = habilitar;
            CuentaPagadaOtros.IsEnabled = habilitar;
            CuentaFacturada.IsEnabled = habilitar;
            CuentaNotaConsumo.IsEnabled = habilitar;
            QuitarPropinasManualmente.IsEnabled = habilitar;
            NoIncluirCuentasReimpresas.IsEnabled = habilitar;
            GenerarVistaPrevia.IsEnabled = habilitar;
        }

        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void FechaInicio_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.Turno)
            {
                ViewModel.AjustarFechaCierre();
            }
            FechaCierre.DisplayDateStart = ViewModel.FechaInicio;
            //FechaCierre.DisplayDateStart = ViewModel.FechaInicio;
        }

        private void FechaCierre_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.Periodo)
            {
                FechaInicio.DisplayDateEnd = ViewModel.FechaCierre;
            }
            //FechaInicio.DisplayDateEnd = ViewModel.FechaCierre;
        }

        private void PorcentajeObjetivo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
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

        private void Procesos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (!(comboBox.SelectedItem is Proceso proceso)) return;
            //ViewModel.Proceso = proceso;
            if (proceso.TipoProceso == TipoProceso.FOLIOS)
                DetalleModificacionCheques.Columns[13].Header = "Eliminar";
            else if (proceso.TipoProceso == TipoProceso.PRODUCTOS)
                DetalleModificacionCheques.Columns[13].Header = "Modificar";
        }

        public bool Validar()
        {
            var corteInicio = ViewModel.FechaInicio.AddSeconds(ViewModel.HoraInicio.TimeOfDay.TotalSeconds);
            var corteCierre = ViewModel.FechaCierre.AddSeconds(ViewModel.HoraCierre.TimeOfDay.TotalSeconds);

            if (corteInicio > corteCierre)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha de cierre", "Fecha inicio", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
