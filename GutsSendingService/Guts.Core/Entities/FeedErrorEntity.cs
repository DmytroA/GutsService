using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class FeedErrorEntity : IGamePlay
    {
        public FeedErrorEntity()
        {
            this.Type = GameType.FeedError;
        }
        public GameType Type { get; private set; }

        public string ErrorMessage { get; set; }
        public string Severity { get; set; }
        public string Cause { get; set; }
        public DateTime LocalStamp { get; set; }
    }
}
