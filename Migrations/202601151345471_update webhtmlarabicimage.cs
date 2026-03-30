namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatewebhtmlarabicimage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebHtmls", "PictureOneAr", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebHtmls", "PictureOneAr");
        }
    }
}
