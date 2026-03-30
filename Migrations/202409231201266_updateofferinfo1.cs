namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateofferinfo1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferInformations", "IsTimeLimit", c => c.Boolean(nullable: false));
            AddColumn("dbo.OfferInformations", "IsTrending", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfferInformations", "IsTrending");
            DropColumn("dbo.OfferInformations", "IsTimeLimit");
        }
    }
}
