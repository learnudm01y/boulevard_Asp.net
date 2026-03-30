namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatememberthirdpartyloginagain : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "ThirdPartyLoginKey", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "ThirdPartyLoginKey", c => c.String(maxLength: 250));
        }
    }
}
