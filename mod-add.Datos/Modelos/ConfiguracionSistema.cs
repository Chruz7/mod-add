using System.ComponentModel.DataAnnotations.Schema;

namespace mod_add.Datos.Modelos
{
    [Table("Configuracion_Sistema")]
    public class ConfiguracionSistema
    {
        public int Id { get; set; }
        public bool ModificarVentasReales { get; set; }
        public int MinProductosCuenta { get; set; }
        public bool EliminarProductosSeleccionados { get; set; }
        public string Contrasena { get; set; }
        public string ContrasenaAdmin { get; set; }
    }
}
