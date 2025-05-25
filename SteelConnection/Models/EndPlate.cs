namespace SteelConnection.Models
{
    public class EndPlate
    {
        public double Tp { get; set; } // Thickness
        public double Af { get; set; } // Front weld size
        public double Aw { get; set; } // Web weld size
        public double Fy { get; set; } // Yield strength (MPa)
        public double Ex { get; set; } // Edge distance (x-direction)
        public double E { get; set; }  // Bolt pitch

        public EndPlate(double tp, double af, double aw, double fy, double ex, double e)
        {
            Tp = tp;
            Af = af;
            Aw = aw;
            Fy = fy;
            Ex = ex;
            E = e;
        }
    }
}