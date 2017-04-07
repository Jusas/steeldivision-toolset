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
    public static class ModularUnitExtractor
    {
        public static object GetUnitData(Configuration configuration, DataSource dataSource)
        {
            var source = dataSource.GetDataSource<NdfBinary>(configuration.DataMappings["DeckData"]);
            var localizationsFile = configuration.DataMappings["LocalizationDeck"];

            List<object> outputData = new List<object>();
            var output = new ExportedDataTable()
            {
                MetaData = new ExportedDataTable.MetaDataModel()
                {
                    DataIdentifier = "Units",
                    ExtractedFrom = new[]
                    {
                        configuration.DataMappings["DeckData"],
                        configuration.DataMappings["LocalizationDeck"]
                    }
                },
                Data = outputData
            };

            var unitDescriptorTable = source.Classes.First(x => x.Name == "TModularUnitDescriptor");

            foreach (var instance in unitDescriptorTable.Instances)
            {
                var unit = new UnitModel();
                outputData.Add(unit);
                unit.Id = (int)instance.Id;

                unit.DescriptorId =
                    (instance.PropertyValues.FirstOrDefault(x => x.Property.Name == "DescriptorId")?.Value as NdfGuid)?.Value
                    .ToString();

                var modules = instance.PropertyValues.First(x => x.Property.Name == "Modules").Value as NdfCollection;
                for (var i = 0; i < modules.Count; i++)
                {
                    var modRef = modules[i].Value as NdfObjectReference;
                    var modInst = modRef.Instance;
                    var controllerName = (int)modInst.GetInstancePropertyValue<int>("ControllerName");

                    switch (controllerName)
                    {
                        case 148:
                            NdfObjectReference typeUnitModuleDescriptorRef;

                            if (modRef.Class.Name == "TTypeUnitModuleDescriptor")
                                typeUnitModuleDescriptorRef = modRef;
                            else
                                typeUnitModuleDescriptorRef = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "Default")?.Value
                                    as NdfObjectReference;
                            
                            var tumd = typeUnitModuleDescriptorRef?.Instance;
                            if (tumd != null)
                            {
                                unit.TypeUnitHint = dataSource.GetLocalizedString(
                                    (string)tumd.GetInstancePropertyValue<string>("TypeUnitHintToken"),
                                    localizationsFile);

                                unit.AliasName = (string)tumd.GetInstancePropertyValue<string>("AliasName");
                                unit.Category = (int)tumd.GetInstancePropertyValue<int>("Category");
                                unit.NameInMenu = dataSource.GetLocalizedString(
                                    (string)tumd.GetInstancePropertyValue<string>("NameInMenuToken"),
                                    localizationsFile);
                                unit.MotherCountry = (string)tumd.GetInstancePropertyValue<string>("MotherCountry");

                            }
                            break;

                        case 103:
                            NdfObjectReference productionModuleDescriptorRef;
                            if (modRef.Class.Name == "TModernWarfareProductionModuleDescriptor")
                                productionModuleDescriptorRef = modRef;
                            else
                                productionModuleDescriptorRef = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "Default")?.Value
                                    as NdfObjectReference;

                            var pmd = productionModuleDescriptorRef?.Instance;
                            if (pmd != null)
                            {
                                unit.Factory = (int)pmd.GetInstancePropertyValue<int>("Factory");
                                unit.ProductionPrice = (int) pmd.GetInstancePropertyValue<int>("ProductionPrice");
                            }
                            break;

                        case 173:
                            NdfObjectReference cmdModuleDescriptorRef;
                            if (modRef.Class.Name == "TCommandManagerModuleDescriptor")
                                cmdModuleDescriptorRef = modRef;
                            else
                                cmdModuleDescriptorRef = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "Default")?.Value
                                    as NdfObjectReference;

                            var cmd = cmdModuleDescriptorRef?.Instance;
                            if (cmd != null)
                            {
                                var isCommand = cmd.PropertyValues.First(x => x.Property.Name == "GiveMoraleBonusToSurroundingUnits").Value as NdfBoolean;
                                if (isCommand != null && Convert.ToBoolean(isCommand.Value))
                                {
                                    unit.ResolvedIsCommand = true;
                                }
                            }
                            break;

                        case 108:
                            NdfObjectReference recModuleDescriptorRef;
                            if (modRef.Class.Name == "TScannerConfigurationDescriptor")
                                recModuleDescriptorRef = modRef;
                            else
                                recModuleDescriptorRef = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "Default")?.Value
                                    as NdfObjectReference;

                            var rmd = recModuleDescriptorRef?.Instance;
                            if (rmd != null)
                            {
                                var opticalStrength = rmd.PropertyValues.First(x => x.Property.Name == "OpticalStrength").Value as NdfSingle;
                                if (opticalStrength != null && opticalStrength.Value > 120)
                                {
                                    unit.ResolvedIsRecon = true;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }

            }
            
            return output;
        }
    }
}
