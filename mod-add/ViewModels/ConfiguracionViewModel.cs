using mod_add.Selectores;
using System.Collections.Generic;

namespace mod_add.ViewModels
{
    public class ConfiguracionViewModel : ViewModelBase
    {
        public ConfiguracionViewModel()
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

            ModificarVentasReales = true;
            MinProductosCuenta = 1;
            EliminarProductosSeleccionados = true;
        }

        public int Guardar()
        {
            return 1;
        }

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
    }
}
