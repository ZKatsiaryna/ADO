using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2.Common
{
    public class PopulationUtils<T>
    {
        public static List<T> CreateList(IDataReader reader)
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                    }
                }
                results.Add(item);
            }
            return results;
        }
    }
}
