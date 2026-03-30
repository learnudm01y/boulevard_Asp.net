namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecategorylabel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "label", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "label");
        }
    }
}
