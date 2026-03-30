namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatewebhtml : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.WebHtmls", "PageIdentifier");
            DropColumn("dbo.WebHtmls", "ArabicBigDetailsOne");
            DropColumn("dbo.WebHtmls", "DeletedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebHtmls", "DeletedDate", c => c.DateTime());
            AddColumn("dbo.WebHtmls", "ArabicBigDetailsOne", c => c.String());
            AddColumn("dbo.WebHtmls", "PageIdentifier", c => c.String(maxLength: 100));
        }
    }
}
