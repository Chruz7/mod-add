﻿using mod_add.Datos.Enums;
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

                        cheques_DAO.Update(Cheque);

                        cheqdet_DAO.Delete(Cheque.folio);
                        cheqdet_DAO.Create(DetallesCheque.ToList());

                        chequespagos_DAO.Update(Chequepago);

                        SR_turnos turno;

                        if (App.ConfiguracionSistema.ModificarVentasReales)
                            turno = turnos_DAO.Get("idturno", Cheque.Idturno).FirstOrDefault();
                        else
                            turno = turnos_DAO.Get("idturno", Cheque.idturno_f).FirstOrDefault();

                        var cheque = cheques_DAO.Find(Cheque.folio);

                        if (FormaPago.tipo == (int)TipoPago.EFECTIVO)
                        {
                            turno.efectivo += (cheque.efectivo - Cheque.efectivo);
                        }
                        else if (FormaPago.tipo == (int)TipoPago.TARJETA)
                        {
                            turno.tarjeta += (cheque.tarjeta - Cheque.tarjeta);
                        }
                        else if (FormaPago.tipo == (int)TipoPago.VALES)
                        {
                            turno.vales += (cheque.vales - Cheque.vales);
                        }
                        else if (FormaPago.tipo == (int)TipoPago.OTROS)
                        {
                            turno.credito += (cheque.otros - Cheque.otros);
                        }

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

        public void ResgistrarBitacora()
        {
            Funciones.RegistrarModificacion(new BitacoraModificacion
            {
                TipoAjuste = TipoAjuste.UNICO,
                FechaProceso = DateTime.Now,
                FechaInicialVenta = Cheque.fecha.Value,
                FechaFinalVenta = Cheque.cierre.Value,
                TotalCuentas = 1,
                CuentasModificadas = 1,
                ImporteAnterior = ImporteAnterior,
                ImporteNuevo = Total,
                Diferencia = Math.Round(Math.Abs(ImporteAnterior - Total) / ImporteAnterior * 100m, 2, MidpointRounding.AwayFromZero),
            });
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
            Cheque.nopersonas = Personas;
            Cheque.propina = Propina;
            Cheque.descuento = Descuento;
            Cheque.totalarticulos = DetallesCheque.Sum(x => x.cantidad.Value);

            decimal descuentoAplicado = (100m - Cheque.descuento.Value) / 100m;
            decimal descuento = Cheque.descuento.Value / 100m;
            decimal totalSinImpuestos_Det = Mat.Redondeo(DetallesCheque.Sum(x => x.ImporteSICD));
            decimal totalConImpuestos_Det = Mat.Redondeo(DetallesCheque.Sum(x => x.ImporteCICD));

            Cheque.subtotal = totalSinImpuestos_Det;
            Cheque.total = Mat.Redondeo(totalConImpuestos_Det * descuentoAplicado);
            
            Cheque.totalconpropina = Cheque.total; // falta validar si la propina se agrega por configuracion
            
            Cheque.totalconcargo = Cheque.total + Cheque.cargo;
            Cheque.totalconpropinacargo = Cheque.total + Cheque.cargo; // falta validar si la propina se agrega por configuracion
            Cheque.descuentoimporte = Mat.Redondeo(totalSinImpuestos_Det * descuento);

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


            Cheque.totalsindescuento = Mat.Redondeo(DetallesCheque.Sum(x => x.ImporteSISD));

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

            Cheque.totalalimentos = Mat.Redondeo(totalalimentos * descuentoAplicado);
            Cheque.totalbebidas = Mat.Redondeo(totalbebidas * descuentoAplicado);
            Cheque.totalotros = Mat.Redondeo(totalotros * descuentoAplicado);

            Cheque.totaldescuentos = Cheque.descuentoimporte;
            Cheque.totaldescuentoalimentos = Mat.Redondeo(totalalimentosdescuento * descuento);
            Cheque.totaldescuentobebidas = Mat.Redondeo(totalbebidasdescuento * descuento);
            Cheque.totaldescuentootros = Mat.Redondeo(totalotrosdescuento * descuento);
            // las cortesias se mantienen?

            Cheque.totaldescuentoycortesia = Cheque.totaldescuentos + Cheque.totalcortesias;
            Cheque.totalalimentossindescuentos = totalalimentossindescuento;
            Cheque.totalbebidassindescuentos = totalbebidassindescuento;
            Cheque.totalotrossindescuentos = totalotrossindescuento;

            Cheque.subtotalcondescuento = Cheque.subtotal - Cheque.descuentoimporte;

            Cheque.totalimpuestod1 = Mat.Redondeo(DetallesCheque.Sum(x => x.ImporteI1CD));
            Cheque.totalimpuestod2 = Mat.Redondeo(DetallesCheque.Sum(x => x.ImporteI2CD));
            Cheque.totalimpuestod3 = Mat.Redondeo(DetallesCheque.Sum(x => x.ImporteI3CD));

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

            Subtotal = Cheque.subtotal.Value;
            Total = Cheque.total.Value;
        }

        public TipoRespuesta ObtenerChequeSR()
        {
            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_cheques_DAO cheques_DAO = new SR_cheques_DAO(context, !App.ConfiguracionSistema.ModificarVentasReales);

                    Cheque = cheques_DAO.Get("folio = @folio and fecha <= @fecha", new object[] {
                        new SqlParameter("folio", Folio),
                        new SqlParameter("fecha", App.FechaMaxima),
                    }).FirstOrDefault();

                    if (Cheque == null) return TipoRespuesta.REGISTRO_NO_ENCONTRADO;
                    if (Cheque.cancelado.Value) return TipoRespuesta.CHEQUE_CANCELADO;
                    var detalles = Cheque.Detalles;
                    if (detalles.Count == 0) return TipoRespuesta.SIN_REGISTROS;

                    var chequespagos = Cheque.Chequespagos;

                    if (chequespagos.Count > 1)
                    {
                        return TipoRespuesta.CHEQUE_CON_MULTIPLE_FORMA_PAGO;
                    }
                    else if (chequespagos.Count < 1)
                    {
                        return TipoRespuesta.CHEQUE_SIN_FORMA_PAGO;
                    }

                    Chequepago = chequespagos[0];
                    FormaPago = Chequepago.Formasdepago;

                    DetallesCheque = new ObservableCollection<SR_cheqdet>(detalles.OrderBy(x => x.movimiento).ToList());
                    UltimoCheqDet = detalles.OrderByDescending(x => x.movimiento).FirstOrDefault();

                    if (Cheque.fecha.HasValue) Fecha = Cheque.fecha.Value;
                    Personas = (int)Cheque.Nopersonas;
                    ClaveCliente = Cheque.idcliente;
                    Descuento = Cheque.descuento.Value;
                    Propina = Cheque.propina.Value;
                    Subtotal = Cheque.subtotal.Value;
                    Total = Cheque.total.Value;
                    ImporteAnterior = Total;

                    ObtenerClienteSR();

                    return TipoRespuesta.HECHO;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return TipoRespuesta.ERROR;
                }
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
        private decimal ImporteAnterior { get; set; }

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
