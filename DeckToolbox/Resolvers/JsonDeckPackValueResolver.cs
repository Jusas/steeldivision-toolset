using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckToolbox.Utils;
using Newtonsoft.Json.Linq;

namespace DeckToolbox.Resolvers
{
    public class JsonDeckPackValueResolver : JsonSourceValueResolver, IDeckPackValueResolver
    {
        public JsonDeckPackValueResolver(IJsonFileReader reader) : base(reader)
        {
        }

        public JsonDeckPackValueResolver(IJsonFileReader reader, IList<string> jsonSourceFiles) : base(reader, jsonSourceFiles)
        {
        }

        public int GetAvailablePhase(int packId)
        {
            var source = _dataSources["Packs"];
            var phase = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["AvailableFromPhase"].Value<int>();
            return phase ?? -1;
        }

        public int GetFactoryType(int packId)
        {
            var source = _dataSources["Packs"];
            var fac = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["FactoryType"].Value<int>();
            return fac ?? -1;
        }

        public int GetExperienceLevel(int packId)
        {
            var source = _dataSources["Packs"];
            var xp = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["ExperienceLevel"].Value<int>();
            return xp ?? -1;
        }

        public string GetTransportName(int packId)
        {
            var source = _dataSources["Packs"];
            var tpDescriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["TransportDescriptorId"].ToString();

            if (tpDescriptorId == null)
                return "unknown";

            var unitSource = _dataSources["Units"];
            var tpName = unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == tpDescriptorId)?["NameInMenu"].ToString();
            return tpName;
        }

        public string GetUnitName(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return "unknown";

            var unitSource = _dataSources["Units"];
            var name = unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["NameInMenu"].ToString();
            return name;
        }

        public int GetUnitCount(int packId)
        {
            var source = _dataSources["Packs"];
            var c = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["TransportAndUnitCount"].Value<int>();
            return c ?? -1;
        }

        public string GetFactoryName(int packId)
        {
            var source = _dataSources["Packs"];
            var fac = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["FactoryType"].Value<int>();
            if (fac == null)
                return "unknown";

            return GetFactoryNameForFactoryId(fac.Value);
        }

        private string GetFactoryNameForFactoryId(int fac)
        {
            // Hardcoded...
            switch (fac)
            {
                case 3:
                    return "Infantry";
                case 4:
                    return "Aircraft";
                case 6:
                    return "Armor";
                case 7:
                    return "Recon";
                case 10:
                    return "Support";
                case 13:
                    return "Anti-Air";
                case 14:
                    return "Artillery";
                case 15:
                    return "Anti-Tank";
                default:
                    return "unknown";
            }
        }

        public bool GetIsRecon(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return false;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["ResolvedIsRecon"].Value<bool>() ?? false;
        }

        public bool GetIsCommand(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return false;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["ResolvedIsCommand"].Value<bool>() ?? false;
        }

        public int GetTransportPrice(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["TransportDescriptorId"].ToString();

            if (descriptorId == null)
                return 0;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["ProductionPrice"].Value<int>() ?? 0;
        }

        public int GetUnitPrice(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return 0;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["ProductionPrice"].Value<int>() ?? 0;
        }

        public string[] GetValidPackFactoryNames()
        {
            var factoryIds = new int[] {3, 4, 6, 7, 10, 13, 14, 15};
            return factoryIds.Select(x => GetFactoryNameForFactoryId(x)).ToArray();
        }

        public bool GetIsObserver(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return false;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["ResolvedIsObserver"].Value<bool>() ?? false;
        }

        public bool GetMainUnitHasWeapons(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return false;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["HasWeapons"].Value<bool>() ?? false;
        }

        public int GetMainUnitFrontalArmor(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return 0;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["FrontalArmor"].Value<int>() ?? 0;
        }

        public int GetMainUnitTurretArcAngle(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return 0;

            var unitSource = _dataSources["Units"];
            return (int)(unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["FrontalArmor"].Value<double>() ?? 0);
        }

        public int GetMainUnitAp(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return 0;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["MaxApDamage"].Value<int>() ?? 0;
        }

        public int GetMainUnitHe(int packId)
        {
            var source = _dataSources["Packs"];
            var descriptorId = source.FirstOrDefault(x => x["Id"].ToString() == packId.ToString())?["UnitDescriptorId"].ToString();

            if (descriptorId == null)
                return 0;

            var unitSource = _dataSources["Units"];
            return unitSource.FirstOrDefault(x => x["DescriptorId"].ToString() == descriptorId)?["MaxHeDamage"].Value<int>() ?? 0;
        }
    }
}
