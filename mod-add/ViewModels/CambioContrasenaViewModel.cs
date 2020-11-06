using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Enums;
using mod_add.Utils;

namespace mod_add.ViewModels
{
    public class CambioContrasenaViewModel
    {
        private readonly DatabaseFactory dbf;
        private readonly IConfiguracionServicio _configuracionServicio;
        public CambioContrasenaViewModel()
        {
            dbf = new DatabaseFactory();
            _configuracionServicio = new ConfiguracionServicio(dbf);

            ContrasenaActual = "";
            ContrasenaNueva = "";
        }

        public TipoRespuesta CambiarContrasena()
        {
            if (string.IsNullOrWhiteSpace(ContrasenaActual) && (ContrasenaActual.Length < 8 || ContrasenaActual.Length > 20))
                return TipoRespuesta.CONTRASENA_INCORRECTA;

            if (ContrasenaNueva.Length < 8 || ContrasenaNueva.Length > 20)
            {
                return TipoRespuesta.LONGITUD_INCORRECTA;
            }

            string contrasenaEncriptada = Encriptado.Codificar(ContrasenaActual);

            if (App.ConfiguracionSistema.Contrasena.Equals(contrasenaEncriptada))
            {
                App.ConfiguracionSistema.Contrasena = Encriptado.Codificar(ContrasenaNueva);
            }
            else if (App.Admin && App.ConfiguracionSistema.ContrasenaAdmin.Equals(contrasenaEncriptada))
            {
                App.ConfiguracionSistema.Contrasena = Encriptado.Codificar(ContrasenaNueva);
            }
            else
            {
                return TipoRespuesta.CONTRASENA_INCORRECTA;
            }

            if (_configuracionServicio.Update(App.ConfiguracionSistema) == 1)
                return TipoRespuesta.HECHO;
            else
                return TipoRespuesta.ERROR;
        }

        public string ContrasenaActual { get; set; }
        public string ContrasenaNueva { get; set; }
    }
}
