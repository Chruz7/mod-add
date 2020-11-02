using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace mod_add.Datos.Modelos
{
    [Table("Registro_Licencias")]
    public class RegistroLicencia
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string Licencia { get; set; }
        public string MesDisplay
        {
            get
            {
                DateTime fecha = new DateTime(Anio, Mes, 1);

                return fecha.ToString("MMM - yyyy", CultureInfo.CreateSpecificCulture("es")).ToUpper();
            }
        }
    }
}
