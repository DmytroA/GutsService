using Guts.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Infrastructure
{
    public class GamePlayJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IGamePlay).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
                                        Type objectType,
                                         object existingValue,
                                         JsonSerializer serializer)
        {

            IGamePlay target = null;
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);
            var prop = jObject.Property("Type");
            var type = (GameType)Enum.Parse(typeof(GameType), prop.Value.ToString());
            switch (type)
            { 
                case GameType.LineUps:
                    target = new LineUpsEntity();
                    break;
                case GameType.MatchBookingReply:
                    target = new MatchBookingReply();
                    break;
                case GameType.MatchStop: 
                    target = new MatchStopEntity();
                    break;
                case GameType.MatchUpdate:
                    target = new MatchUpdateEntity();
                    break;
                case GameType.MatchUpdateFull:
                    target = new MatchUpdateFullEntity();
                    break;
                case GameType.OddsSuggestion:
                    target = new OddsSuggestionEntity();
                    break;
                case GameType.ScoutInfo:
                    target = new ScoutInfoEntity();
                    break;
            }
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }


        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        public override void WriteJson(JsonWriter writer,
                                       object value,
                                       JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
