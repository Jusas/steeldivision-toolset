using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;

namespace DataExtractor
{
    public static class NdfObjectExtensions
    {
        public static object GetInstancePropertyValue<DataType>(this NdfObject instance, string propertyName)
        {
            var val = instance.PropertyValues.FirstOrDefault(pv => pv.Property.Name == propertyName)?.Value;

            if (typeof(DataType) == typeof(double))
                return (val == null || val is NdfNull) ? 0 : double.Parse(val.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
            if (typeof(DataType) == typeof(bool))
                return (val != null && !(val is NdfNull)) && bool.Parse(val.ToString());
            if (typeof(DataType) == typeof(int))
                return (val == null || val is NdfNull) ? 0 : int.Parse(val.ToString());
            if (typeof(DataType) == typeof(string))
                return (val == null || val is NdfNull) ? "" : val.ToString();
            if (typeof(DataType) == typeof(float))
                return (val == null || val is NdfNull) ? 0 : float.Parse(val.ToString());
            if (typeof(DataType) == typeof(UInt32))
                return (val == null || val is NdfNull) ? 0 : UInt32.Parse(val.ToString());
            if (typeof(DataType) == typeof(int[]))
                return (val == null || val is NdfNull) ? new[] { 0 } : (val as NdfCollection).Select(x => int.Parse(x.Value.ToString())).ToArray();
            if (typeof(DataType) == typeof(Dictionary<int, int>))
            {
                var dic = new Dictionary<int, int>();
                if (val == null || val is NdfNull)
                    return dic;

                var maplist = val as NdfMapList;
                foreach(var item in maplist)
                {
                    var entry = item.Value as NdfMap;
                    var key = (int)(entry.Key.Value as NdfInt32).Value;
                    var valueHolder = entry.Value as MapValueHolder;
                    var value = (int)(valueHolder.Value as NdfInt32).Value;
                    dic.Add(key, value);
                }
                return dic;
            }
            return null;
        }
    }
}
