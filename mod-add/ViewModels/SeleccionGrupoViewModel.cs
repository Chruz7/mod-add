using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.ViewModels
{
    public class SeleccionGrupoViewModel : ViewModelBase
    {
        public SeleccionGrupoViewModel()
        {
            GruposAlmacen = new List<SR_grupos>();
            Grupos = new List<SR_grupos>();

            ObtenerGruposSR();
            Filtrar("");
        }

        public void ObtenerGruposSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_grupos_DAO grupos_DAO = new SR_grupos_DAO(context);
                GruposAlmacen = grupos_DAO.GetAll();
            }
        }

        public void Filtrar(string texto)
        {
            Grupos = GruposAlmacen.Where(x => x.descripcion.Contains(texto) || x.idgrupo.Contains(texto)).ToList();
        }

        private List<SR_grupos> GruposAlmacen { get; set; }

        private List<SR_grupos> _Grupos;
        public List<SR_grupos> Grupos
        {
            get { return _Grupos; }
            set
            {
                _Grupos = value;
                OnPropertyChanged(nameof(Grupos));
            }
        }
    }
}
