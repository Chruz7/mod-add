using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mod_add.ViewModels
{
    public class SeleccionProductoViewModel : ViewModelBase
    {
        public SeleccionProductoViewModel()
        {
            Grupos = new List<SR_grupos>();
            ProductosAlmacen = new List<SR_productos>();
            Productos = new List<SR_productos>();
            Buscador = "";

            ObtenerGruposSR();
        }

        public void ObtenerGruposSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_grupos_DAO grupos_DAO = new SR_grupos_DAO(context);

                Grupos = grupos_DAO.GetAll();
            }
        }

        public void ObtenerProductosSR(List<SR_productos> productos)
        {
            try
            {
                ProductosAlmacen = productos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //ProductosAlmacen = ProductosAlmacen.OrderBy(x => x.Detalle.precio).ToList();
            Productos = ProductosAlmacen;

            //Filtrar("");
        }

        public void Filtrar(string Buscador)
        {
            //Productos = ProductosAlmacen.Where(x => x.descripcion.Contains(Buscador) || x.idproducto.Contains(Buscador) || x.Detalle.Display_precio.Contains(Buscador)).ToList();
            Productos = ProductosAlmacen.Where(x => x.descripcion.Contains(Buscador) || x.idproducto.Contains(Buscador)).ToList();
        }

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

        public List<SR_productos> ProductosAlmacen { get; set; }

        private List<SR_productos> _Productos;
        public List<SR_productos> Productos
        {
            get { return _Productos; }
            set
            {
                _Productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        private string _Buscador;
        public string Buscador
        {
            get { return _Buscador; }
            set
            {
                _Buscador = value;
                OnPropertyChanged(nameof(Buscador));
            }
        }
    }
}
