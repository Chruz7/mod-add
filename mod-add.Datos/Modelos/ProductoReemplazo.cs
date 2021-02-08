using System.ComponentModel.DataAnnotations.Schema;

namespace mod_add.Datos.Modelos
{
    [Table("Productos_Reemplazo")]
    public class ProductoReemplazo
    {
        public int Id { get; set; }
        public bool Reemplazar { get; set; }
        public string Clave { get; set; }
        public int Porcentaje { get; set; }
        [NotMapped]
        public bool OmitirPorActuzalizacion { get; set; }
    }
}
