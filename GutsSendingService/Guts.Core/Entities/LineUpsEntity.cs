using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class LineUpsEntity : IGamePlay
    {
        public LineUpsEntity()
        {
            this.Type = GameType.LineUps;
        }
        public int Id { get; set; }
        public long MatchId { get; set; }
        public int PlayersCount { get; set; }
        public int ManagersCount { get; set; }
        public int TeamOfficals { get; set; }

        public GameType Type { get; private set; }
    }
}
