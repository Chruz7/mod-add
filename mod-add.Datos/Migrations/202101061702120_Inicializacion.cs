namespace mod_add.Datos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicializacion : DbMigration
    {
        public override void Up()
        {
            Sql($"INSERT INTO dbo.Configuracion_Sistema VALUES({0}, {1}, {0}, '25D55AD283AA400AF464C76D713C07AD', '3F6F7549DB51EAB5A1F6E1DCB95D1479')");

            for (int i = 0; i < 5; i++)
            {
                Sql($"INSERT INTO dbo.Productos_Reemplazo VALUES({0}, '', {0})");
            }
        }

        public override void Down()
        {
        }
    }
}
