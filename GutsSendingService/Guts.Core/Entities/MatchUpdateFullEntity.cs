using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchUpdateFullEntity : IGamePlay
    {
        public MatchUpdateFullEntity() 
        {
            this.Type = GameType.MatchUpdateFull;
        }
        public int Id { get; set; }
        public long MatchId { get; set; }
        public int AttacksTeam1 { get; set; }
        public int AttackTeam2 { get; set; }
        public int RedCardsTeam1 { get; set; }
        public int RedCardsTeam2 { get; set; }
        public Nullable<int> PossessionTeam1 { get; set; }
        public Nullable<int> PossessionTeam2 { get; set; }

        public GameType Type { get; private set; }
    }
}
