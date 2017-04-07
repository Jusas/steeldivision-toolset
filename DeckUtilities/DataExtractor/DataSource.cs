using System;
using System.Collections.Generic;
using System.Linq;
using moddingSuite.BL;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Ndfbin;

namespace DataExtractor
{
    public class DataSource
    {
        private Dictionary<string, Tuple<EdataManager, Dictionary<string, object>>> _dataSources =
            new Dictionary<string, Tuple<EdataManager, Dictionary<string, object>>>();

        public DataType GetDataSource<DataType>(string sourceDataPath)
        {
            var separator = sourceDataPath.LastIndexOf(':');
            var sourceDataFile = sourceDataPath.Substring(0, separator);
            var dataObjectPath = sourceDataPath.Substring(separator + 1);

            if (!_dataSources.ContainsKey(sourceDataFile))
            {
                EdataManager eManager = new EdataManager(sourceDataFile);
                eManager.ParseEdataFile();
                _dataSources.Add(sourceDataFile, new Tuple<EdataManager, Dictionary<string, object>>(eManager, new Dictionary<string, object>()));
            }

            var src = _dataSources[sourceDataFile];
            var dataManager = src.Item1;
            var dataObjectDic = src.Item2;
            var dataObject = dataObjectDic.FirstOrDefault(x => x.Key == dataObjectPath).Value;
            if (dataObject != null)
                return (DataType)dataObject;
            else
            {
                var dataContentFile = dataManager.Files.FirstOrDefault(f => f.Path.Contains(dataObjectPath));
                if (typeof(DataType) == typeof(NdfBinary))
                {
                    var ndfbinReader = new NdfbinReader();
                    object ndfBinary = ndfbinReader.Read(dataManager.GetRawData(dataContentFile));
                    dataObjectDic.Add(dataObjectPath, ndfBinary);
                    return (DataType)ndfBinary;
                }
                if (typeof(DataType) == typeof(TradManager))
                {
                    object tm = new TradManager(dataManager.GetRawData(dataContentFile));
                    dataObjectDic.Add(dataObjectPath, tm);
                    return (DataType)tm;
                }

                // Other types not implemented.
                throw new NotImplementedException();
            }
        }

        public string GetLocalizedString(string localizationHash, string sourceDataPath)
        {
            var dic = GetDataSource<TradManager>(sourceDataPath);
            var str = dic.Entries.FirstOrDefault(x => x.HashView == localizationHash);
            return str?.Content ?? "";
        }
    }
}
