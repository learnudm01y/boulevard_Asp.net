namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetablepropertyinformation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyInformations", "Exteriors", c => c.String());
            AddColumn("dbo.PropertyInformations", "Interiors", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyInformations", "Interiors");
            DropColumn("dbo.PropertyInformations", "Exteriors");
        }
    }
}
