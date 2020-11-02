using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using mod_add.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Design;
using System.Diagnostics;
using System.Linq;

namespace mod_add.ViewModels
{
    public class LicenciaViewModel : ViewModelBase
    {
        private readonly DatabaseFactory dbf;
        private readonly IRegistroLicenciaServicio registroLicenciaServicio;

        public LicenciaViewModel()
        {
            dbf = new DatabaseFactory();
            registroLicenciaServicio = new RegistroLicenciaServicio(dbf);

            Licencia = "";

            ObtenerRegistrosLicencia();
        }

        public TipoRespuesta Guardar()
        {
            //FALTA VALIDAR
            SRLibrary.Utils.valideSerialKey valideSerial = new SRLibrary.Utils.valideSerialKey();

            try
            {
                if (!valideSerial.isValidSerial(Licencia))
                    return TipoRespuesta.LICENCIA_INCORRECTA;

                if (registroLicenciaServicio.Exite(Licencia))
                    return TipoRespuesta.EXITE;

                var fecha = DateTime.Parse(valideSerial.getDecryptSerial(Licencia));

                int result = registroLicenciaServicio.Create(new RegistroLicencia { 
                    Anio = fecha.Year,
                    Mes = fecha.Month,
                    Licencia = Licencia
                });

                return result == 1 ? TipoRespuesta.HECHO : TipoRespuesta.ERROR;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"INICIO-ERROR\n{ex}\nFIN-ERROR");
                return TipoRespuesta.ERROR;
            }
        }

        public void ObtenerRegistrosLicencia()
        {
            RegistrosLicencia = registroLicenciaServicio.GetAll().OrderBy(x => x.Anio).ThenBy(x => x.Mes).ToList();

            App.ObtenerLicencias();
        }

        private List<RegistroLicencia> registrosLicencia;

        public List<RegistroLicencia> RegistrosLicencia
        {
            get { return registrosLicencia; }
            set
            {
                registrosLicencia = value;
                OnPropertyChanged(nameof(RegistrosLicencia));
            }
        }


        private string licencia;
        public string Licencia
        {
            get { return licencia; }
            set
            {
                licencia = value;
                OnPropertyChanged(nameof(Licencia));
            }
        }
    }
}
