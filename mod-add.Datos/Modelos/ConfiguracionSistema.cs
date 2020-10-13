namespace mod_add.Datos.Modelos
{
    public class ConfiguracionSistema
    {
        public int Id { get; set; }
        public bool ModificarVentasReales { get; set; }
        public int MinProductosCuenta { get; set; }
        public bool EliminarProductosSeleccionados { get; set; }
    }
}
