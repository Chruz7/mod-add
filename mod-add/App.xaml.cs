using mod_add.Datos.Contexto;
using mod_add.Datos.Modelos;
using mod_add.Vistas;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private void Application_Startup(object sender, StartupEventArgs e)
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
                        Contrasena = "",
                        ContrasenaAdmin = ""
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
                ProductosReemplazo = context.ProductosReemplazo.ToList();
                ProductosEliminar = context.ProductosEliminar.ToList();
            }

            Autenticacion autenticacion = new Autenticacion();
            autenticacion.ShowDialog();

            //IrPrincipal();
        }

        public static void IrPrincipal()
        {
            Principal principal = new Principal();
            principal.Show();
        }
    }
}
