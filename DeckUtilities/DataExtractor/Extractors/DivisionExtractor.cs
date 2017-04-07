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
    public static class DivisionExtractor
    {
        public static object GetDivisions(Configuration configuration, DataSource dataSource)
        {
            var source = dataSource.GetDataSource<NdfBinary>(configuration.DataMappings["DeckData"]);
            List<object> outputData = new List<object>();
            var output = new ExportedDataTable()
            {
                MetaData = new ExportedDataTable.MetaDataModel()
                {
                    DataIdentifier = "Divisions",
                    ExtractedFrom = new[]
                    {
                        configuration.DataMappings["DeckData"],
                        configuration.DataMappings["LocalizationDeck"]
                    }
                },
                Data = outputData
            };

            var serializer = source.Classes.First(x => x.Name == "TDeckSerializer");
            var instance = serializer.Instances.First();
            var divisionMap = instance.PropertyValues.First(x => x.Property.Name == "DivisionIds").Value as NdfMapList;
            for (var i = 0; i < divisionMap.Count; i++)
            {
                var division = new DivisionModel();
                outputData.Add(division);

                var kvMap = divisionMap[i].Value as NdfMap;
                division.Id = i;

                var mapValue = kvMap.Key as MapValueHolder;
                var divRef = mapValue.Value as NdfObjectReference;
                var divInst = divRef.Instance;

                division.DivisionName = dataSource.GetLocalizedString(divInst.GetInstancePropertyValue<string>("DivisionName") as string,
                    configuration.DataMappings["LocalizationDeck"]);

                division.DivisionNickName = dataSource.GetLocalizedString(divInst.GetInstancePropertyValue<string>("DivisionNickName") as string,
                    configuration.DataMappings["LocalizationDeck"]);

                var phaseList = divInst.PropertyValues.First(x => x.Property.Name == "PhaseList").Value as NdfCollection;

                List<int> incomes = new List<int>();
                for (var j = 0; j < phaseList.Count; j++)
                {
                    var phaseRef = phaseList[j].Value as NdfObjectReference;
                    var phaseInst = phaseRef.Instance;
                    var income = (UInt32)phaseInst.GetInstancePropertyValue<UInt32>("PhaseIncome");
                    incomes.Add((int)income);
                }

                division.PhaseIncome = incomes.ToArray();

            }

            return output;
        }
    }
}
