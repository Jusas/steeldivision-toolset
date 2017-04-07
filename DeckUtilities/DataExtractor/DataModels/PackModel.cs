namespace DataExtractor.DataModels
{
    public class PackModel
    {
        public int Id { get; set; }
        public string DerivedUnitName { get; set; }
        public int AvailableFromPhase { get; set; }
        public int FactoryType { get; set; }
        public int ExperienceLevel { get; set; }
        public string TransportDescriptorId { get; set; }
        public string UnitDescriptorId { get; set; }
        public int TransportAndUnitCount { get; set; }
    }
}
