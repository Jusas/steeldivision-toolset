using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ReplayToolbox.Utils;

namespace ReplayToolbox.Models
{
    public class Player
    {
        public string PlayerUserId { get; set; }
        public double PlayerElo { get; set; }
        public int PlayerRank { get; set; }
        public int PlayerLevel { get; set; }
        public string PlayerName { get; set; }
        public string PlayerTeamName { get; set; }
        public string PlayerAvatar { get; set; }
        public string PlayerIALevel { get; set; }
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public bool PlayerReady { get; set; }
        public string PlayerDeckName { get; set; }
        public string PlayerDeckContent { get; set; }
        public int PlayerAlliance { get; set; }
        [JsonConverter(typeof(StringToBooleanJsonConverter))]
        public bool PlayerIsEnteredInLobby { get; set; }
        public int PlayerScoreLimit { get; set; }
        public int PlayerIncomeRate { get; set; }
    }
}
