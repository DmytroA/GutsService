using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchListUpdateEntity : IGamePlay
    {
        public MatchListUpdateEntity()
        {
            this.Type = GameType.MatchList;
        }
        public GameType Type { get; private set; }
        public MatchUpdate[] MatchList { get; set; }
        public bool WasRequested { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
