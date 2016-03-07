namespace Reciever.Migrations
{
    using Guts.Core.Entities;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiveScoutEventTypy",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EventType = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.LiveScoutRawData",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Data = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.LiveScout",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    LocalStamp = c.DateTime(nullable: false),
                    LiveScoutJsonId = c.Long(nullable: false),
                    EventTypeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LiveScoutEventTypy", t => t.EventTypeId, cascadeDelete: true)
                .ForeignKey("dbo.LiveScoutRawData", t => t.LiveScoutJsonId, cascadeDelete: true)
                .Index(t => t.LiveScoutJsonId)
                .Index(t => t.EventTypeId);
            Sql("SET IDENTITY_INSERT LiveScoutEventTypy ON");
            Sql(FillEventTypeTable());
            Sql("SET IDENTITY_INSERT LiveScoutEventTypy OFF");
        }

        public string FillEventTypeTable()
        {
            return Enum.GetValues(typeof(GameType)).Cast<GameType>().Select(s =>
                string.Format("INSERT INTO LiveScoutEventTypy(Id,EventType) values({0},'{1}');", (int)s, s.ToString())).Aggregate((x, y) => x + y);
        }

        public override void Down()
        {
            DropForeignKey("dbo.LiveScout", "LiveScoutJsonId", "dbo.LiveScoutRawData");
            DropForeignKey("dbo.LiveScout", "EventTypeId", "dbo.LiveScoutEventTypy");
            DropIndex("dbo.LiveScout", new[] { "EventTypeId" });
            DropIndex("dbo.LiveScout", new[] { "LiveScoutJsonId" });
            DropTable("dbo.LiveScout");
            DropTable("dbo.LiveScoutRawData");
            DropTable("dbo.LiveScoutEventTypy");
        }
    }
}

