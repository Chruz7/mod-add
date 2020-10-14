using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System.Collections.Generic;
using System.Linq;

namespace mod_add.ViewModels
{
    public class BitacoraViewModel : ViewModelBase
    {
        private readonly DatabaseFactory dbf;
        private readonly IBitacoraServicio _bitacoraServicio;

        public BitacoraViewModel()
        {
            dbf = new DatabaseFactory();
            _bitacoraServicio = new BitacoraServicio(dbf);
        }

        public void ObtenerBitacoras()
        {
            RegistrosBitacora = _bitacoraServicio.GetAll().ToList();
        }

        private List<RegistroBitacora> _RegistrosBitacora;
        public List<RegistroBitacora> RegistrosBitacora
        {
            get { return _RegistrosBitacora; }
            set
            {
                _RegistrosBitacora = value;
                OnPropertyChanged(nameof(RegistroBitacora));
            }
        }
    }
}
