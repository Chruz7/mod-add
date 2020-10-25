using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using mod_add.Enums;
using mod_add.Utils;
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

            ObtenerBitacora();
        }

        public void ObtenerBitacora()
        {
            BitacoraModificaciones = _bitacoraServicio.GetAll().ToList();
        }

        public TipoRespuesta ExportarExcel()
        {
            var respuesta = Exportar.Excel(BitacoraModificaciones);

            return respuesta;
        }

        private List<BitacoraModificacion> _BitacoraModificaciones;
        public List<BitacoraModificacion> BitacoraModificaciones
        {
            get { return _BitacoraModificaciones; }
            set
            {
                _BitacoraModificaciones = value;
                OnPropertyChanged(nameof(BitacoraModificaciones));
            }
        }
    }
}
