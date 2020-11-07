using mod_add.Datos.Enums;
using mod_add.Datos.Modelos;
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
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
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
                    Titulo = "NO (Usar el precio del producto)",
                    Valor = false
                }
            };

            InicializarControles();
        }

        public void InicializarControles()
        {
            Folio = 0;
            Fecha = new DateTime(2000, 1, 1);
            Personas = 0;
            ClaveCliente = "";
            Condicional = Condicionales.Find(x => !x.Valor);
            Descuento = 0;
            Propina = 0;
            Subtotal = 0;
            Total = 0;

            DetallesCheque = new ObservableCollection<SR_cheqdet>();
        }

        public TipoRespuesta Guardar()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_cheqdet_DAO cheqdet_DAO = new SR_cheqdet_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_chequespagos_DAO chequespagos_DAO = new SR_chequespagos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);
                        SR_turnos_DAO turnos_DAO = new SR_turnos_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                        SR_turnos turno = turnos_DAO.Get("idturno", Cheque.idturno).FirstOrDefault();

                        var cheque = cheques_DAO.Find(Cheque.folio);

                        if (FormaPago.tipo == (int)TipoPago.EFECTIVO)
                        {
                            turno.efectivo += (cheque.efectivo - Cheque.efectivo) * -1;
                        }
                        else if (FormaPago.tipo == (int)TipoPago.TARJETA)
                        {
                            turno.tarjeta += (cheque.tarjeta - Cheque.tarjeta) * -1;
                        }
                        else if (FormaPago.tipo == (int)TipoPago.VALES)
                        {
                            turno.vales += (cheque.vales - Cheque.vales) * -1;
                        }
                        else if (FormaPago.tipo == (int)TipoPago.OTROS)
                        {
                            turno.credito += (cheque.otros - Cheque.otros) * 1;
                        }

                        cheques_DAO.Update(Cheque);

                        cheqdet_DAO.Delete(Cheque.folio);
                        cheqdet_DAO.Create(DetallesCheque.ToList());

                        chequespagos_DAO.Delete("folio = @folio", new SqlParameter("folio", Cheque.folio));
                        chequespagos_DAO.Create(Chequepago);

                        turnos_DAO.Update(turno);

                        transaction.Commit();
                        return TipoRespuesta.HECHO;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                        return TipoRespuesta.ERROR;
                    }
                }
            }
        }

        public void AniadirCliente(SR_clientes cliente)
        {
            Cheque.idcliente = cliente.idcliente;

            ClaveCliente = cliente.idcliente;
            NombreCliente = cliente.nombre;
        }

        public void AniadirProducto(SR_productos producto)
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

        public void EliminarProducto(SR_cheqdet cheqdet)
        {
            //int index = DetallesCheque.IndexOf(cheqdet);

            DetallesCheque.Remove(cheqdet);

            bool modificador = cheqdet.modificador ?? false;

            if (!modificador && !string.IsNullOrWhiteSpace(cheqdet.idproductocompuesto))
                DetallesCheque = new ObservableCollection<SR_cheqdet>(DetallesCheque.Where(x => x.idproductocompuesto != cheqdet.idproductocompuesto).ToList());

            for (int i = 0; i < DetallesCheque.Count; i++)
                DetallesCheque[i].movimiento = i + 1;

            AjustarCheque();
        }

        public void CambiarProducto(SR_cheqdet cheqdet, SR_productos producto)
        {
            var detalleProductoCheque = cheqdet.Producto.Detalle;
            var detalleProducto = producto.Detalle;

            int index = DetallesCheque.IndexOf(cheqdet);

            DetallesCheque[index].idproducto = producto.idproducto;

            if (detalleProducto.idunidad.Equals(detalleProductoCheque.idunidad))
            {
                DetallesCheque[index].cantidad = 1m;
            }

            if (!Condicional.Valor)
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
            if (Cheque == null) return;
            //validado con folio 28312  y 28138
            //falta validar el redondeo

            #region Ajuste del cheque
            //Cheque.nopersonas = Personas;
            //Cheque.propina = Propina;
            //Cheque.descuento = Descuento;
            Cheque.totalarticulos = DetallesCheque.Sum(x => x.cantidad.Value);

            decimal descuentoAplicado = (100m - Cheque.descuento.Value) / 100m;
            decimal descuento = Cheque.descuento.Value / 100m;
            decimal totalSinImpuestos_Det = Mat.Redondear(DetallesCheque.Sum(x => x.ImporteSICD));
            decimal totalConImpuestos_Det = Mat.Redondear(DetallesCheque.Sum(x => x.ImporteCICD));

            Cheque.subtotal = totalSinImpuestos_Det;
            Cheque.total = Mat.Redondear(totalConImpuestos_Det * descuentoAplicado);
            
            Cheque.totalconpropina = Cheque.total; // falta validar si la propina se agrega por configuracion
            
            Cheque.totalconcargo = Cheque.total + Cheque.cargo;
            Cheque.totalconpropinacargo = Cheque.total + Cheque.cargo; // falta validar si la propina se agrega por configuracion
            Cheque.descuentoimporte = Mat.Redondear(totalSinImpuestos_Det * descuento);

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


            Cheque.totalsindescuento = Mat.Redondear(DetallesCheque.Sum(x => x.ImporteSISD));

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

                decimal importedetalle = detalle.ImporteSICD;
                decimal importedetallesindescuento = detalle.ImporteSISD;
                decimal importedetalledescuento = importedetallesindescuento - importedetalle;

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

            Cheque.totalalimentos = Mat.Redondear(totalalimentos * descuentoAplicado);
            Cheque.totalbebidas = Mat.Redondear(totalbebidas * descuentoAplicado);
            Cheque.totalotros = Mat.Redondear(totalotros * descuentoAplicado);

            Cheque.totaldescuentos = Cheque.descuentoimporte;
            Cheque.totaldescuentoalimentos = Mat.Redondear(totalalimentosdescuento * descuento);
            Cheque.totaldescuentobebidas = Mat.Redondear(totalbebidasdescuento * descuento);
            Cheque.totaldescuentootros = Mat.Redondear(totalotrosdescuento * descuento);
            // las cortesias se mantienen?

            Cheque.totaldescuentoycortesia = Cheque.totaldescuentos + Cheque.totalcortesias;
            Cheque.totalalimentossindescuentos = totalalimentossindescuento;
            Cheque.totalbebidassindescuentos = totalbebidassindescuento;
            Cheque.totalotrossindescuentos = totalotrossindescuento;

            Cheque.subtotalcondescuento = Cheque.subtotal - Cheque.descuentoimporte;

            Cheque.totalimpuestod1 = Mat.Redondear(DetallesCheque.Sum(x => x.ImporteI1CD));
            Cheque.totalimpuestod2 = Mat.Redondear(DetallesCheque.Sum(x => x.ImporteI2CD));
            Cheque.totalimpuestod3 = Mat.Redondear(DetallesCheque.Sum(x => x.ImporteI3CD));

            Cheque.totalimpuesto1 = Cheque.totalimpuestod1;

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

            Subtotal = Mat.Redondear(Cheque.subtotal.Value, 2);
            Total = Mat.Redondear(Cheque.total.Value, 2);
        }

        public Respuesta ObtenerChequeSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                    Cheque = cheques_DAO.Get("numcheque", Folio).FirstOrDefault();

                    if (Cheque == null) return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.REGISTRO_NO_ENCONTRADO
                    };

                    if (!Funciones.ValidarMesBusqueda(App.MesesValidos, Cheque.fecha.Value))
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.FECHA_INACCESIBLE,
                            Mensaje = Cheque.fecha.Value.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es"))
                        };
                    }

                    if (Cheque.cancelado.Value) return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.CHEQUE_CANCELADO
                    };

                    var detalles = Cheque.Detalles.OrderBy(x => x.movimiento).ToList();

                    if (detalles.Count == 0) return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.SIN_REGISTROS
                    };

                    var chequespagos = Cheque.Chequespagos;

                    if (chequespagos.Count == 0)
                    {
                        return new Respuesta
                        {
                            TipoRespuesta = TipoRespuesta.CHEQUE_SIN_FORMA_PAGO
                        };
                    }

                    Chequepago = chequespagos[0];
                    FormaPago = Chequepago.Formasdepago;

                    Chequepago.idformadepago = App.ClavePagoEfectivo;

                    //DetallesCheque = new ObservableCollection<SR_cheqdet>(detalles);
                    UltimoCheqDet = detalles.OrderByDescending(x => x.movimiento).FirstOrDefault();

                    if (Cheque.fecha.HasValue) Fecha = Cheque.fecha.Value;
                    Personas = (int)Cheque.nopersonas;
                    ClaveCliente = Cheque.idcliente;
                    Descuento = (decimal)(float)Cheque.descuento.Value;
                    Propina = Mat.Redondear(Cheque.propina.Value, 2);
                    Subtotal = Mat.Redondear(Cheque.subtotal.Value);
                    Total = Mat.Redondear(Cheque.total.Value);

                    ObtenerClienteSR();

                    return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.HECHO,
                        MultipleFormaPago = chequespagos.Count > 1,
                        Cheqdet = detalles,
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return new Respuesta
                    {
                        TipoRespuesta = TipoRespuesta.ERROR
                    };
                }
            }
        }

        public void CargarResultados(Respuesta respuesta)
        {
            try
            {
                foreach (var modelo in respuesta.Cheqdet)
                {
                    DetallesCheque.Add(new SR_cheqdet
                    {
                        foliodet = modelo.foliodet,
                        movimiento = modelo.movimiento,
                        comanda = modelo.comanda,
                        cantidad = (decimal)(float)modelo.cantidad,
                        idproducto = modelo.idproducto,
                        descuento = Math.Round(modelo.descuento ?? 0, 2),
                        precio = Math.Round(modelo.precio ?? 0, 2),
                        impuesto1 = modelo.impuesto1,
                        impuesto2 = modelo.impuesto2,
                        impuesto3 = modelo.impuesto3,
                        preciosinimpuestos = modelo.preciosinimpuestos,
                        tiempo = modelo.tiempo,
                        hora = modelo.hora,
                        modificador = modelo.modificador,
                        mitad = modelo.mitad,
                        comentario = modelo.comentario,
                        idestacion = modelo.idestacion,
                        usuariodescuento = modelo.usuariodescuento,
                        comentariodescuento = modelo.comentariodescuento,
                        idtipodescuento = modelo.idtipodescuento,
                        horaproduccion = modelo.horaproduccion,
                        idproductocompuesto = modelo.idproductocompuesto,
                        productocompuestoprincipal = modelo.productocompuestoprincipal,
                        preciocatalogo = modelo.preciocatalogo,
                        marcar = modelo.marcar,
                        idmeseroproducto = modelo.idmeseroproducto,
                        prioridadproduccion = modelo.prioridadproduccion,
                        estatuspatin = modelo.estatuspatin,
                        idcortesia = modelo.idcortesia,
                        numerotarjeta = modelo.numerotarjeta,
                        estadomonitor = modelo.estadomonitor,
                        llavemovto = modelo.llavemovto,
                        horameserofinalizado = modelo.horameserofinalizado,
                        meserofinalizado = modelo.meserofinalizado,
                        sistema_envio = modelo.sistema_envio,
                        idturno_cierre = modelo.idturno_cierre,
                        procesado = modelo.procesado,
                        promovolumen = modelo.promovolumen,
                        iddispositivo = modelo.iddispositivo,
                        productsyncidsr = modelo.productsyncidsr,
                        subtotalsrx = modelo.subtotalsrx,
                        totalsrx = modelo.totalsrx,
                        idlistaprecio = modelo.idlistaprecio,
                        tipocambio = modelo.tipocambio,
                        idmovtobillar = modelo.idmovtobillar,
                        impuestoimporte3 = modelo.impuestoimporte3,
                        estrateca_DiscountCode = modelo.estrateca_DiscountCode,
                        estrateca_DiscountID = modelo.estrateca_DiscountID,
                        estrateca_DiscountAmount = modelo.estrateca_DiscountAmount
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
            }
        }

        public void ObtenerClienteSR()
        {
            if (string.IsNullOrWhiteSpace(ClaveCliente)) return;

            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_clientes_DAO clientes_DAO = new SR_clientes_DAO(context);
                    var cliente = clientes_DAO.Find(ClaveCliente);

                    NombreCliente = cliente.nombre;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                }
            }
        }

        private SR_cheques Cheque { get; set; }
        private SR_formasdepago FormaPago { get; set; }
        private SR_chequespagos Chequepago { get; set; }
        private SR_cheqdet UltimoCheqDet { get; set; }

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

        private Condicional condicional;
        public Condicional Condicional
        {
            get { return condicional; }
            set
            {
                condicional = value;
                OnPropertyChanged(nameof(Condicional));
            }
        }

        private int _Personas;
        public int Personas
        {
            get { return _Personas; }
            set
            {
                if (Cheque != null) Cheque.nopersonas = value;
                _Personas = value;
                OnPropertyChanged(nameof(Personas));
            }
        }

        private string claveCliente;
        public string ClaveCliente
        {
            get { return claveCliente; }
            set
            {
                claveCliente = value;
                OnPropertyChanged(nameof(ClaveCliente));
            }
        }

        private string nombreCliente;
        public string NombreCliente
        {
            get { return nombreCliente; }
            set
            {
                nombreCliente = value;
                OnPropertyChanged(nameof(nombreCliente));
            }
        }

        private decimal _Descuento;
        public decimal Descuento
        {
            get { return _Descuento; }
            set
            {
                if (Cheque != null) Cheque.descuento = value;
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
                if (Cheque != null) Cheque.propina = value;
                _Propina = value;
                OnPropertyChanged(nameof(Propina));
            }
        }

        private decimal _Subtotal;
        public decimal Subtotal
        {
            get { return _Subtotal; }
            set
            {
                _Subtotal = value;
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        private decimal _Total;
        public decimal Total
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
