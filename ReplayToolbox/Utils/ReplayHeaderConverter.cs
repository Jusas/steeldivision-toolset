using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReplayToolbox.Models;

namespace ReplayToolbox.Utils
{
    public class ReplayHeaderConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject o = JObject.Load(reader);
            ReplayHeader h = new ReplayHeader();
            serializer.Populate(o.CreateReader(), h);
            foreach (var prop in o.Properties())
            {
                if (prop.Name.StartsWith("player_"))
                {
                    var player = JsonConvert.DeserializeObject<Player>(prop.Value.ToString());
                    h.Players.Add(player);
                }
            }
            return h;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
