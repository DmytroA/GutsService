using Guts.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reciever
{
    public class GamePlayDataContext : DbContext 
    {
        public GamePlayDataContext()
            : base ("GutsLiveScoutEntities")
        {
            
        }
        public DbSet<LineUpsEntity> LineUps { get; set; }
        public DbSet<MatchBookingReply> MatchBookingReplies { get; set; }
        public DbSet<MatchStopEntity> MatchStops { get; set; }
        public DbSet<MatchUpdateEntity> MatchUpdates { get; set; }
        public DbSet<MatchUpdateFullEntity> MatchUpdateFulls { get; set; }
        public DbSet<OddsSuggestionEntity> OddsSuggestions { get; set; }
        public DbSet<ScoutInfoEntity> ScoutInfos { get; set; }
    }
}
