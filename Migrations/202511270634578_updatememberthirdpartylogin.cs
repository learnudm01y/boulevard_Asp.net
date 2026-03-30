namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatememberthirdpartylogin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "ThirdPartyLogin", c => c.Boolean(nullable: false));
            AddColumn("dbo.Members", "ThirdPartyLoginKey", c => c.String(maxLength: 250));
            AddColumn("dbo.Members", "ThirdPartyLoginFrom", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "ThirdPartyLoginFrom");
            DropColumn("dbo.Members", "ThirdPartyLoginKey");
            DropColumn("dbo.Members", "ThirdPartyLogin");
        }
    }
}
