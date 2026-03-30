namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatescategorytablev2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "IsTop", c => c.Boolean());
            AddColumn("dbo.Categories", "IsTrenbding", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "IsTrenbding");
            DropColumn("dbo.Categories", "IsTop");
        }
    }
}
