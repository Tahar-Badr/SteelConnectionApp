using System;
using SteelConnection.Models;

namespace SteelConnection.Operations.Column
{
    internal static class ColumnWebCompression
    {
        /// <summary>
        /// Calculates the column web compression resistance.
        /// </summary>
        public static double Calculate(
            double t_fb,   // Beam flange thickness (mm)
            double t_fc,   // Column flange thickness (mm)
            double r_c,    // Column root radius (mm)
            double s_p,    // Bolt spacing in direction of force (mm)
            double d_wc,   // Column web depth (mm)
            double f_ywc,  // Yield strength of web (MPa)
            double E,      // Modulus of elasticity (MPa)
            double a_f)    // Distance from weld toe to bolt row (mm)
        {
            double b_eff = t_fb + 2 * a_f * Math.Sqrt(2) + 5 * (t_fc + r_c) + s_p;
            double omega = 1 / Math.Sqrt(1 + 1.3 * Math.Pow(b_eff / d_wc, 2));
            double lambda_p = 0.932 * Math.Sqrt(b_eff * d_wc * f_ywc / (E * Math.Pow(t_fc, 2)));
            double rho = (lambda_p > 0.72) ? (lambda_p - 0.2) / Math.Pow(lambda_p, 2) : 1.0;

            double resistance = (omega * b_eff * t_fc * f_ywc * rho) / SafetyFactors.GammaM0;
            return resistance;
        }

        /// <summary>
        /// Overloaded method using JointInput directly.
        /// </summary>
        public static double Calculate(JointInput input)
        {
            return Calculate(
                input.T_fb,
                input.T_fc,
                input.R_c,
                input.S_p,
                input.d_wc,
                input.f_ywc,
                input.E,
                input.A_f);
        }
    }
}
