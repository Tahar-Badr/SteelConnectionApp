using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Operations.end_plate
{
    internal class EndPlateBending
    {
        public static class EndPlateBending
        {
            public class ResistanceResults
            {
                public double Mode1 { get; set; }  // Yielding of the plate
                public double Mode2 { get; set; }  // Bolt failure with plate yielding
                public double Mode3 { get; set; }  // Bolt failure
                public double Governing => Math.Min(Mode1, Math.Min(Mode2, Mode3));
            }

            public static (ResistanceResults Row1, ResistanceResults Row2,
                          ResistanceResults Row3, ResistanceResults Group23) Calculate(
                double t_p, double f_yp, double m, double m_x, double e, double e_x,
                double w, double b_p, double p, double sumFtRd, double a_f)
            {
                // Common calculations
                double m_plRd = (Math.Pow(t_p, 2) * f_yp) / (4 * SafetyFactors.GammaM0);
                double n_x = Math.Min(e_x, 1.25 * m_x);

                // Calculate alpha parameter
                double lambda1 = m / (m + e);
                double lambda2 = (e_x + e - 0.8 * a_f * Math.Sqrt(2)) / (m + e);
                double alpha = Math.Min(6 * lambda1, Math.Pow(1.5 * lambda2, 0.8));

                var results = (
                    Row1: CalculateRow1(m_x, e_x, w, b_p, m_plRd, n_x, sumFtRd),
                    Row2: CalculateRow2(m, e, p, alpha, m_plRd, n_x, sumFtRd),
                    Row3: CalculateRow3(m, e, p, m_plRd, n_x, sumFtRd),
                    Group23: CalculateGroup23(m, e, p, m_plRd, n_x, sumFtRd)
                );

                return results;
            }

            private static ResistanceResults CalculateRow1(
                double m_x, double e_x, double w, double b_p,
                double m_plRd, double n_x, double sumFtRd)
            {
                var res = new ResistanceResults();

                // Circular patterns
                double l_eff_cp1 = 2 * Math.PI * m_x;
                double l_eff_cp2 = Math.PI * m_x + w;
                double l_eff_cp3 = Math.PI * m_x + 2 * e_x;
                double l_eff_cp = Math.Min(l_eff_cp1, Math.Min(l_eff_cp2, l_eff_cp3));

                // Non-circular patterns
                double l_eff_nc1 = 4 * m_x + 1.25 * e_x;
                double l_eff_nc2 = e_x + 2 * m_x + 0.625 * e_x;
                double l_eff_nc3 = 0.5 * b_p;
                double l_eff_nc4 = 0.5 * w + 2 * m_x + 0.625 * e_x;
                double l_eff_nc = Math.Min(l_eff_nc1, Math.Min(l_eff_nc2, Math.Min(l_eff_nc3, l_eff_nc4)));

                // Mode 1 (Eq. 6.26)
                res.Mode1 = (4 * l_eff_cp * m_plRd) / m_x;

                // Mode 2 (Eq. 6.28)
                res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * sumFtRd) / (m_x + n_x);

                // Mode 3 (Eq. 6.27)
                res.Mode3 = sumFtRd;

                return res;
            }

            private static ResistanceResults CalculateRow2(
                double m, double e, double p, double alpha,
                double m_plRd, double n_x, double sumFtRd)
            {
                var res = new ResistanceResults();

                // Individual row
                double l_eff_cp_indiv = 2 * Math.PI * m;
                double l_eff_nc_indiv = alpha * m;

                // As first in group
                double l_eff_cp_group = Math.PI * m + p;
                double l_eff_nc_group = 0.5 * p + alpha * m - (2 * m + 0.625 * e);

                // Use minimal effective length
                double l_eff_cp = Math.Min(l_eff_cp_indiv, l_eff_cp_group);
                double l_eff_nc = Math.Min(l_eff_nc_indiv, l_eff_nc_group);

                res.Mode1 = (4 * l_eff_cp * m_plRd) / m;
                res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * sumFtRd) / (m + n_x);
                res.Mode3 = sumFtRd;

                return res;
            }

            private static ResistanceResults CalculateRow3(
                double m, double e, double p,
                double m_plRd, double n_x, double sumFtRd)
            {
                var res = new ResistanceResults();

                // Individual row
                double l_eff_cp_indiv = 2 * Math.PI * m;
                double l_eff_nc_indiv = 4 * m + 1.25 * e;

                // As last in group
                double l_eff_cp_group = Math.PI * m + p;
                double l_eff_nc_group = 2 * m + 0.625 * e + 0.5 * p;

                // Use minimal effective length
                double l_eff_cp = Math.Min(l_eff_cp_indiv, l_eff_cp_group);
                double l_eff_nc = Math.Min(l_eff_nc_indiv, l_eff_nc_group);

                res.Mode1 = (4 * l_eff_cp * m_plRd) / m;
                res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * sumFtRd) / (m + n_x);
                res.Mode3 = sumFtRd;

                return res;
            }

            private static ResistanceResults CalculateGroup23(
                double m, double e, double p,
                double m_plRd, double n_x, double sumFtRd)
            {
                var res = new ResistanceResults();

                // Group 2+3 effective lengths
                double l_eff_cp = 2 * p;  // Circular pattern
                double l_eff_nc = p;       // Non-circular pattern

                res.Mode1 = (4 * l_eff_cp * m_plRd) / m;
                res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * sumFtRd) / (m + n_x);
                res.Mode3 = sumFtRd;

                return res;
            }

            public static double GetDesignResistance(
                (ResistanceResults Row1, ResistanceResults Row2,
                 ResistanceResults Row3, ResistanceResults Group23) results,
                int activeRows)
            {
                return activeRows switch
                {
                    1 => results.Row1.Governing,
                    2 => results.Row2.Governing,
                    3 => Math.Min(
                        results.Row3.Governing,
                        results.Group23.Governing),
                    _ => throw new ArgumentException("Supported active rows: 1-3")
                };
            }
        }
    }
}
