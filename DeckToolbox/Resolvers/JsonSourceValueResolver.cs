using System;
using System.Collections.Generic;
using System.Text;
using DeckToolbox.Utils;
using Newtonsoft.Json.Linq;

namespace DeckToolbox.Resolvers
{
    public abstract class JsonSourceValueResolver
    {
        public IList<string> JsonSourceFiles { get; set; }
        protected Dictionary<string, JToken> _dataSources = new Dictionary<string, JToken>();
        protected IJsonFileReader _jsonFileReader;

        protected JsonSourceValueResolver(IJsonFileReader reader)
            : this(reader, null)
        {
        }

        protected JsonSourceValueResolver(IJsonFileReader reader, IList<string> jsonSourceFiles)
        {
            _jsonFileReader = reader;
            JsonSourceFiles = jsonSourceFiles;
            Initialize();
        }

        protected void Initialize()
        {
            _dataSources = new Dictionary<string, JToken>();
            foreach (var file in JsonSourceFiles)
            {
                try
                {
                    var txt = _jsonFileReader.ReadFile(file);
                    var dataSource = JToken.Parse(txt);
                    var id = dataSource["MetaData"]["DataIdentifier"].ToString();
                    var data = dataSource["Data"];
                    _dataSources.Add(id, data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
