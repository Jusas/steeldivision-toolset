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
                    NdfObject modInst;
                    var controllerName = (int)(modRef?.Instance?.GetInstancePropertyValue<int>("ControllerName") ?? 0);

                    switch (controllerName)
                    {
                        case 148:
                            modInst = TryGetReference("TTypeUnitModuleDescriptor", modRef)?.Instance;
                            
                            if (modInst != null)
                            {
                                unit.TypeUnitHint = dataSource.GetLocalizedString(
                                    (string)modInst.GetInstancePropertyValue<string>("TypeUnitHintToken"),
                                    localizationsFile);

                                unit.AliasName = (string)modInst.GetInstancePropertyValue<string>("AliasName");
                                unit.Category = (int)modInst.GetInstancePropertyValue<int>("Category");
                                unit.NameInMenu = dataSource.GetLocalizedString(
                                    (string)modInst.GetInstancePropertyValue<string>("NameInMenuToken"),
                                    localizationsFile);
                                unit.MotherCountry = (string)modInst.GetInstancePropertyValue<string>("MotherCountry");

                            }
                            break;

                        case 103:
                            modInst = TryGetReference("TModernWarfareProductionModuleDescriptor", modRef)?.Instance;
                            
                            if (modInst != null)
                            {
                                unit.Factory = (int)modInst.GetInstancePropertyValue<int>("Factory");
                                unit.ProductionPrice = (int)modInst.GetInstancePropertyValue<int>("ProductionPrice");
                            }
                            break;

                        case 173:
                            modInst = TryGetReference("TCommandManagerModuleDescriptor", modRef)?.Instance;

                            var isCommand = modInst?.PropertyValues.First(x => x.Property.Name == "GiveMoraleBonusToSurroundingUnits").Value as NdfBoolean;
                            if (isCommand != null && Convert.ToBoolean(isCommand.Value))
                            {
                                unit.ResolvedIsCommand = true;
                            }
                            break;

                        case 108:
                            modInst = TryGetReference("TScannerConfigurationDescriptor", modRef)?.Instance;

                            var opticalStrength = modInst?.PropertyValues.First(x => x.Property.Name == "OpticalStrength").Value as NdfSingle;
                            if (opticalStrength != null && opticalStrength.Value > 120)
                            {
                                unit.ResolvedIsRecon = true;
                            }
                            break;

                        case 32:
                            modInst = TryGetReference("TModernWarfareDamageModuleDescriptor", modRef)?.Instance;

                            if (modInst != null)
                            {
                                var cddRef = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "CommonDamageDescriptor")?.Value as
                                    NdfObjectReference;
                                var armorPropsRef = cddRef?.Instance.PropertyValues.FirstOrDefault(
                                    x => x.Property.Name == "BlindageProperties")?.Value as NdfObjectReference;
                                var frontArmorRef =
                                    armorPropsRef?.Instance.PropertyValues.FirstOrDefault(
                                        x => x.Property.Name == "ArmorDescriptorFront")?.Value as NdfObjectReference;
                                var frontArmorValue =
                                    (int)(frontArmorRef?.Instance?.GetInstancePropertyValue<int>("BaseBlindage") ?? 0);

                                unit.FrontalArmor = frontArmorValue;
                            }
                            break;

                        case 157:
                            modInst = TryGetReference("TWeaponManagerModuleDescriptor_Wargame", modRef)?.Instance;

                            if (modInst != null)
                            {
                                var tdl = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "TurretDescriptorList")?.Value as
                                    NdfCollection;

                                int biggestWeaponApValue = 0;
                                int biggestWeaponHeValue = 0;
                                double biggestWeaponRange = 0;
                                bool hasWeapon = false;
                                NdfObjectReference mainTurret = null;
                                NdfObjectReference potentialMainTurret = null;
                                foreach (var turret in tdl)
                                {
                                    var turretDescriptor = turret.Value as NdfObjectReference;
                                    if (turretDescriptor == null)
                                        continue;

                                    if (mainTurret == null)
                                        mainTurret = turretDescriptor;

                                    var mountedWeaponList =
                                        turretDescriptor.Instance.PropertyValues.FirstOrDefault(
                                                x => x.Property.Name == "MountedWeaponDescriptorList")?.Value as
                                            NdfCollection;

                                    if(mountedWeaponList == null)
                                        continue;

                                    foreach (var weapon in mountedWeaponList)
                                    {
                                        var weaponDescriptor = weapon.Value as NdfObjectReference;
                                        if (weaponDescriptor == null)
                                            continue;

                                        hasWeapon = true;
                                        var ammoRef =
                                            weaponDescriptor.Instance.PropertyValues.FirstOrDefault(
                                                x => x.Property.Name == "Ammunition")?.Value as NdfObjectReference;
                                        var damageValue =
                                            (int) (ammoRef?.Instance.GetInstancePropertyValue<int>("Arme") ?? 0);
                                        var isAp =
                                            (bool)
                                            (ammoRef?.Instance.GetInstancePropertyValue<bool>("PiercingWeapon") ?? false);
                                        var weaponRange =
                                            (double)
                                            (ammoRef?.Instance.GetInstancePropertyValue<double>("PorteeMaximale") ?? 0);

                                        if (isAp && damageValue > biggestWeaponApValue)
                                        {
                                            mainTurret = turretDescriptor;
                                            biggestWeaponApValue = damageValue;
                                        }
                                        if (!isAp && damageValue > biggestWeaponHeValue)
                                        {
                                            biggestWeaponHeValue = damageValue;
                                            potentialMainTurret = turretDescriptor;
                                        }
                                        if (weaponRange > biggestWeaponRange)
                                            biggestWeaponRange = weaponRange;

                                    }

                                    
                                }
                                
                                unit.MaxApDamage = biggestWeaponApValue;
                                unit.MaxHeDamage = biggestWeaponHeValue;
                                unit.HasWeapons = hasWeapon;
                                unit.WeaponMaxRange = (int)(biggestWeaponRange / 130); // world units, and divide by 130 gets us game units?

                                if (biggestWeaponApValue == 0 && biggestWeaponHeValue > 0)
                                {
                                    mainTurret = potentialMainTurret;
                                }

                                var angleRad =
                                        (float)(mainTurret?.Instance.GetInstancePropertyValue<float>("AngleRotationMax") ?? 0.0f);
                                var angleDeg = Math.Round(angleRad * 180.0 / Math.PI, 1);
                                unit.TurretArcAngle = angleDeg;
                                
                            }
                            break;

                        case 129:
                            modInst = TryGetReference("TTagsModuleDescriptor", modRef)?.Instance;

                            if (modInst != null)
                            {
                                var tagsList = modInst.PropertyValues.FirstOrDefault(x => x.Property.Name == "TagSet")?.Value as
                                    NdfCollection;
                                if (tagsList != null)
                                {
                                    foreach (var tag in tagsList)
                                    {
                                        var tagStr = tag.Value.ToString();
                                        if (tagStr.StartsWith("Observ", StringComparison.InvariantCultureIgnoreCase))
                                            unit.ResolvedIsObserver = true;
                                    }
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

        private static NdfObjectReference TryGetReference(string className, NdfObjectReference objRef)
        {
            if (objRef?.Class.Name == className)
                return objRef;
            else
                return objRef?.Instance?.PropertyValues.FirstOrDefault(x => x.Property.Name == "Default")?.Value
                    as NdfObjectReference;
        }
    }
}
