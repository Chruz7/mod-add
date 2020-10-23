using mod_add.Datos.Contexto;
using mod_add.Datos.Modelos;
using mod_add.Vistas;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace mod_add
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool Admin { get; set; }
        public static ConfiguracionSistema ConfiguracionSistema { get; set; }
        public static List<ProductoReemplazo> ProductosReemplazo { get; set; }
        public static List<ProductoEliminar> ProductosEliminar { get; set; }
        public static MidpointRounding MidpointRounding { get; set; }
        private static Principal Principal { get; set; }
        public static SR_configuracion SRConfiguracion { get; set; }
        public static List<SR_productosdetalle> SRDetProductosReemplazo { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(".\\debug.log"));
            Debug.AutoFlush = true;

            Autenticacion autenticacion = new Autenticacion();

            Splash splash = new Splash();
            splash.Show();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Admin = false;
                    MidpointRounding = MidpointRounding.AwayFromZero;

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        if (context.ConfiguracionSistema.Count() == 0)
                        {
                            context.ConfiguracionSistema.Add(new ConfiguracionSistema
                            {
                                ModificarVentasReales = true,
                                MinProductosCuenta = 1,
                                EliminarProductosSeleccionados = false,
                                Contrasena = "Ok123456",
                                ContrasenaAdmin = "Ok123456"
                            });

                            List<ProductoReemplazo> productosReemplazo = new List<ProductoReemplazo>();

                            for (int i = 0; i < 5; i++)
                            {
                                productosReemplazo.Add(new ProductoReemplazo
                                {
                                    Reemplazar = false,
                                    Clave = "",
                                    Porcentaje = 0
                                });
                            }

                            context.ProductosReemplazo.AddRange(productosReemplazo);

                            context.SaveChanges();
                        }

                        ConfiguracionSistema = context.ConfiguracionSistema.FirstOrDefault();
                        ProductosReemplazo = context.ProductosReemplazo.Where(x => x.Reemplazar).ToList();
                        ProductosEliminar = context.ProductosEliminar.Where(x => x.Eliminar).ToList();
                    }

                    ObtenerConfiguracionSR();
                    ObtenerDetalleProductosReemplazoSR();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }

            }).ContinueWith(task =>
            {
                splash.Close();
                autenticacion.Show();
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            //IrPrincipal();
        }

        public static void IrPrincipal()
        {
            Principal = new Principal();
            Principal.Show();
        }

        public static void HabilitarPrincipal(bool habilitar = true)
        {
            Principal.IsEnabled = habilitar;
        }

        public static void ObtenerConfiguracionSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_configuracion_DAO configuracion_DAO = new SR_configuracion_DAO(context);
                    SRConfiguracion = configuracion_DAO.GetAll().FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public static void ObtenerDetalleProductosReemplazoSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_productosdetalle_DAO productosdetalle_DAO = new SR_productosdetalle_DAO(context);
                    SRDetProductosReemplazo = productosdetalle_DAO.WhereIn("idproducto", ProductosReemplazo.Select(x => (object)x.Clave).ToArray());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Debug.Flush();
            Debug.Close();
        }
    }
}
