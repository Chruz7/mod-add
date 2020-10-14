﻿using System.ComponentModel.DataAnnotations.Schema;

namespace mod_add.Datos.Modelos
{
    public class ProductoEliminar
    {
        public int Id { get; set; }
        public string Clave { get; set; }
        public bool Eliminar { get; set; }

        [NotMapped]
        public string Grupo { get; set; }
        [NotMapped]
        public string Descripcion { get; set; }
        [NotMapped]
        public bool Guardado { get; set; }
    }
}
