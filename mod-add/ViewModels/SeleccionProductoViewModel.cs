using mod_add.Datos.ModelosPersonalizados;
using SRLibrary.Models;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace mod_add.ViewModels
{
    public class SeleccionProductoViewModel : ViewModelBase
    {
        public SeleccionProductoViewModel()
        {
            ProductosAlmacen = new List<ProductoSeleccion>();
            Productos = new ObservableCollection<ProductoSeleccion>();
            Buscador = "";

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

                var productos = productos_DAO.join(joins, new object[] { });

                foreach (var producto in productos)
                {
                    ProductosAlmacen.Add(new ProductoSeleccion
                    {
                        Clave = producto.idproducto,
                        Grupo = producto.idgrupo,
                        Descripcion = producto.descripcion,
                        Precio = producto.productosdetalle_precio
                    });
                }

                ProductosAlmacen = ProductosAlmacen.OrderBy(x => x.Precio).ToList();

                Filtrar("");
            }
        }

        public void Filtrar(string Buscador)
        {
            Productos = new ObservableCollection<ProductoSeleccion>(ProductosAlmacen.Where(x => x.Descripcion.Contains(Buscador) || x.Clave.Contains(Buscador) || x.Display_Precio.Contains(Buscador)).ToList());
        }

        public List<ProductoSeleccion> ProductosAlmacen { get; set; }

        private ObservableCollection<ProductoSeleccion> _Productos;
        public ObservableCollection<ProductoSeleccion> Productos
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
