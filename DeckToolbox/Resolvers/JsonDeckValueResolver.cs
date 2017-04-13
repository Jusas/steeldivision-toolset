using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckToolbox.Utils;
using Newtonsoft.Json.Linq;

namespace DeckToolbox.Resolvers
{
    public class JsonDeckValueResolver : JsonSourceValueResolver, IDeckValueResolver
    {
        public JsonDeckValueResolver(IJsonFileReader reader) : base(reader)
        {
        }

        public JsonDeckValueResolver(IJsonFileReader reader, IList<string> jsonSourceFiles) : base(reader, jsonSourceFiles)
        {
        }

        public string GetDivisionName(int divisionId)
        {
            var source = _dataSources["Divisions"];
            var name = source.FirstOrDefault(x => x["Id"].ToString() == divisionId.ToString())?["DivisionName"].ToString();
            return name;
        }

        public string GetDeckPackName(int deckPackId)
        {
            var packSource = _dataSources["Packs"];
            var descriptorId = packSource.FirstOrDefault(x => x["Id"].ToString() == deckPackId.ToString())?["UnitDescriptorId"].ToString();
            var tpDescriptorId = packSource.FirstOrDefault(x => x["Id"].ToString() == deckPackId.ToString())?["TransportDescriptorId"].ToString();

            var unitSource = _dataSources["Units"];
            var unitName = descriptorId != null ? unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["NameInMenu"].ToString() 
                : "unknown";
            var tpName = tpDescriptorId != null ? unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == tpDescriptorId)?["NameInMenu"].ToString()
                : "";
            string name = tpName != "" ? $"{unitName} + {tpName}" : unitName;

            return name;
        }

        public int[] GetPhaseIncomes(int divisionId)
        {
            var source = _dataSources["Divisions"];
            var incomeValues = source.FirstOrDefault(x => x["Id"].ToString() == divisionId.ToString())?["PhaseIncome"];
            var values = incomeValues?.Values<int>().ToArray();
            return values;
        }

        public int GetNationality(int divisionId)
        {
            var source = _dataSources["Divisions"];
            var nationality = source.FirstOrDefault(x => x["Id"].ToString() == divisionId.ToString())?["Nationality"].Value<int>();
            return nationality ?? -1;
        }
    }
}
