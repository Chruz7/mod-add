using mod_add.Enums;
using mod_add.Utils;

namespace mod_add.ViewModels
{
    public class AutenticacionViewModel : ViewModelBase
    {
        public AutenticacionViewModel()
        {
            Contrasena = "";
        }

        public TipoRespuesta Autenticar()
        {
            if (string.IsNullOrWhiteSpace(Contrasena) && Contrasena.Length < 8)
                return TipoRespuesta.CONTRASENA_INCORRECTA;

            string contrasenaEncriptada = Encriptado.Contrasena(Contrasena);

            if (App.ConfiguracionSistema.Contrasena.Equals(contrasenaEncriptada))
            {
                return TipoRespuesta.HECHO;
            }
            else if (App.ConfiguracionSistema.ContrasenaAdmin.Equals(contrasenaEncriptada))
            {
                App.Admin = true;
                return TipoRespuesta.HECHO;
            }

            return TipoRespuesta.CONTRASENA_INCORRECTA;
        }

        private string _Contrasena;
        public string Contrasena
        {
            get { return _Contrasena; }
            set
            {
                _Contrasena = value;
                OnPropertyChanged(nameof(Contrasena));
            }
        }
    }
}
