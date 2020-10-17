using mod_add.Datos.Contexto;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Modelos;
using mod_add.Vistas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public static MidpointRounding MidpointRounding { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MidpointRounding = MidpointRounding.AwayFromZero;
            //Autenticacion autenticacion = new Autenticacion();
            //autenticacion.ShowDialog();

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

            }

            IrPrincipal();
        }

        public static void IrPrincipal()
        {
            Principal principal = new Principal();
            principal.Show();
        }
    }
}
