using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class ScoutInfoEntity : IGamePlay
    {
        public ScoutInfoEntity()
        {
            this.Type = GameType.ScoutInfo;
        }
        public GameType Type { get; private set; }
        public int ScoutInfoLength { get; set; }
        public long MatchId { get; set; }
        public DateTime Timestamp { get; set; }
        public ICollection<ScoutInfo> ScoutInfos { get; set; }

    }
}
