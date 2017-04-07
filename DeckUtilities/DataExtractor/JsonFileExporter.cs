using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnitExtractor;
using UnitExtractor.DataModels;

namespace DataExtractor
{
    public class JsonFileExporter : IExporter
    {
        private Dictionary<string, Func<Configuration, DataSource, object>> _extractors = new Dictionary<string, Func<Configuration, DataSource, object>>();
        private DataSource _dataSource;
        private Configuration _configuration;
        private string _outputDir;

        public JsonFileExporter(Configuration c, DataSource d, string outputDirectory)
        {
            _configuration = c;
            _dataSource = d;
            _outputDir = outputDirectory;
        }

        public void AddExporter(Func<Configuration, DataSource, object> extractorMethod, string outputFilename)
        {
            _extractors.Add(outputFilename, extractorMethod);
        }

        public void Export()
        {
            foreach(var e in _extractors)
            {
                var outputFilename = e.Key;
                var data = e.Value.Invoke(_configuration, _dataSource);
                var serialized = JsonConvert.SerializeObject(data, Formatting.Indented);
                if (!Directory.Exists(_outputDir))
                    Directory.CreateDirectory(_outputDir);
                File.WriteAllText(Path.Combine(_outputDir, outputFilename), serialized);
            }
        }
    }
}
