namespace Reciever.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeNameEventType : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LiveScoutEventTypy", newName: "LiveScoutEventType");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.LiveScoutEventType", newName: "LiveScoutEventTypy");
        }
    }
}
