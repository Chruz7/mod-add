using mod_add.Datos.Contexto;
using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using mod_add.Selectores;
using SRLibrary.Models;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using System.Collections.Generic;
using System.Linq;

namespace mod_add.ViewModels
{
    public class ConfiguracionViewModel : ViewModelBase
    {
        private readonly DatabaseFactory dbf;
        private readonly IConfiguracionServicio _configuracionServicio;
        private readonly IProductoReemplazoServicio _productoReemplazoServicio;
        public ConfiguracionViewModel()
        {
            dbf = new DatabaseFactory();
            _configuracionServicio = new ConfiguracionServicio(dbf);
            _productoReemplazoServicio = new ProductoReemplazoServicio(dbf);

            ObtenerCondicionales();
            ObtenerConfiguracion();
            ObtenerProdutosReemplazo();
        }

        public void ObtenerCondicionales()
        {
            Condicionales = new List<Condicional>
            {
                new Condicional
                {
                    Titulo = "SI",
                    Valor = true
                },
                new Condicional
                {
                    Titulo = "No",
                    Valor = false
                }
            };
        }

        public void ObtenerConfiguracion()
        {
            ConfiguracionSistema = _configuracionServicio.ObtenerConfiguracion();

            ModificarVentasReales = ConfiguracionSistema.ModificarVentasReales;
            MinProductosCuenta = ConfiguracionSistema.MinProductosCuenta;
            EliminarProductosSeleccionados = ConfiguracionSistema.EliminarProductosSeleccionados;
        }

        public void ObtenerProdutosReemplazo()
        {
            var productos = _productoReemplazoServicio.GetAll().ToList();

            Producto1 = productos[0];

            P1_Reemplazar = Producto1.Reemplazar;
            P1_Clave = Producto1.Clave;
            P1_Porcentaje = Producto1.Porcentaje;

            if (!string.IsNullOrEmpty(Producto1.Clave))
            {
                var productoSR = ObtenerProductoSR(Producto1.Clave);

                P1_Nombre = productoSR.descripcion;
                P1_Precio = string.Format("{0:C}", productoSR.productosdetalle_precio);
            }
            else
            {
                P1_Nombre = "";
                P1_Precio = "";
            }

            Producto2 = productos[1];

            P2_Reemplazar = Producto2.Reemplazar;
            P2_Clave = Producto2.Clave;
            P2_Porcentaje = Producto2.Porcentaje;

            if (!string.IsNullOrEmpty(Producto2.Clave))
            {
                var productoSR = ObtenerProductoSR(Producto2.Clave);

                P2_Nombre = productoSR.descripcion;
                P2_Precio = string.Format("{0:C}", productoSR.productosdetalle_precio);
            }
            else
            {
                P2_Nombre = "";
                P2_Precio = "";
            }

            Producto3 = productos[2];

            P3_Reemplazar = Producto3.Reemplazar;
            P3_Clave = Producto3.Clave;
            P3_Porcentaje = Producto3.Porcentaje;

            if (!string.IsNullOrEmpty(Producto3.Clave))
            {
                var productoSR = ObtenerProductoSR(Producto3.Clave);

                P3_Nombre = productoSR.descripcion;
                P3_Precio = string.Format("{0:C}", productoSR.productosdetalle_precio);
            }
            else
            {
                P3_Nombre = "";
                P3_Precio = "";
            }

            Producto4 = productos[3];

            P4_Reemplazar = Producto4.Reemplazar;
            P4_Clave = Producto4.Clave;
            P4_Porcentaje = Producto4.Porcentaje;

            if (!string.IsNullOrEmpty(Producto4.Clave))
            {
                var productoSR = ObtenerProductoSR(Producto4.Clave);

                P4_Nombre = productoSR.descripcion;
                P4_Precio = string.Format("{0:C}", productoSR.productosdetalle_precio);
            }
            else
            {
                P4_Nombre = "";
                P4_Precio = "";
            }

            Producto5 = productos[4];

            P5_Reemplazar = Producto5.Reemplazar;
            P5_Clave = Producto5.Clave;
            P5_Porcentaje = Producto5.Porcentaje;

            if (!string.IsNullOrEmpty(Producto5.Clave))
            {
                var productoSR = ObtenerProductoSR(Producto5.Clave);

                P5_Nombre = productoSR.descripcion;
                P5_Precio = string.Format("{0:C}", productoSR.productosdetalle_precio);
            }
            else
            {
                P5_Nombre = "";
                P5_Precio = "";
            }
        }

        public dynamic ObtenerProductoSR(string idproducto)
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

                var productosSR = productos_DAO.join($"productos.idproducto = {idproducto}", joins, new object[] { });

                foreach (var productoSR in productosSR)
                {
                    return productoSR;
                }
            }

            return null;
        }

        public int Guardar()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                try
                {
                    var configuracion = context.ConfiguracionSistema.FirstOrDefault();
                    configuracion.ModificarVentasReales = ModificarVentasReales;
                    configuracion.MinProductosCuenta = MinProductosCuenta;
                    configuracion.EliminarProductosSeleccionados = EliminarProductosSeleccionados;

                    var productosReemplazo = context.ProductosReemplazo.ToList();

                    productosReemplazo[0].Reemplazar = P1_Reemplazar;
                    productosReemplazo[0].Clave = P1_Clave;
                    productosReemplazo[0].Porcentaje = P1_Porcentaje;

                    productosReemplazo[1].Reemplazar = P2_Reemplazar;
                    productosReemplazo[1].Clave = P2_Clave;
                    productosReemplazo[1].Porcentaje = P2_Porcentaje;

                    productosReemplazo[2].Reemplazar = P3_Reemplazar;
                    productosReemplazo[2].Clave = P3_Clave;
                    productosReemplazo[2].Porcentaje = P3_Porcentaje;

                    productosReemplazo[3].Reemplazar = P4_Reemplazar;
                    productosReemplazo[3].Clave = P4_Clave;
                    productosReemplazo[3].Porcentaje = P4_Porcentaje;

                    productosReemplazo[4].Reemplazar = P5_Reemplazar;
                    productosReemplazo[4].Clave = P5_Clave;
                    productosReemplazo[4].Porcentaje = P5_Porcentaje;

                    context.SaveChanges();

                    return 1;
                }
                catch
                {
                    return 0;
                }
            }
        }

        private ConfiguracionSistema ConfiguracionSistema { get; set; }

        private List<Condicional> _Condicionales;
        public List<Condicional> Condicionales
        {
            get { return _Condicionales; }
            set
            {
                _Condicionales = value;
                OnPropertyChanged(nameof(Condicionales));
            }
        }

        public bool ModificarVentasReales { get; set; }

        private int _MinProductosCuenta;
        public int MinProductosCuenta
        {
            get { return _MinProductosCuenta; }
            set
            {
                _MinProductosCuenta = value;
                OnPropertyChanged(nameof(MinProductosCuenta));
            }
        }

        private bool _EliminarProductosSeleccionados;
        public bool EliminarProductosSeleccionados
        {
            get { return _EliminarProductosSeleccionados; }
            set
            {
                _EliminarProductosSeleccionados = value;
                OnPropertyChanged(nameof(EliminarProductosSeleccionados));
            }
        }

        public ProductoReemplazo Producto1 { get; set; }
        public ProductoReemplazo Producto2 { get; set; }
        public ProductoReemplazo Producto3 { get; set; }
        public ProductoReemplazo Producto4 { get; set; }
        public ProductoReemplazo Producto5 { get; set; }



        private bool _P1_Reemplazar;
        public bool P1_Reemplazar
        {
            get { return _P1_Reemplazar; }
            set
            {
                _P1_Reemplazar = value;
                OnPropertyChanged(nameof(P1_Reemplazar));
            }
        }

        private string _P1_Clave;
        public string P1_Clave
        {
            get { return _P1_Clave; }
            set
            {
                _P1_Clave = value;
                OnPropertyChanged(nameof(P1_Clave));
            }
        }

        private string _P1_Nombre;
        public string P1_Nombre
        {
            get { return _P1_Nombre; }
            set
            {
                _P1_Nombre = value;
                OnPropertyChanged(nameof(P1_Nombre));
            }
        }

        private string _P1_Precio;
        public string P1_Precio
        {
            get { return _P1_Precio; }
            set
            {
                _P1_Precio = value;
                OnPropertyChanged(nameof(P1_Precio));
            }
        }

        private int _P1_Porcentaje;
        public int P1_Porcentaje
        {
            get { return _P1_Porcentaje; }
            set
            {
                _P1_Porcentaje = value;
                OnPropertyChanged(nameof(P1_Porcentaje));
            }
        }



        private bool _P2_Reemplazar;
        public bool P2_Reemplazar
        {
            get { return _P2_Reemplazar; }
            set
            {
                _P2_Reemplazar = value;
                OnPropertyChanged(nameof(P2_Reemplazar));
            }
        }

        private string _P2_Clave;
        public string P2_Clave
        {
            get { return _P2_Clave; }
            set
            {
                _P2_Clave = value;
                OnPropertyChanged(nameof(P2_Clave));
            }
        }

        private string _P2_Nombre;
        public string P2_Nombre
        {
            get { return _P2_Nombre; }
            set
            {
                _P2_Nombre = value;
                OnPropertyChanged(nameof(P2_Nombre));
            }
        }

        private string _P2_Precio;
        public string P2_Precio
        {
            get { return _P2_Precio; }
            set
            {
                _P2_Precio = value;
                OnPropertyChanged(nameof(P2_Precio));
            }
        }

        private int _P2_Porcentaje;
        public int P2_Porcentaje
        {
            get { return _P2_Porcentaje; }
            set
            {
                _P2_Porcentaje = value;
                OnPropertyChanged(nameof(P2_Porcentaje));
            }
        }



        private bool _P3_Reemplazar;
        public bool P3_Reemplazar
        {
            get { return _P3_Reemplazar; }
            set
            {
                _P3_Reemplazar = value;
                OnPropertyChanged(nameof(P3_Reemplazar));
            }
        }

        private string _P3_Clave;
        public string P3_Clave
        {
            get { return _P3_Clave; }
            set
            {
                _P3_Clave = value;
                OnPropertyChanged(nameof(P3_Clave));
            }
        }

        private string _P3_Nombre;
        public string P3_Nombre
        {
            get { return _P3_Nombre; }
            set
            {
                _P3_Nombre = value;
                OnPropertyChanged(nameof(P3_Nombre));
            }
        }

        private string _P3_Precio;
        public string P3_Precio
        {
            get { return _P3_Precio; }
            set
            {
                _P3_Precio = value;
                OnPropertyChanged(nameof(P3_Precio));
            }
        }

        private int _P3_Porcentaje;
        public int P3_Porcentaje
        {
            get { return _P3_Porcentaje; }
            set
            {
                _P3_Porcentaje = value;
                OnPropertyChanged(nameof(P3_Porcentaje));
            }
        }



        private bool _P4_Reemplazar;
        public bool P4_Reemplazar
        {
            get { return _P4_Reemplazar; }
            set
            {
                _P4_Reemplazar = value;
                OnPropertyChanged(nameof(P4_Reemplazar));
            }
        }

        private string _P4_Clave;
        public string P4_Clave
        {
            get { return _P4_Clave; }
            set
            {
                _P4_Clave = value;
                OnPropertyChanged(nameof(P3_Clave));
            }
        }

        private string _P4_Nombre;
        public string P4_Nombre
        {
            get { return _P4_Nombre; }
            set
            {
                _P4_Nombre = value;
                OnPropertyChanged(nameof(P4_Nombre));
            }
        }

        private string _P4_Precio;
        public string P4_Precio
        {
            get { return _P4_Precio; }
            set
            {
                _P4_Precio = value;
                OnPropertyChanged(nameof(P4_Precio));
            }
        }

        private int _P4_Porcentaje;
        public int P4_Porcentaje
        {
            get { return _P4_Porcentaje; }
            set
            {
                _P4_Porcentaje = value;
                OnPropertyChanged(nameof(P4_Porcentaje));
            }
        }



        private bool _P5_Reemplazar;
        public bool P5_Reemplazar
        {
            get { return _P5_Reemplazar; }
            set
            {
                _P5_Reemplazar = value;
                OnPropertyChanged(nameof(P5_Reemplazar));
            }
        }

        private string _P5_Clave;
        public string P5_Clave
        {
            get { return _P5_Clave; }
            set
            {
                _P5_Clave = value;
                OnPropertyChanged(nameof(P5_Clave));
            }
        }

        private string _P5_Nombre;
        public string P5_Nombre
        {
            get { return _P5_Nombre; }
            set
            {
                _P5_Nombre = value;
                OnPropertyChanged(nameof(P5_Nombre));
            }
        }

        private string _P5_Precio;
        public string P5_Precio
        {
            get { return _P5_Precio; }
            set
            {
                _P5_Precio = value;
                OnPropertyChanged(nameof(P5_Precio));
            }
        }

        private int _P5_Porcentaje;
        public int P5_Porcentaje
        {
            get { return _P5_Porcentaje; }
            set
            {
                _P5_Porcentaje = value;
                OnPropertyChanged(nameof(P5_Porcentaje));
            }
        }
    }
}
