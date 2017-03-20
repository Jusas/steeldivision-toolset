using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ReplayToolbox.Utils;

namespace ReplayToolbox.Models
{
    public class Game
    {
        public int GameMode { get; set; }
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public bool IsNetworkMode { get; set; }
        public int NbMaxPlayer { get; set; }
        public string Seed { get; set; } 
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public bool Private { get; set; }
        public string ServerName { get; set; }
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public string Version { get; set; }
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public object GameType { get; set; }
        public string Map { get; set; }
        public int InitMoney { get; set; }
        public int TimeLimit { get; set; }
        public int ScoreLimit { get; set; }
        public int NbAI { get; set; }
        public string VictoryCond { get; set; }
        public int IncomeRate { get; set; }
    }
}
