using mod_add.Selectores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.ViewModels
{
    public class AjusteUnicoViewModel : ViewModelBase
    {
        public AjusteUnicoViewModel()
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
                    Titulo = "No (Usar el precio del producto)",
                    Valor = false
                }
            };

            Folio = "ddedd";
            Fecha = DateTime.Now;
            Personas = 3;
            Cliente = "Pedro";
            CambiarPrecio = false;
            Descuento = 5;
            Propina = string.Format("{0:C}", 10);
            Subtotal = string.Format("{0:C}", 30);
            Total = string.Format("{0:C}", 20);
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

        public bool CambiarPrecio { get; set; }

        private string _Folio;
        public string Folio
        {
            get { return _Folio; }
            set
            {
                _Folio = value;
                OnPropertyChanged(nameof(Folio));
            }
        }

        private DateTime _Fecha;
        public DateTime Fecha
        {
            get { return _Fecha; }
            set
            {
                _Fecha = value;
                OnPropertyChanged(nameof(Fecha));
            }
        }

        private int _Personas;
        public int Personas
        {
            get { return _Personas; }
            set
            {
                _Personas = value;
                OnPropertyChanged(nameof(Personas));
            }
        }

        private string _Cliente;
        public string Cliente
        {
            get { return _Cliente; }
            set
            {
                _Cliente = value;
                OnPropertyChanged(nameof(Cliente));
            }
        }

        private int _Descuento;
        public int Descuento
        {
            get { return _Descuento; }
            set
            {
                _Descuento = value;
                OnPropertyChanged(nameof(Descuento));
            }
        }

        private string _Propina;
        public string Propina
        {
            get { return _Propina; }
            set
            {
                _Propina = value;
                OnPropertyChanged(nameof(Propina));
            }
        }

        private string _Subtotal;
        public string Subtotal
        {
            get { return _Subtotal; }
            set
            {
                _Subtotal = value;
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        private string _Total;
        public string Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
                OnPropertyChanged(nameof(Total));
            }
        }
    }
}
