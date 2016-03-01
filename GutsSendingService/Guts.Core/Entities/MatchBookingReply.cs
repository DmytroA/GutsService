using System;
using System.Collections.Generic;
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

        public int Id { get; set; }
        public Nullable<long> MatchId { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }

        public GameType Type { get; private set; }
    }
}
