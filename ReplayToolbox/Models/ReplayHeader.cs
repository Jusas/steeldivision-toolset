using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ReplayToolbox.Utils;

namespace ReplayToolbox.Models
{
    [JsonConverter(typeof(ReplayHeaderConverter))]
    public class ReplayHeader
    {
        public Game Game { get; set; }
        public List<Player> Players { get; set; }

        public ReplayHeader()
        {
            Players = new List<Player>();
        }
    }
}
