using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        public long MatchId { get; set; }
        public string Reason { get; set; }
        public GameType Type { get; private set; }


    }
}
