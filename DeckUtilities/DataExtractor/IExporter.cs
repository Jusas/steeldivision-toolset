using System;
using UnitExtractor;
using UnitExtractor.DataModels;

namespace DataExtractor
{
    public interface IExporter
    {
        void AddExporter(Func<Configuration, DataSource, object> extractorMethod, string outputFilename);
        void Export();
    }
}
