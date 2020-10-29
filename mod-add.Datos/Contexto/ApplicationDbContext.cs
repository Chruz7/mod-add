using mod_add.Datos.Modelos;
using System.Data.Entity;
using System.Threading.Tasks;

namespace mod_add.Datos.Contexto
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("Connection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
        }

        public DbSet<ConfiguracionSistema> ConfiguracionSistema { get; set; }
        public DbSet<BitacoraModificacion> RegistrosBitacora { get; set; }
        public DbSet<ProductoEliminacion> ProductosEliminar { get; set; }
        public DbSet<ProductoReemplazo> ProductosReemplazo { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Cheque> Cheques { get; set; }
        public DbSet<ChequeDetalle> ChequesDetalle { get; set; }
        public DbSet<ChequePago> ChequesPago { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public override Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
