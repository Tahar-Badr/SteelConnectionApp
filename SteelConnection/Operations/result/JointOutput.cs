using System.Collections.Generic;

namespace SteelConnection.Operations.Result
{
    internal class JointOutput
    {
        public double V_wpRd { get; set; }     // Shear resistance
        public double F_cwcRd { get; set; }    // Compression resistance
        public double Mj_Rd { get; set; }      // Moment resistance
        public double Sj_ini { get; set; }     // Initial stiffness

        public dynamic Classification { get; set; }
        public List<string> Warnings { get; } = new();
    }
}
