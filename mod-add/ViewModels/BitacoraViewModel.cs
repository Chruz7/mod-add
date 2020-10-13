using mod_add.Datos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.ViewModels
{
    public class BitacoraViewModel : ViewModelBase
    {
        public BitacoraViewModel()
        {

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
