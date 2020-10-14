using System;
using mod_add.Datos.Contexto;

namespace mod_add.Datos.Infraestructura
{
    public interface IDatabaseFactory : IDisposable
    {
        ApplicationDbContext Get();
    }
}
