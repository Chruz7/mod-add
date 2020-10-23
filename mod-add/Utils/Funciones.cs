using mod_add.Datos.Implementaciones;
using mod_add.Datos.Infraestructura;
using mod_add.Datos.Interfaces;
using mod_add.Datos.Modelos;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace mod_add.Utils
{
    public static class Funciones
    {
        public static List<T> CloneList<T>(List<T> oldList)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<T>)formatter.Deserialize(stream);
        }


        public static void RegistrarModificacion(BitacoraModificacion bitacora)
        {
            DatabaseFactory dbf = new DatabaseFactory();
            IBitacoraServicio bitacoraServicio = new BitacoraServicio(dbf);

            bitacoraServicio.Create(bitacora);
        }
    }
}
