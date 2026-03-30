namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatevehicalmember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemberVehicalInfoes", "MemberId", c => c.Long(nullable: false));
            CreateIndex("dbo.MemberVehicalInfoes", "MemberId");
            AddForeignKey("dbo.MemberVehicalInfoes", "MemberId", "dbo.Members", "MemberId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberVehicalInfoes", "MemberId", "dbo.Members");
            DropIndex("dbo.MemberVehicalInfoes", new[] { "MemberId" });
            DropColumn("dbo.MemberVehicalInfoes", "MemberId");
        }
    }
}
