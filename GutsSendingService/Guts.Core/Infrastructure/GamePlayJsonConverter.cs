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
            return typeof(LiveScoutEntity).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
                                        Type objectType,
                                         object existingValue,
                                         JsonSerializer serializer)
        {

            LiveScoutEntity target = new LiveScoutEntity();

            JObject jObject = JObject.Load(reader);
            serializer.Populate(jObject.CreateReader(), target);
            target.Json = new LiveScoutJsonEntity
            {
                Data = jObject.ToString()
            };
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
