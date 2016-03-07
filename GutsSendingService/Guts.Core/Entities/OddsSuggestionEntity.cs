using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace Guts.Core.Entities
{
    public class OddsSuggestionEntity : IGamePlay
    {
        public OddsSuggestionEntity()
        {
            this.Type = GameType.OddsSuggestion;            
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; } 
        public int OddLength { get; set; }
        public DateTime Timestamp { get; set; }
        [NotMapped]
        public ICollection<ScoutOdds> Odds { get; set; }

    }
}
