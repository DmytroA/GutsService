using Guts.Core.Entities;
using System;
using System.Collections.Generic;
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
        public int Id { get; set; }
        public long MatchId { get; set; }

        public string BetStatus { get; set; }
        
        public GameType Type { get; private set; }
    }
}
