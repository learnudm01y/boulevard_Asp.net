namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatememberaddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemberAddresses", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MemberAddresses", "Type");
        }
    }
}
