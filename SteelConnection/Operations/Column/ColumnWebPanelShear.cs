using System;
using SteelConnection.Models;

namespace SteelConnection.Operations.Column
{
    internal static class ColumnWebPanelShear
    {
        /// <summary>
        /// Calculates the shear resistance of column web panel (Component N°1)
        /// </summary>
        public static double Calculate(
            double A_vc,         // Shear area of column web panel (mm²)
            double f_ywc,        // Yield strength of column web (MPa)
            double beta = 1.0)   // Transformation parameter (default 1.0)
        {
            return (0.9 * A_vc * f_ywc * beta) / (Math.Sqrt(3) * SafetyFactors.GammaM0); // Eq. 6.7
        }

        /// <summary>
        /// Overloaded method using JointInput directly.
        /// </summary>
        public static double Calculate(JointInput input)
        {
            return Calculate(input.A_vc, input.f_ywc, input.Beta);
        }

        public static (double F1, double F2, double F3) RedistributeForces(
            double F1_min, double F2_min, double F3_min,
            double F_glob_min)
        {
            double F3_red = 0;
            double F2_red = F2_min;
            double F1_red = F1_min;

            if (F_glob_min >= (F1_min + F2_min + F3_min))
                return (F1_min, F2_min, F3_min);

            if ((F1_min + F2_min) > F_glob_min)
            {
                F3_red = 0;
                F2_red = F_glob_min - F1_min;
                if (F2_red < 0) F2_red = 0;
            }
            else
            {
                F3_red = F_glob_min - (F1_min + F2_min);
            }

            return (F1_red, F2_red, F3_red);
        }

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
