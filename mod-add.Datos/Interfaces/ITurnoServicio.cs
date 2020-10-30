using mod_add.Datos.Infraestructura;
using mod_add.Datos.Modelos;

namespace mod_add.Datos.Interfaces
{
    public interface ITurnoServicio : IServiceBase<Turno>
    {
        object[] Obteneridturno();

        long Primeridturno();
    }
}
