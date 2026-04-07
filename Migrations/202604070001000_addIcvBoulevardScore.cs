namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addIcvBoulevardScore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IcvBoulevardScore", c => c.String(maxLength: 50));
            AddColumn("dbo.TempProducts", "IcvBoulevardScore", c => c.String(maxLength: 50));
        }

        public override void Down()
        {
            DropColumn("dbo.TempProducts", "IcvBoulevardScore");
            DropColumn("dbo.Products", "IcvBoulevardScore");
        }
    }
}
