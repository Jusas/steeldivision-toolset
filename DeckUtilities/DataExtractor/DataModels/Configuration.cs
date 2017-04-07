using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnitExtractor.DataModels
{
    public class Configuration
    {
        public List<string> DataSources { get; set; }
        public Dictionary<string, string> DataMappings { get; set; }     

        public Configuration()
        {
        }

        public static Configuration FromFile(string fromFile)
        {
            try
            {
                var confjson = File.ReadAllText(fromFile);
                var conf = JsonConvert.DeserializeObject<Configuration>(confjson);
                conf.ParseMappings();
                return conf;
            }
            catch(Exception)
            {
                return null;
            }
        }
        
        private void ParseMappings()
        {
            var variablePattern = new Regex(@"\$\((.*?)\)");
            var keys = DataMappings.Keys.ToArray();
            foreach(var key in keys)
            {
                var mapping = DataMappings[key];
                var replaceables = variablePattern.Matches(mapping);
                foreach(Match replaceable in replaceables)
                {
                    var parts = replaceable.Groups[1].Value.Split('.');
                    string source = parts[0];
                    string index = null;
                    if(parts.Length == 2)
                    {
                        index = parts[1];
                    }

                    var prop = GetType().GetProperty(source);
                    if(prop != null)
                    {
                        var propValue = prop.GetValue(this);
                        if (index == null)
                            mapping = variablePattern.Replace(mapping, propValue.ToString());
                        else
                            mapping = variablePattern.Replace(mapping, ((IEnumerable<object>)propValue).ElementAt(int.Parse(index)).ToString());
                    }
                }
                DataMappings[key] = mapping;
            }
            
        }
    }
}
