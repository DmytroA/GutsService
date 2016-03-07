using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchBookingReply : IGamePlay
    {
        public MatchBookingReply()
        {
            this.Type = GameType.MatchBookingReply;
        }
        public GameType Type { get; private set; }
        public Nullable<long> MatchId { get; set; }
        public string Message { get; set; }
        public BookMatchResult Result { get; set; }
        public DateTime Timestamp { get; set; }
        public IDictionary<string, string> AdditionalData { get; set; }
    }
}
