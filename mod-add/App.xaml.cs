using mod_add.Datos.Contexto;
using mod_add.Datos.Modelos;
using mod_add.Utils;
using mod_add.Vistas;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public static string PathEjecucion { get; set; }
        public static string PathBitacoraFiscal { get; set; }
        public static string PathDetalladoFormasPago { get; set; }
        public static string PathDetalladoHorizontal { get; set; }
        public static string PathDetalladoVertical { get; set; }
        public static string PathReimpresionFolios { get; set; }
        public static string PathResumido { get; set; }

        public static bool Admin { get; set; }
        public static string ClaveEmpresa { get; set; }
        public static string ClavePagoEfectivo { get; set; }
        public static List<DateTime> MesesValidos { get; set; }
        public static ConfiguracionSistema ConfiguracionSistema { get; set; }
        public static List<ProductoReemplazo> ProductosReemplazo { get; set; }
        public static List<ProductoEliminacion> ProductosEliminar { get; set; }
        public static MidpointRounding MidpointRounding { get; set; }
        private static Principal Principal { get; set; }
        public static SR_configuracion SRConfiguracion { get; set; }
        public static SR_formasdepago SRformadepago { get; set; }
        public static List<SR_productosdetalle> SRProductosDetalle { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(".\\debug.log"));
            Debug.AutoFlush = true;

            bool sinErrores = true;
            Autenticacion autenticacion = new Autenticacion();

            Splash splash = new Splash();
            splash.Show();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    PathEjecucion = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    string pathTemp = Path.Combine(PathEjecucion, "temp");
                    PathBitacoraFiscal = Path.Combine(pathTemp, "bitacora-fiscal");
                    PathDetalladoFormasPago = Path.Combine(pathTemp, "detallado-formas-de-pago");
                    PathDetalladoHorizontal = Path.Combine(pathTemp, "detallado-horizontal");
                    PathDetalladoVertical = Path.Combine(pathTemp, "detallado-vertical");
                    PathReimpresionFolios = Path.Combine(pathTemp, "reimpresion-folios");
                    PathResumido = Path.Combine(pathTemp, "resumido");

                    if (!Directory.Exists(pathTemp))
                    {
                        Directory.CreateDirectory(pathTemp);
                        Directory.CreateDirectory(PathBitacoraFiscal);
                        Directory.CreateDirectory(PathDetalladoFormasPago);
                        Directory.CreateDirectory(PathDetalladoHorizontal);
                        Directory.CreateDirectory(PathDetalladoVertical);
                        Directory.CreateDirectory(PathReimpresionFolios);
                        Directory.CreateDirectory(PathResumido);
                    }
                    
                    Admin = false;
                    ClaveEmpresa = ConfiguracionLocalServicio.ReadSetting("CLAVE-EMPRESA");
                    ClavePagoEfectivo = ConfiguracionLocalServicio.ReadSetting("CLAVE-PAGO-EFECTIVO");
                    MidpointRounding = MidpointRounding.AwayFromZero;
                    MesesValidos = new List<DateTime>();

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        //if (context.ConfiguracionSistema.Count() == 0)
                        //{
                        //    context.ConfiguracionSistema.Add(new ConfiguracionSistema
                        //    {
                        //        ModificarVentasReales = false,
                        //        MinProductosCuenta = 1,
                        //        EliminarProductosSeleccionados = false,
                        //        Contrasena = Encriptado.Codificar("Ok123456"),
                        //        ContrasenaAdmin = Encriptado.Codificar("Ok123456")
                        //    });

                        //    List<ProductoReemplazo> productosReemplazo = new List<ProductoReemplazo>();

                        //    for (int i = 0; i < 5; i++)
                        //    {
                        //        productosReemplazo.Add(new ProductoReemplazo
                        //        {
                        //            Reemplazar = false,
                        //            Clave = "",
                        //            Porcentaje = 0
                        //        });
                        //    }

                        //    context.ProductosReemplazo.AddRange(productosReemplazo);

                        //    SRLibrary.Utils.valideSerialKey valideSerialKey = new SRLibrary.Utils.valideSerialKey();

                        //    context.RegistroLicencias.Add(new RegistroLicencia
                        //    {
                        //        Anio = 2020,
                        //        Mes = 9,
                        //        Licencia = valideSerialKey.getEncryptSerial("2020-09-01")
                        //    });

                        //    context.RegistroLicencias.Add(new RegistroLicencia
                        //    {
                        //        Anio = 2020,
                        //        Mes = 10,
                        //        Licencia = valideSerialKey.getEncryptSerial("2020-10-01")
                        //    });

                        //    context.RegistroLicencias.Add(new RegistroLicencia
                        //    {
                        //        Anio = 2020,
                        //        Mes = 11,
                        //        Licencia = valideSerialKey.getEncryptSerial("2020-11-01")
                        //    });


                        //    context.SaveChanges();
                        //}

                        ConfiguracionSistema = context.ConfiguracionSistema.FirstOrDefault();
                        ProductosReemplazo = context.ProductosReemplazo.Where(x => x.Reemplazar).ToList();
                        ProductosEliminar = context.ProductosEliminar.Where(x => x.Eliminar).ToList();
                    }
                    ObtenerLicencias();
                    ObtenerConfiguracionSR();
                    ObtenerProductosDetalleSR();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    sinErrores = false;
                }

            }).ContinueWith(task =>
            {
                //IrPrincipal(); //comentar
                splash.Close();

                if (sinErrores)
                    autenticacion.Show();
                else
                    autenticacion.Close();
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static void ObtenerLicencias()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var registroLicencias = context.RegistroLicencias.OrderBy(x => x.Anio).ThenBy(x => x.Mes);

                SRLibrary.Utils.valideSerialKey valideSerialKey = new SRLibrary.Utils.valideSerialKey();

                foreach (var registroLicencia in registroLicencias)
                {
                    var result = valideSerialKey.getDecryptSerial(registroLicencia.Licencia);
                    var fecha = DateTime.Parse(result);
                    MesesValidos.Add(fecha);
                }
            }
        }

        public static void IrPrincipal()
        {
            Principal = new Principal();
            if (Admin) Principal.Title = "Principal - ADMIN";
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

                    SR_formasdepago_DAO formasdepago_DAO = new SR_formasdepago_DAO(context);
                    SRformadepago = formasdepago_DAO.Find(ClavePagoEfectivo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        public static void ObtenerProductosDetalleSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_productosdetalle_DAO productosdetalle_DAO = new SR_productosdetalle_DAO(context);
                    SRProductosDetalle = productosdetalle_DAO.WhereIn("idproducto", ProductosReemplazo.Select(x => (object)x.Clave).ToArray());
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
