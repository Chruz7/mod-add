using mod_add.Selectores;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

            Folio = "";
            Personas = 0;
            Cliente = "";
            CambiarPrecio = false;
            Descuento = 0;
            Propina = 0;
            Subtotal = string.Format("{0:C}", 0);
            Total = string.Format("{0:C}", 0);

            DetallesCheque = new ObservableCollection<SR_cheqdet>();
        }

        public int Guardar()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    Cheque.nopersonas = Personas;
                    Cheque.idcliente = Cliente;
                    Cheque.propina = Propina;

                    return 1;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public void Aniadir(SR_productos producto)
        {
            int ultimoMovimiento = (int)DetallesCheque.Max(x => x.movimiento);

            decimal precio = producto.Detalle.precio ?? 0;
            decimal impuesto1 = producto.Detalle.impuesto1 ?? 0;
            decimal impuesto2 = producto.Detalle.impuesto2 ?? 0;
            decimal impuesto3 = producto.Detalle.impuesto3 ?? 0;

            decimal preciosinumpuestos = precio - (precio * (impuesto1 * 0.01m) + precio * (impuesto2 * 0.01m) + precio * (impuesto3 * 0.01m));

            DetallesCheque.Add(new SR_cheqdet
            {
                foliodet = Cheque.folio,
                movimiento = ultimoMovimiento + 1,
                comanda = UltimoCheqDet.comanda,
                cantidad = 1,
                idproducto = producto.idproducto,
                descuento = 0,
                precio = precio,
                impuesto1 = impuesto1,
                impuesto2 = impuesto2,
                impuesto3 = impuesto3,
                preciosinimpuestos = preciosinumpuestos,
                tiempo = UltimoCheqDet.tiempo,
                hora = UltimoCheqDet.hora,
                modificador = false,
                mitad = 0,
                comentario = "",
                idestacion = UltimoCheqDet.idestacion,
                usuariodescuento = "",
                comentariodescuento = "",
                idtipodescuento = "",
                horaproduccion = null,
                idproductocompuesto = "",
                productocompuestoprincipal = false,
                preciocatalogo = precio,
                marcar = false,
                idmeseroproducto = UltimoCheqDet.idmeseroproducto,
                prioridadproduccion = null,
                estatuspatin = UltimoCheqDet.estatuspatin,
                idcortesia = "",
                numerotarjeta = "",
                estadomonitor = UltimoCheqDet.estadomonitor,
                llavemovto = null,
                horameserofinalizado = null,
                meserofinalizado = null,
                sistema_envio = UltimoCheqDet.sistema_envio,
                idturno_cierre = UltimoCheqDet.idturno_cierre,
                procesado = true,
                promovolumen = false, //duda
                iddispositivo = UltimoCheqDet.iddispositivo,
                productsyncidsr = -1,
                subtotalsrx = -1,
                totalsrx = -1,
                idmovtobillar = 0, //duda
                idlistaprecio = null,
                tipocambio = null,
                impuestoimporte3 = producto.Detalle.impuestoimporte3,
                estrateca_DiscountCode = "",
                estrateca_DiscountID = "",
                estrateca_DiscountAmount = 0,
            });
        }

        public void Eliminar(SR_cheqdet cheqdet)
        {
            DetallesCheque.Remove(cheqdet);

            bool modificador = cheqdet.modificador ?? false;

            if (!modificador && !string.IsNullOrEmpty(cheqdet.idproductocompuesto))
                DetallesCheque = new ObservableCollection<SR_cheqdet>(DetallesCheque.Where(x => x.idproductocompuesto != cheqdet.idproductocompuesto && x.modificador.Value).ToList());

            for (int i = 0; i < DetallesCheque.Count; i++)
            {
                DetallesCheque[i].movimiento = i + 1;
            }
        }

        public void Cambiar(SR_cheqdet cheqdet, SR_productos producto)
        {
            foreach (var detalle in DetallesCheque)
            {
                if (detalle.idproducto.Equals(cheqdet.idproducto))
                {
                    detalle.idproducto = producto.idproducto;

                    if (!CambiarPrecio)
                    {
                        detalle.precio = producto.Detalle.precio;

                        decimal precio = producto.Detalle.precio ?? 0;
                        decimal impuesto1 = producto.Detalle.impuesto1 ?? 0;
                        decimal impuesto2 = producto.Detalle.impuesto2 ?? 0;
                        decimal impuesto3 = producto.Detalle.impuesto3 ?? 0;

                        decimal preciosinumpuestos = precio - (precio * (impuesto1 * 0.01m) + precio * (impuesto2 * 0.01m) + precio * (impuesto3 * 0.01m));

                        detalle.impuesto1 = impuesto1;
                        detalle.impuesto2 = impuesto2;
                        detalle.impuesto3 = impuesto3;
                        detalle.preciosinimpuestos = preciosinumpuestos;
                        detalle.preciocatalogo = precio;
                        detalle.impuestoimporte3 = producto.Detalle.impuestoimporte3;
                    }
                }
            }
        }

        public int ObtenerCheque(long folio)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context);

                    Cheque = cheques_DAO.Get("folio", folio).FirstOrDefault();

                    if (Cheque != null)
                    {
                        DetallesCheque = new ObservableCollection<SR_cheqdet>(Cheque.Detalles);

                        UltimoCheqDet = DetallesCheque.OrderByDescending(x => x.movimiento).FirstOrDefault();


                        Personas = (int)Cheque.nopersonas;
                        Cliente = Cheque.idcliente;
                        Propina = Cheque.propina ?? 0;

                        if (Cheque.fecha.HasValue) Fecha = Cheque.fecha.Value;

                        Subtotal = string.Format("{0:C}", Cheque.subtotal);
                        Total = string.Format("{0:C}", Cheque.total);
                    }
                    else
                    {
                        return 0;
                    }


                    return 1;
                }
                catch
                {
                    return 0;
                }
            }
        }

        private SR_cheques Cheque { get; set; }
        public SR_cheqdet UltimoCheqDet { get; set; }

        private ObservableCollection<SR_cheqdet> _DetallesCheque;
        public ObservableCollection<SR_cheqdet> DetallesCheque
        {
            get { return _DetallesCheque; }
            set
            {
                _DetallesCheque = value;
                OnPropertyChanged(nameof(DetallesCheque));
            }
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

        private decimal _Propina;
        public decimal Propina
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
