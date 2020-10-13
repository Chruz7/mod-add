using SRLibrary.Models;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.ViewModels
{
    public class SeleccionProductoViewModel : ViewModelBase
    {
        public SeleccionProductoViewModel()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_productos_DAO productos_DAO = new SR_productos_DAO(context);
                List<Joins> joins = new List<Joins>
                {
                    new Joins()
                    {
                        Modelo = "SR_productosdetalle",
                        KeyPrimary = "idproducto",
                        KeyForeing = "idproducto"
                    }
                };

                Productos = productos_DAO.join(joins, new object[] { });
            }
        }

        private List<dynamic> _Productos;
        public List<dynamic> Productos
        {
            get { return _Productos; }
            set
            {
                _Productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }
    }
}
