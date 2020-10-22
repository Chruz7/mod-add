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

        public Respuesta Autenticar()
        {
            if (string.IsNullOrWhiteSpace(Contrasena) && Contrasena.Length < 8)
                return Respuesta.CONTRASENA_INCORRECTA;

            string contrasenaEncriptada = Encriptado.Contrasena(Contrasena);

            if (App.ConfiguracionSistema.Contrasena.Equals(contrasenaEncriptada))
            {
                return Respuesta.HECHO;
            }
            else if (App.ConfiguracionSistema.ContrasenaAdmin.Equals(contrasenaEncriptada))
            {
                App.Admin = true;
                return Respuesta.HECHO;
            }

            return Respuesta.CONTRASENA_INCORRECTA;
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
