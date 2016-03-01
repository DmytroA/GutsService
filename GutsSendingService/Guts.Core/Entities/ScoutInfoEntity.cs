using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class ScoutInfoEntity : IGamePlay
    {
        public ScoutInfoEntity()
        {
            this.Type = GameType.ScoutInfo;
        }
        public int Id { get; set; }
        public int ScoutInfoLength { get; set; }
        public long MatchId { get; set; }

        public GameType Type { get; private set; }
    }
}
