using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchDataEntity :IGamePlay
    {
        public MatchDataEntity()
        {
            this.Type = GameType.MatchData;
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; }
        public string MatchTime { get; set; }
        public string RemainingTimeInPeriod { get; set; }
        public IDictionary<string, string> AdditionalData { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
