using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Operations.Column
{
    internal class ColumnWebPanelShear
    {
        public static class ColumnWebPanelShear
        {
            /// <summary>
            /// Calculates the shear resistance of column web panel (Component N°1)
            /// </summary>
            public static double Calculate(
                double A_vc,         // Shear area of column web panel (mm²)
                double f_ywc,        // Yield strength of column web (MPa)
                double beta = 1.0)   // Transformation parameter (default 1.0)
            {
                return (0.9 * A_vc * f_ywc) / (Math.Sqrt(3) * SafetyFactors.GammaM0); // Eq. 6.7
            }

            /// <summary>
            /// Redistributes forces considering group effects and global limits
            /// </summary>
            public static (double F1, double F2, double F3) RedistributeForces(
                double F1_min, double F2_min, double F3_min,
                double F_glob_min)
            {
                double F3_red = 0;
                double F2_red = F2_min;
                double F1_red = F1_min;

                // Case 1: Global limit exceeds total capacity
                if (F_glob_min >= (F1_min + F2_min + F3_min))
                    return (F1_min, F2_min, F3_min);

                // Case 2: Row 3 must be fully reduced
                if ((F1_min + F2_min) > F_glob_min)
                {
                    F3_red = 0;
                    F2_red = F_glob_min - F1_min;
                    if (F2_red < 0) F2_red = 0;
                }
                // Case 3: Partial reduction needed
                else
                {
                    F3_red = F_glob_min - (F1_min + F2_min);
                }

                return (F1_red, F2_red, F3_red);
            }

            /// <summary>
            /// Checks equilibrium between bolt row forces and global components
            /// </summary>
            public static bool CheckEquilibrium(
                double F1, double F2, double F3,
                double V_wpRd, double F_cwcRd, double F_cfbRd)
            {
                double F_glob_min = Math.Min(V_wpRd, Math.Min(F_cwcRd, F_cfbRd));
                double sumF = F1 + F2 + F3;

                return sumF <= F_glob_min;
            }
        }
    }
}
