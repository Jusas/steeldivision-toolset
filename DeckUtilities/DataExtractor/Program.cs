using DataExtractor.Extractors;
using UnitExtractor.DataModels;

namespace DataExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var exporter = new JsonFileExporter(Configuration.FromFile("configuration.json"), new DataSource(), @".\Json");
            exporter.AddExporter(DivisionExtractor.GetDivisions, "Divisions.json");
            exporter.AddExporter(ModularUnitExtractor.GetUnitData, "Units.json");
            exporter.AddExporter(PackExtractor.GetPacks, "Packs.json");
            exporter.Export();
        }
    }
}
