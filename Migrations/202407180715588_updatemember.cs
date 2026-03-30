namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberFirebases",
                c => new
                    {
                        MemberFirebaseId = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        FirebaseToken = c.String(maxLength: 250),
                        Status = c.String(maxLength: 50),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MemberFirebaseId);
            
            AddColumn("dbo.Members", "Name", c => c.String(maxLength: 150));
            AddColumn("dbo.Members", "PhoneNumber", c => c.String(maxLength: 50));
            AddColumn("dbo.Members", "PhoneCode", c => c.String(maxLength: 50));
            AddColumn("dbo.Members", "Password", c => c.String(maxLength: 150));
            AddColumn("dbo.Members", "Address", c => c.String(maxLength: 250));
            AddColumn("dbo.Members", "SecurityToken", c => c.String(maxLength: 150));
            AddColumn("dbo.Members", "ThirdPartyKey", c => c.String());
            AddColumn("dbo.Members", "OTPNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.Members", "Email", c => c.String(maxLength: 200));
            AlterColumn("dbo.Members", "Image", c => c.String());
            DropColumn("dbo.Members", "FirstName");
            DropColumn("dbo.Members", "LastName");
            DropColumn("dbo.Members", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "Phone", c => c.String(maxLength: 20));
            AddColumn("dbo.Members", "LastName", c => c.String(maxLength: 150));
            AddColumn("dbo.Members", "FirstName", c => c.String(maxLength: 150));
            AlterColumn("dbo.Members", "Image", c => c.String(maxLength: 180));
            AlterColumn("dbo.Members", "Email", c => c.String(maxLength: 250));
            DropColumn("dbo.Members", "OTPNumber");
            DropColumn("dbo.Members", "ThirdPartyKey");
            DropColumn("dbo.Members", "SecurityToken");
            DropColumn("dbo.Members", "Address");
            DropColumn("dbo.Members", "Password");
            DropColumn("dbo.Members", "PhoneCode");
            DropColumn("dbo.Members", "PhoneNumber");
            DropColumn("dbo.Members", "Name");
            DropTable("dbo.MemberFirebases");
        }
    }
}
