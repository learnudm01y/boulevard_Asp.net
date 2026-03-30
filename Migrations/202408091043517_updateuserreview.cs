namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateuserreview : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserReviews", "Details", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserReviews", "Details");
        }
    }
}
