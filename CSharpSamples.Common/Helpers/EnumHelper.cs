using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpSamples.Common.Helpers
{
    public class EnumHelper
    {
        public static IDictionary<int, string> GetEnumList(Type enumType, Func<int, string, string> getCustomName = null)
        {
            var names = Enum.GetNames(enumType).ToList();
            var values = Enum.GetValues(enumType);
            var valuePair = new Dictionary<int, string>();

            for (var i = 0; i < names.Count(); i++)
            {
                var name = names[i];
                var value = Convert.ToInt32(values.GetValue(i));

                if (getCustomName != null)
                    name = getCustomName(value, name);
                valuePair.Add(value, name);
            }

            return valuePair;
        }
    }
}
