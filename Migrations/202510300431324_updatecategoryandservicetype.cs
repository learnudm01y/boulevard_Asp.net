namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecategoryandservicetype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTypes", "BigDescription", c => c.String());
            AddColumn("dbo.ServiceTypes", "BigDescriptionAr", c => c.String());
            AddColumn("dbo.Categories", "Icon", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "Icon");
            DropColumn("dbo.ServiceTypes", "BigDescriptionAr");
            DropColumn("dbo.ServiceTypes", "BigDescription");
        }
    }
}
