namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomeraddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemberAddresses", "longitude", c => c.String(maxLength: 250));
            AddColumn("dbo.MemberAddresses", "latitude", c => c.String(maxLength: 250));
            AddColumn("dbo.MemberAddresses", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MemberAddresses", "IsDefault");
            DropColumn("dbo.MemberAddresses", "latitude");
            DropColumn("dbo.MemberAddresses", "longitude");
        }
    }
}
