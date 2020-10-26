using mod_add.Enums;
using SR.Datos;
using SRLibrary.SR_Context;
using SRLibrary.SR_DAO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace mod_add.ViewModels
{
    public class SeleccionClienteViewModel : ViewModelBase
    {
        public SeleccionClienteViewModel()
        {
            Clientes = new List<SR_clientes>();
        }

        public TipoRespuesta ObtenerClientesSR()
        {
            if (string.IsNullOrEmpty(Buscador)) return TipoRespuesta.NADA;

            using (SoftRestaurantDBContext context = new SoftRestaurantDBContext())
            {
                try
                {
                    SR_clientes_DAO clientes_DAO = new SR_clientes_DAO(context);

                    Clientes = clientes_DAO.Get("nombre LIKE @texto + '%' OR idcliente LIKE @texto + '%'", new object[] {
                        new SqlParameter("texto", Buscador)
                    });

                    return Clientes.Count > 0 ? TipoRespuesta.HECHO : TipoRespuesta.SIN_REGISTROS;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                    return TipoRespuesta.ERROR;
                }
            }
        }

        private string buscador;
        public string Buscador
        {
            get { return buscador; }
            set
            {
                buscador = value;
                OnPropertyChanged(nameof(Buscador));
            }
        }

        private List<SR_clientes> clientes;
        public List<SR_clientes> Clientes
        {
            get { return clientes; }
            set
            {
                clientes = value;
                OnPropertyChanged(nameof(Clientes));
            }
        }
    }
}
