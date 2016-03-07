using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class ClosedHandlerEntity : IGamePlay
    {
        public ClosedHandlerEntity()
        {
            this.Type = GameType.ClosedHandler;
        }
        public DateTime Timestamp { get; set; }
        public GameType Type { get; private set; }
    }
}
