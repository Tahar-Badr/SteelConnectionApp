using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SteelConnection.data
{
    public static class ProfileLoader
    {
        public static List<SteelProfile> Load(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<List<SteelProfile>>(json);
        }
    }
}
