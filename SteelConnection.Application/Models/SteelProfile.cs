using Newtonsoft.Json;

namespace SteelConnection.Application.Models
{
    public class SteelProfile
    {

        public string? ProfileType { get; set; }
        public int Size { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public double WebThickness { get; set; }
        public double FlangeThickness { get; set; }
        public double RootRadius { get; set; }
        public double Area { get; set; }
        public double Iy { get; set; }
        public double Iz { get; set; }
        public double Wy { get; set; }
        public double Wz { get; set; }
        public double Wply { get; set; }
        public double Wplz { get; set; }


        public static List<SteelProfile> LoadProfilesFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("JSON file not found.");
            }

            string jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<SteelProfile>>(jsonData);
        }
    }
}
