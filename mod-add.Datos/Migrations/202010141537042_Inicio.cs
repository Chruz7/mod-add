namespace mod_add.Datos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfiguracionSistemas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModificarVentasReales = c.Boolean(nullable: false),
                        MinProductosCuenta = c.Int(nullable: false),
                        EliminarProductosSeleccionados = c.Boolean(nullable: false),
                        Contrasena = c.String(),
                        ContrasenaAdmin = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductoEliminars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Clave = c.String(),
                        Eliminar = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductoReemplazoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reemplazar = c.Boolean(nullable: false),
                        Clave = c.String(),
                        Porcentaje = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RegistroBitacoras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FechaProceso = c.DateTime(nullable: false),
                        FechaInicialVenta = c.DateTime(nullable: false),
                        FechaFinalVenta = c.DateTime(nullable: false),
                        TotalCuentas = c.Int(nullable: false),
                        CuentasModificadas = c.Int(nullable: false),
                        ImporteAnterior = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImporteNuevo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Diferencia = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RegistroBitacoras");
            DropTable("dbo.ProductoReemplazoes");
            DropTable("dbo.ProductoEliminars");
            DropTable("dbo.ConfiguracionSistemas");
        }
    }
}
