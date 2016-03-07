using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchStopEntity : IGamePlay
    {
        public MatchStopEntity()
        {
            this.Type = GameType.MatchStop;
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }



    }
}
