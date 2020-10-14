using mod_add.Datos.Modelos;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
        public DbSet<RegistroBitacora> RegistrosBitacora { get; set; }
        public DbSet<ProductoEliminar> ProductosEliminar { get; set; }
        public DbSet<ProductoReemplazo> ProductosReemplazo { get; set; }

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
