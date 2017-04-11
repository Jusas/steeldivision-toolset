using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataExtractor.DataModels;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using UnitExtractor.DataModels;

namespace DataExtractor.Extractors
{
    public static class PackExtractor
    {
        public static object GetPacks(Configuration configuration, DataSource dataSource)
        {
            var source = dataSource.GetDataSource<NdfBinary>(configuration.DataMappings["DeckData"]);
            List<object> outputData = new List<object>();
            var output = new ExportedDataTable()
            {
                MetaData = new ExportedDataTable.MetaDataModel()
                {
                    DataIdentifier = "Packs",
                    ExtractedFrom = new[]
                    {
                        configuration.DataMappings["DeckData"],
                        configuration.DataMappings["LocalizationOutgame"]
                    }
                },
                Data = outputData
            };

            var serializer = source.Classes.First(x => x.Name == "TDeckSerializer");
            var instance = serializer.Instances.First();
            var packMap = instance.PropertyValues.First(x => x.Property.Name == "PackIds").Value as NdfMapList;
            for (var i = 0; i < packMap.Count; i++)
            {
                var pack = new PackModel();
                outputData.Add(pack);

                var kvMap = packMap[i].Value as NdfMap;
                pack.Id = (int)(UInt32)((kvMap.Value as MapValueHolder).Value as NdfUInt32).Value;

                var mapValue = kvMap.Key as MapValueHolder;
                var packRef = mapValue.Value as NdfObjectReference;
                var packInst = packRef.Instance;

                pack.FactoryType = (int)packInst.GetInstancePropertyValue<int>("FactoryType");
                pack.ExperienceLevel = (int) packInst.GetInstancePropertyValue<int>("ExperienceLevel");
                pack.AvailableFromPhase = (int)(UInt32) packInst.GetInstancePropertyValue<UInt32>("AvailableFromPhase");

                var tpUnitList = packInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "TransporterAndUnitsList")?.Value as
                    NdfCollection;

                if (tpUnitList != null)
                {
                    pack.TransportAndUnitCount = tpUnitList.Count;

                    var tpAndUnitsDescriptorRef = tpUnitList.FirstOrDefault()?.Value as NdfObjectReference;
                    var inst = tpAndUnitsDescriptorRef?.Instance;
                    if (inst != null)
                    {
                        var tpRef = inst.PropertyValues.FirstOrDefault(x => x.Property.Name == "TransporterDescriptor")?.Value as
                            NdfObjectReference;
                        var tpInst = tpRef?.Instance;
                        if (tpInst != null)
                        {
                            pack.TransportDescriptorId =
                            (tpInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "DescriptorId")?.Value as
                                NdfGuid)?.Value.ToString();
                        }

                        var unitList = inst.PropertyValues.FirstOrDefault(x => x.Property.Name == "UnitDescriptorList")?.Value as NdfCollection;
                        var unitRef = unitList?.FirstOrDefault()?.Value as NdfObjectReference;
                        var unitInst = unitRef?.Instance;
                        if (unitInst != null)
                        {
                            pack.UnitDescriptorId =
                            (unitInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "DescriptorId")?.Value
                                as NdfGuid)?.Value.ToString();
                        }
                    }

                    

                }

            }

            return output;
        }
    }
}
