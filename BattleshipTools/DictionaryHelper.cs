using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTools
{
    public static class DictionaryHelper
    {
        public static void AddOrIncrement(Dictionary<string, int> dict, string key, int by = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += by;
            else
                dict.Add(key, by);
        }

        public static void AddOrIncrement(Dictionary<string, long> dict, string key, long by = 1)
        {
            if (dict.ContainsKey(key))
                dict[key] += by;
            else
                dict.Add(key, by);
        }
    }
}
