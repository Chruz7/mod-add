﻿using mod_add.Enums;
using mod_add.Selectores;
using mod_add.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace mod_add.Vistas
{
    /// <summary>
    /// Lógica de interacción para CorteZForm.xaml
    /// </summary>
    public partial class CorteZForm : Window
    {
        private readonly CorteZViewModel ViewModel;
        public CorteZForm()
        {
            InitializeComponent();

            Fecha.DisplayDateEnd = DateTime.Today.AddDays(-1);
            Reportes.IsEnabled = false;

            ViewModel = new CorteZViewModel();
            DataContext = ViewModel;
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            Generar(TipoDestino.IMPRESION);
        }

        private void ExportarTXT_Click(object sender, RoutedEventArgs e)
        {
            Generar(TipoDestino.EXPORTAR_TXT);
        }

        private void ExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            Generar(TipoDestino.EXPORTAR_EXCEL);
        }

        private void Generar(TipoDestino tipoDestino)
        {
            App.HabilitarPrincipal(false);

            Respuesta respuesta = new Respuesta
            {
                TipoRespuesta = TipoRespuesta.NADA
            };

            LoadingWindow loading = new LoadingWindow();
            loading.AgregarMensaje("Generando reporte");
            loading.Show();

            Task.Factory.StartNew(() =>
            {
                respuesta = ViewModel.Generar(tipoDestino);

            }).ContinueWith(task =>
            {
                loading.Close();
                App.HabilitarPrincipal();

                if (respuesta.TipoRespuesta == TipoRespuesta.FECHA_INACCESIBLE)
                {
                    MessageBox.Show($"No cuenta con la licencia para generar reportes en el mes de {respuesta.Mensaje}", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.SIN_REGISTROS)
                {
                    MessageBox.Show("No hay información para generar el reporte", "Busqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (respuesta.TipoRespuesta == TipoRespuesta.ERROR)
                {
                    MessageBox.Show("Hubo un error al intentar generar el reporte", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
