namespace SteelConnection.Models
{
    public class EndPlate
    {
        public double Tp { get; set; } // Plate thickness
        public double Af { get; set; } // Flange area
        public double Aw { get; set; } // Web area
        public double Fy { get; set; } // Yield strength
        public double Ex { get; set; } // Eccentricity x
        public double E { get; set; }  // Elastic modulus

        public EndPlate(double tp, double af, double aw, double fy, double ex, double e)
        {
            Tp = tp;
            Af = af;
            Aw = aw;
            Fy = fy;
            Ex = ex;
            E = e;
        }

        public void DisplayProperties()
        {
            Console.WriteLine($"End Plate - Tp: {Tp}, Af: {Af}, Aw: {Aw}, Fy: {Fy}, Ex: {Ex}, E: {E}");
        }
    }
}
