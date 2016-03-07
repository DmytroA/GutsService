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
    public class MatchUpdateDeltaUpdateEntity
    {
        public MatchUpdateDeltaUpdateEntity() 
        {
            this.Type = GameType.MatchUpdateDeltaUpdate;
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; }
        public MatchUpdate MatchUpdate { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
