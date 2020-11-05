namespace mod_add.Datos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configuracion_Sistema",
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
                "dbo.Productos_Eliminacion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Clave = c.String(),
                        Eliminar = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Productos_Reemplazo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reemplazar = c.Boolean(nullable: false),
                        Clave = c.String(),
                        Porcentaje = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Registro_Licencias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Anio = c.Int(nullable: false),
                        Mes = c.Int(nullable: false),
                        Licencia = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Registro_Licencias");
            DropTable("dbo.Productos_Reemplazo");
            DropTable("dbo.Productos_Eliminacion");
            DropTable("dbo.Configuracion_Sistema");
        }
    }
}
