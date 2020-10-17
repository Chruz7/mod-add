using mod_add.Enums;
using mod_add.Selectores;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using SRLibrary.SR_DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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

            Inicializar();
        }

        public void Inicializar()
        {
            Folio = "";
            Fecha = new DateTime(2000, 1, 1);
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
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context);

                    cheques_DAO.Update(Cheque);

                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context);

                    cheqdet_DAO.Delete(Cheque.folio);
                    cheqdet_DAO.Create(DetallesCheque.ToList());

                    return 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return 0;
                }
            }
        }

        public void Aniadir(SR_productos producto)
        {
            int ultimoMovimiento = (int)DetallesCheque.Max(x => x.movimiento);

            var detalleProducto = producto.Detalle;

            DetallesCheque.Add(new SR_cheqdet
            {
                foliodet = Cheque.folio,
                movimiento = ultimoMovimiento + 1,
                comanda = UltimoCheqDet.comanda,
                cantidad = 1,
                idproducto = producto.idproducto,
                descuento = 0,
                precio = detalleProducto.precio,
                impuesto1 = detalleProducto.impuesto1,
                impuesto2 = detalleProducto.impuesto1,
                impuesto3 = detalleProducto.impuesto1,
                preciosinimpuestos = detalleProducto.preciosinimpuestos,
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
                preciocatalogo = detalleProducto.precio,
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
                impuestoimporte3 = detalleProducto.impuestoimporte3,
                estrateca_DiscountCode = "",
                estrateca_DiscountID = "",
                estrateca_DiscountAmount = 0,
            });

            AjustarCheque();
        }

        public void Eliminar(SR_cheqdet cheqdet)
        {
            int index = DetallesCheque.IndexOf(cheqdet);

            DetallesCheque.RemoveAt(index);

            bool modificador = cheqdet.modificador ?? false;

            if (!modificador && !string.IsNullOrWhiteSpace(cheqdet.idproductocompuesto))
                DetallesCheque = new ObservableCollection<SR_cheqdet>(DetallesCheque.Where(x => x.idproductocompuesto != cheqdet.idproductocompuesto && x.modificador.Value).ToList());

            for (int i = 0; i < DetallesCheque.Count; i++)
                DetallesCheque[i].movimiento = i + 1;

            AjustarCheque();
        }

        public void Cambiar(SR_cheqdet cheqdet, SR_productos producto)
        {
            foreach (var detalleCheque in DetallesCheque)
            {
                if (detalleCheque.idproducto.Equals(cheqdet.idproducto))
                {
                    detalleCheque.idproducto = producto.idproducto;

                    if (!CambiarPrecio)
                    {
                        var detalleProducto = producto.Detalle;

                        detalleCheque.precio = detalleProducto.precio;
                        detalleCheque.impuesto1 = detalleProducto.impuesto1;
                        detalleCheque.impuesto2 = detalleProducto.impuesto2;
                        detalleCheque.impuesto3 = detalleProducto.impuesto3;
                        detalleCheque.preciosinimpuestos = detalleProducto.preciosinimpuestos;
                        detalleCheque.preciocatalogo = detalleProducto.precio;
                        detalleCheque.impuestoimporte3 = detalleProducto.impuestoimporte3;
                    }
                }
            }

            AjustarCheque();
        }

        public void AjustarCheque()
        {
            Cheque.propina = Propina;
            Cheque.descuento = Descuento;
            Cheque.totalarticulos = DetallesCheque.Count;

            decimal subtotalTemp = Math.Round(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.cantidad ?? 0), 2, App.MidpointRounding);

            decimal totalTemp = Math.Round(DetallesCheque.Sum(x => x.precio ?? 0 * x.cantidad ?? 0), 2, App.MidpointRounding);

            decimal descuentoimporte = Math.Round(subtotalTemp - (subtotalTemp * (Descuento / 100m)), 2, App.MidpointRounding);

            Cheque.descuentoimporte = descuentoimporte;

            Cheque.subtotal = subtotalTemp;
            Cheque.subtotalcondescuento = Cheque.subtotal - Cheque.descuentoimporte;

            Cheque.total = Math.Round(totalTemp - (totalTemp * (Descuento / 100m)), 2, App.MidpointRounding);

            Cheque.totalsindescuento = subtotalTemp;
            Cheque.totalconpropina = Cheque.total + Cheque.propina;


            foreach (var detalle in DetallesCheque)
            {
                decimal precio = detalle.precio ?? 0;
                decimal impuesto1 = detalle.impuesto1 ?? 0;
                decimal impuesto2 = detalle.impuesto2 ?? 0;
                decimal impuesto3 = detalle.impuesto3 ?? 0;
                decimal preciosinimpuestos = detalle.preciosinimpuestos ?? 0;
                decimal cantidad = detalle.cantidad ?? 0;

                Cheque.totalimpuesto1 += (preciosinimpuestos * (impuesto1 * 0.01m)) * cantidad;
                Cheque.totalimpuestod1 += (preciosinimpuestos * (impuesto1 * 0.01m)) * cantidad;
                Cheque.totalimpuestod2 += (preciosinimpuestos * (impuesto2 * 0.01m)) * cantidad;
                Cheque.totalimpuestod3 += (preciosinimpuestos * (impuesto3 * 0.01m)) * cantidad;

                var producto = detalle.Producto;
                var grupo = producto.Grupo;

                if (grupo.clasificacion == 1)
                {
                    Cheque.totalbebidassinimpuestos += (preciosinimpuestos * cantidad);
                }

                if (grupo.clasificacion == 2)
                {
                    Cheque.totalalimentossinimpuestos += (preciosinimpuestos * cantidad);
                }

                if (grupo.clasificacion == 3)
                {
                    Cheque.totalotrossinimpuestos += (preciosinimpuestos * cantidad);
                }
            }

            Cheque.totalbebidas = Cheque.totalbebidassindescuentos - (Cheque.totalbebidassindescuentos * (Descuento / 100m));
            Cheque.totalalimentos = Cheque.totalalimentossindescuentos - (Cheque.totalalimentossinimpuestos * (Descuento / 100m));
            Cheque.totalotros = Cheque.totalotrossindescuentos - (Cheque.totalotrossinimpuestos * (Descuento / 100m));


            Cheque.totalconcargo = Cheque.total + Cheque.cargo;
            Cheque.totalconpropinacargo = Cheque.total + Cheque.propina + Cheque.cargo;

            Cheque.cambio = 0; // como se debe ajustar el cambio si es efectivo? - por el momento queda en ceros
            Cheque.cambiorepartidor = 0;


            //if (FormaPago.tipo == 1)
            //{
            //    Cheque.efectivo = Cheque.total;
            //}
            //else if (FormaPago.tipo == 2)
            //{
            //    Cheque.tarjeta = Cheque.total;

            //    Cheque.propinatarjeta = Cheque.propina;
            //}
            //else if (FormaPago.tipo == 3)
            //{
            //    Cheque.vales = Cheque.total;
            //}
            //else if (FormaPago.tipo == 4)
            //{
            //    Cheque.otros = Cheque.total;
            //}

            Descuento = Cheque.descuento ?? 0;
            Propina = Cheque.propina ?? 0;

            Subtotal = string.Format("{0:C}", Cheque.subtotal);
            Total = string.Format("{0:C}", Cheque.total);
        }

        public SR_productos ObtenerProductoSR(string idproducto)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                SR_productos_DAO productos_DAO = new SR_productos_DAO();

                return productos_DAO.Find(idproducto);
            }
        }

        public Respuesta ObtenerCheque(long folio)
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context);

                    Cheque = cheques_DAO.Get("folio", folio).FirstOrDefault();

                    if (Cheque != null)
                    {
                        //var chequespagos = Cheque.Chequespagos;

                        //if (chequespagos.Count != 1)
                        //{
                        //    return Respuesta.MAS_DE_UNA_FORMA_PAGO;
                        //}

                        //FormaPago = chequespagos[0].Formasdepago;

                        DetallesCheque = new ObservableCollection<SR_cheqdet>(Cheque.Detalles);

                        UltimoCheqDet = DetallesCheque.OrderByDescending(x => x.movimiento).FirstOrDefault();


                        if (Cheque.fecha.HasValue) Fecha = Cheque.fecha.Value;
                        Personas = (int)Cheque.nopersonas;
                        Cliente = Cheque.idcliente;
                        Descuento = Cheque.descuento ?? 0;
                        Propina = Cheque.propina ?? 0;

                        Subtotal = string.Format("{0:C}", Cheque.subtotal);
                        Total = string.Format("{0:C}", Cheque.total);
                    }
                    else
                    {
                        return Respuesta.CHEQUE_NO_ENCONTRADO;
                    }


                    return Respuesta.HECHO;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    return Respuesta.ERROR;
                }
            }
        }

        public SR_formasdepago FormaPago { get; set; }

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

        private decimal _Descuento;
        public decimal Descuento
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
