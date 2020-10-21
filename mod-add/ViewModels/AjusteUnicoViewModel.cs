using mod_add.Enums;
using mod_add.Selectores;
using mod_add.Utils;
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
            Folio = 0;
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
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                    SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                    cheques_DAO.Update(Cheque);

                    cheqdet_DAO.Delete(Cheque.folio);
                    cheqdet_DAO.Create(DetallesCheque.ToList());

                    chequespagos_DAO.Update(Chequepago);

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
                comanda = "",
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
                DetallesCheque = new ObservableCollection<SR_cheqdet>(DetallesCheque.Where(x => x.idproductocompuesto != cheqdet.idproductocompuesto).ToList());

            for (int i = 0; i < DetallesCheque.Count; i++)
                DetallesCheque[i].movimiento = i + 1;

            AjustarCheque();
        }

        public void Cambiar(SR_cheqdet cheqdet, SR_productos producto)
        {
            var detalleProductoCheque = cheqdet.Producto.Detalle;
            var detalleProducto = producto.Detalle;

            int index = DetallesCheque.IndexOf(cheqdet);

            DetallesCheque[index].idproducto = producto.idproducto;

            if (detalleProducto.idunidad != detalleProductoCheque.idunidad)
            {
                decimal nuevaCantidad = Math.Round(DetallesCheque[index].cantidad ?? 0, 0, App.MidpointRounding);
                DetallesCheque[index].cantidad = nuevaCantidad >= 1 ? nuevaCantidad : 1;
            }

            if (!CambiarPrecio)
            {
                DetallesCheque[index].precio = detalleProducto.precio;
                DetallesCheque[index].impuesto1 = detalleProducto.impuesto1;
                DetallesCheque[index].impuesto2 = detalleProducto.impuesto2;
                DetallesCheque[index].impuesto3 = detalleProducto.impuesto3;
                DetallesCheque[index].preciosinimpuestos = detalleProducto.preciosinimpuestos;
                DetallesCheque[index].preciocatalogo = detalleProducto.precio;
                DetallesCheque[index].impuestoimporte3 = detalleProducto.impuestoimporte3;
                DetallesCheque[index].idproductocompuesto = "";
            }

            bool modificador = cheqdet.modificador ?? false;

            if (!modificador && !string.IsNullOrWhiteSpace(cheqdet.idproductocompuesto))
                DetallesCheque = new ObservableCollection<SR_cheqdet>(DetallesCheque.Where(x => x.idproductocompuesto != cheqdet.idproductocompuesto).ToList());

            for (int i = 0; i < DetallesCheque.Count; i++)
                DetallesCheque[i].movimiento = i + 1;

            AjustarCheque();

            //al cambiar el producto conserva el descuento?
        }

        public void AjustarCheque()
        {
            //validado con folio 28312  y 28138
            //falta validar el redondeo

            #region Ajuste del cheque
            Cheque.nopersonas = Personas;
            Cheque.propina = Propina;
            Cheque.descuento = Descuento;
            Cheque.totalarticulos = DetallesCheque.Count;

            decimal totalSinImpuestos_Det = Mat.Redondeo(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.cantidad ?? 0 * (100m - x.descuento ?? 0) / 100m));
            decimal totalConImpuestos_Det = Mat.Redondeo(DetallesCheque.Sum(x => x.precio ?? 0 * x.cantidad ?? 0 * (100m - x.descuento ?? 0) / 100m));

            Cheque.subtotal = totalSinImpuestos_Det;
            Cheque.total = Mat.Redondeo(totalConImpuestos_Det * (100m - Cheque.descuento ?? 0) / 100m);
            
            Cheque.totalconpropina = Cheque.total; // falta validar si la propina se agrega por configuracion
            
            Cheque.totalimpuesto1 = Mat.Redondeo(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.impuesto1 ?? 0 / 100m * x.cantidad ?? 0 * (100m - x.descuento ?? 0) / 100m));
            
            Cheque.totalconcargo = Cheque.total + Cheque.cargo;
            Cheque.totalconpropinacargo = Cheque.total + Cheque.cargo; // falta validar si la propina se agrega por configuracion
            Cheque.descuentoimporte = Mat.Redondeo(totalSinImpuestos_Det * Cheque.descuento ?? 0 / 100m); ;

            if (FormaPago.tipo == (int)TipoPago.EFECTIVO)
            {
                Cheque.efectivo = Cheque.total;
            }
            else if (FormaPago.tipo == (int)TipoPago.TARJETA)
            {
                Cheque.tarjeta = Cheque.total;
                Cheque.propinatarjeta = Cheque.propina;
            }
            else if (FormaPago.tipo == (int)TipoPago.VALES)
            {
                Cheque.vales = Cheque.total;
            }
            else if (FormaPago.tipo == (int)TipoPago.OTROS)
            {
                Cheque.otros = Cheque.total;
            }


            Cheque.totalsindescuento = Mat.Redondeo(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.cantidad ?? 0));

            decimal totalalimentos = 0;
            decimal totalbebidas = 0;
            decimal totalotros = 0;
            decimal totalalimentosdescuento = 0;
            decimal totalbebidasdescuento = 0;
            decimal totalotrosdescuento = 0;
            decimal totalalimentossindescuento = 0;
            decimal totalbebidassindescuento = 0;
            decimal totalotrossindescuento = 0;

            foreach (var detalle in DetallesCheque)
            {
                var producto = detalle.Producto;
                var grupo = producto.Grupo;

                decimal importedetalle = detalle.preciosinimpuestos ?? 0 * detalle.cantidad ?? 0 * (100m - detalle.descuento ?? 0) / 100m;
                decimal importedetalledescuento = detalle.preciosinimpuestos ?? 0 * detalle.cantidad ?? 0 * detalle.descuento ?? 0 / 100m;
                decimal importedetallesindescuento = detalle.preciosinimpuestos ?? 0 * detalle.cantidad ?? 0;

                if (grupo.clasificacion == 1)
                {
                    totalbebidas += importedetalle;
                    totalbebidasdescuento += importedetalledescuento;
                    totalbebidassindescuento += importedetallesindescuento;
                }
                else if (grupo.clasificacion == 2)
                {
                    totalalimentos += importedetalle;
                    totalalimentosdescuento += importedetalledescuento;
                    totalalimentossindescuento += importedetallesindescuento;
                }
                else if (grupo.clasificacion == 3)
                {
                    totalotros += importedetalle;
                    totalotrosdescuento += importedetalledescuento;
                    totalotrossindescuento += importedetallesindescuento;
                }
            }

            Cheque.totalalimentos = Mat.Redondeo(totalalimentos * (100m - Descuento) / 100m);
            Cheque.totalbebidas = Mat.Redondeo(totalbebidas * (100m - Descuento) / 100m);
            Cheque.totalotros = Mat.Redondeo(totalotros * (100m - Descuento) / 100m);

            Cheque.totaldescuentos = Cheque.descuentoimporte;
            Cheque.totaldescuentoalimentos = Mat.Redondeo(totalalimentosdescuento * Descuento / 100m);
            Cheque.totaldescuentobebidas = Mat.Redondeo(totalbebidasdescuento * Descuento / 100m);
            Cheque.totaldescuentootros = Mat.Redondeo(totalotrosdescuento * Descuento / 100m);
            // las cortesias se mantienen?

            Cheque.totaldescuentoycortesia = Cheque.totaldescuentos + Cheque.totalcortesias;
            Cheque.totalalimentossindescuentos = totalalimentossindescuento;
            Cheque.totalbebidassindescuentos = totalbebidassindescuento;
            Cheque.totalotrossindescuentos = totalotrossindescuento;

            Cheque.subtotalcondescuento = Cheque.subtotal - Cheque.descuentoimporte;

            Cheque.totalimpuestod1 = Mat.Redondeo(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.impuesto1 ?? 0 / 100m * x.cantidad ?? 0 * (100m - x.descuento ?? 0) / 100m));
            Cheque.totalimpuestod2 = Mat.Redondeo(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.impuesto2 ?? 0 / 100m * x.cantidad ?? 0 * (100m - x.descuento ?? 0) / 100m));
            Cheque.totalimpuestod3 = Mat.Redondeo(DetallesCheque.Sum(x => x.preciosinimpuestos ?? 0 * x.impuesto3 ?? 0 / 100m * x.cantidad ?? 0 * (100m - x.descuento ?? 0) / 100m));

            Cheque.desc_imp_original = Cheque.descuentoimporte;

            Cheque.cambio = 0; // como se debe ajustar el cambio si es efectivo? - por el momento queda en ceros
            Cheque.cambiorepartidor = 0;

            //falta ajustar mas campos del cheque
            #endregion

            #region Ajuste del cheque pago
            Chequepago.importe = Cheque.total;

            if (FormaPago.tipo == (int)TipoPago.TARJETA)
            {
                Chequepago.propina = Cheque.propina;
            }
            #endregion

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

        public Respuesta ObtenerCheque()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                    Cheque = cheques_DAO.Find(Folio);

                    if (Cheque == null) return Respuesta.CHEQUE_NO_ENCONTRADO;
                    if (Cheque.cancelado.Value) return Respuesta.CHEQUE_CANCELADO;

                    var chequespagos = Cheque.Chequespagos;

                    if (chequespagos.Count > 1)
                    {
                        return Respuesta.CHEQUE_CON_MULTIPLE_FORMA_PAGO;
                    }
                    else if (chequespagos.Count < 1)
                    {
                        return Respuesta.CHEQUE_SIN_FORMA_PAGO;
                    }

                    Chequepago = chequespagos[0];
                    FormaPago = Chequepago.Formasdepago;

                    DetallesCheque = new ObservableCollection<SR_cheqdet>(Cheque.Detalles.OrderBy(x => x.movimiento).ToList());

                    UltimoCheqDet = DetallesCheque.OrderByDescending(x => x.movimiento).FirstOrDefault();


                    if (Cheque.fecha.HasValue) Fecha = Cheque.fecha.Value;
                    Personas = (int)Cheque.Nopersonas;
                    Cliente = Cheque.idcliente;
                    Descuento = Cheque.descuento ?? 0;
                    Propina = Cheque.propina ?? 0;

                    Subtotal = string.Format("{0:C}", Cheque.subtotal);
                    Total = string.Format("{0:C}", Cheque.total);

                    return Respuesta.HECHO;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return Respuesta.ERROR;
                }
            }
        }


        private SR_cheques Cheque { get; set; }
        private SR_formasdepago FormaPago { get; set; }
        private SR_chequespagos Chequepago { get; set; }
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

        private long _Folio;
        public long Folio
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
