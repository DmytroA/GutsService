using Guts.Core.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameType {
        LineUps,
        MatchBookingReply,
        MatchStop,
        MatchUpdate,
        MatchUpdateFull,
        OddsSuggestion,
        ScoutInfo
    }

    [JsonConverter(typeof(GamePlayJsonConverter))]
    public interface IGamePlay
    {
        GameType Type { get;  }
    }
}
