using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class LineUpsEntity : IGamePlay
    {
        public LineUpsEntity()
        {
            this.Type = GameType.LineUps;
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; }
        public List<Manager> Managers { get; set; }
        public List<Player> Players { get; set; }
        public List<TeamOffical> TeamOfficials { get; set; }
        public IDictionary<string, string> AdditionalData { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
