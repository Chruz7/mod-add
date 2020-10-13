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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Autenticacion autenticacion = new Autenticacion();
            //autenticacion.ShowDialog();

            IrPrincipal();
        }

        public static void IrPrincipal()
        {
            Principal principal = new Principal();
            principal.Show();
        }
    }
}
