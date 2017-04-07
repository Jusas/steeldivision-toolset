namespace DataExtractor.DataModels
{
    public class UnitModel
    {
        public int Id { get; set; }
        public string DescriptorId { get; set; }
        public string TypeUnitHint { get; set; }
        public string NameInMenu { get; set; }
        public int Category { get; set; }
        public string AliasName { get; set; }
        public string MotherCountry { get; set; }
        public int Factory { get; set; }
        public int ProductionPrice { get; set; }
        public bool ResolvedIsRecon { get; set; }
        public bool ResolvedIsCommand { get; set; }
    }
}
