using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Operations.Column
{
    internal class ColumnWebCompression
    {
        public static class ColumnWebCompression
        {
            public static double Calculate(
                double t_fb, double t_fc, double r_c, double s_p,
                double d_wc, double f_ywc, double E, double a_f)
            {
                double b_eff = t_fb + 2 * a_f * Math.Sqrt(2) + 5 * (t_fc + r_c) + s_p;
                double omega = 1 / Math.Sqrt(1 + 1.3 * Math.Pow(b_eff / d_wc, 2));
                double lambda_p = 0.932 * Math.Sqrt(b_eff * d_wc * f_ywc / (E * Math.Pow(t_fc, 2)));
                double rho = (lambda_p > 0.72) ? (lambda_p - 0.2) / Math.Pow(lambda_p, 2) : 1.0;
                return (omega * b_eff * t_fc * f_ywc * rho) / SafetyFactors.GammaM0;
            }
        }
    }
}
