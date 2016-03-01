using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class OddsSuggestionEntity : IGamePlay
    {
        public OddsSuggestionEntity()
        {
            this.Type = GameType.OddsSuggestion;
        }
        public int Id { get; set; }
        public long MatchId { get; set; } 
        public int OddLength { get; set; }

        public GameType Type { get; private set; }
    }
}
