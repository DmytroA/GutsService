using Guts.Core.Entities;
using Sportradar.SDK.FeedProviders.Common;
using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchUpdateEntity : IGamePlay
    {
        public MatchUpdateEntity()
        {
            this.Type = GameType.MatchUpdate;
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; }
        public MatchUpdate MatchUpdate { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
