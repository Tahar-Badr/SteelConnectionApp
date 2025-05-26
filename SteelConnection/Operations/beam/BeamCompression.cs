using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Operations.beam
{
    internal class BeamCompression
    {
        public static class BeamCompression
        {
            /// <summary>
            /// Calculates the compression resistance of beam flange and web
            /// </summary>
            /// <param name="w_ply">Plastic section modulus (mm³)</param>
            /// <param name="f_yb">Beam yield strength (MPa)</param>
            /// <param name="h_b">Beam depth (mm)</param>
            /// <param name="t_fb">Beam flange thickness (mm)</param>
            /// <returns>Design resistance in kN</returns>
            public static double Calculate(
                double w_ply,
                double f_yb,
                double h_b,
                double t_fb)
            {
                // 1. Calculate plastic moment resistance (Eq. 6.22)
                double M_plRd = (w_ply * f_yb) / SafetyFactors.GammaM0;

                // 2. Calculate design resistance (Eq. 6.23)
                double F_Rd = M_plRd / (h_b - t_fb);

                return F_Rd / 1000; // Convert N to kN
            }

            /// <summary>
            /// Alternative calculation using SteelProfile object
            /// </summary>
            public static double Calculate(SteelProfile beam)
            {
                return Calculate(
                    w_ply: beam.Wply,
                    f_yb: beam.YieldStrength,
                    h_b: beam.Height,
                    t_fb: beam.FlangeThickness);
            }
        }
    }
}
