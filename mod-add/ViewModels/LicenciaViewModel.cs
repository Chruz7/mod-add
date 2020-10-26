using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Enums;

namespace mod_add.ViewModels
{
    public class LicenciaViewModel : ViewModelBase
    {
        private readonly DatabaseFactory dbf;
        private readonly IConfiguracionServicio configuracionServicio;

        public LicenciaViewModel()
        {
            dbf = new DatabaseFactory();
            configuracionServicio = new ConfiguracionServicio(dbf);
            Licencia = App.ConfiguracionSistema.Licencia;
        }

        public TipoRespuesta Guardar()
        {
            //FALTA VALIDAR
            App.ConfiguracionSistema.Licencia = Licencia;

            int result = configuracionServicio.Update(App.ConfiguracionSistema);

            return result == 1 ? TipoRespuesta.HECHO : TipoRespuesta.ERROR;
        }


        private string licencia;
        public string Licencia
        {
            get { return licencia; }
            set
            {
                licencia = value;
                OnPropertyChanged(nameof(Licencia));
            }
        }
    }
}
