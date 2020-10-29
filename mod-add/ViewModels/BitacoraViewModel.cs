using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using mod_add.Enums;
using mod_add.Utils;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System.Collections.Generic;
using System.Linq;

namespace mod_add.ViewModels
{
    public class BitacoraViewModel : ViewModelBase
    {
        //private readonly DatabaseFactory dbf;
        //private readonly IBitacoraServicio _bitacoraServicio;

        public BitacoraViewModel()
        {
            //dbf = new DatabaseFactory();
            //_bitacoraServicio = new BitacoraServicio(dbf);

            BitacoraModificaciones = new List<SR_bitacorafiscal>();
            ObtenerBitacora();
        }

        public void ObtenerBitacora()
        {
            //BitacoraModificaciones = _bitacoraServicio.GetAll().ToList();
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_bitacorafiscal_DAO bitacorafiscal_DAO = new SR_bitacorafiscal_DAO(context);

                BitacoraModificaciones = bitacorafiscal_DAO.GetAll();
            }
        }

        public TipoRespuesta ExportarExcel()
        {
            var respuesta = Exportar.Excel(BitacoraModificaciones);

            return respuesta;
        }

        //private List<BitacoraModificacion> _BitacoraModificaciones;
        //public List<BitacoraModificacion> BitacoraModificaciones
        //{
        //    get { return _BitacoraModificaciones; }
        //    set
        //    {
        //        _BitacoraModificaciones = value;
        //        OnPropertyChanged(nameof(BitacoraModificaciones));
        //    }
        //}

        private List<SR_bitacorafiscal> _BitacoraModificaciones;
        public List<SR_bitacorafiscal> BitacoraModificaciones
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
