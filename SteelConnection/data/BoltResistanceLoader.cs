using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SteelConnection.data
{
    public static class BoltResistanceLoader
    {
        public static Dictionary<string, Dictionary<string, double>> Load()
        {
            string json = File.ReadAllText("Resources/bolt_tension_resistance.json");
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(json);
        }
    }
}
