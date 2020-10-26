using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System.Collections.Generic;
using System.Linq;

namespace mod_add.ViewModels
{
    public class SeleccionGrupoViewModel : ViewModelBase
    {
        public SeleccionGrupoViewModel()
        {
            Grupos2 = new List<SR_grupos>();
            Grupos = new List<SR_grupos>();

            ObtenerGruposSR();
            Filtrar("");
        }

        public void ObtenerGruposSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_grupos_DAO grupos_DAO = new SR_grupos_DAO(context);
                Grupos2 = grupos_DAO.GetAll();
            }
        }

        public void Filtrar(string texto)
        {
            Grupos = Grupos2.Where(x => x.descripcion.Contains(texto) || x.idgrupo.Contains(texto)).ToList();
        }

        private List<SR_grupos> Grupos2 { get; set; }

        private List<SR_grupos> grupos;
        public List<SR_grupos> Grupos
        {
            get { return grupos; }
            set
            {
                grupos = value;
                OnPropertyChanged(nameof(Grupos));
            }
        }
    }
}
