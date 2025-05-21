using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Operations.Column
{
    internal class ColumnFlangeBending
    {
        public static class ColumnFlangeBending
        {
            public class ResistanceResults
            {
                public double Mode1 { get; set; }
                public double Mode2 { get; set; }
                public double Mode3 { get; set; }
                public double Governing => Math.Min(Mode1, Math.Min(Mode2, Mode3));
            }

            public static (ResistanceResults Individual, ResistanceResults Group12,
                          ResistanceResults Group123, ResistanceResults Group23) Calculate(
                double b_c, double w, double m, double t_fc, double f_yfc,
                double p, double e, double n, double sumFtRd, int boltRows)
            {
                var results = (
                    Individual: new ResistanceResults(),
                    Group12: new ResistanceResults(),
                    Group123: new ResistanceResults(),
                    Group23: new ResistanceResults()
                );

                // Common calculations
                double m_plRd = (Math.Pow(t_fc, 2) * f_yfc) / (4 * SafetyFactors.GammaM0);

                if (boltRows == 2)
                {
                    // Simplified 2-row calculation
                    double b_eff = Math.Min(2 * Math.PI * m, 4 * m + 1.25 * e);
                    double l_eff = b_eff; // For 2 rows, effective length = effective width

                    results.Individual.Mode1 = (4 * l_eff * m_plRd) / m;
                    results.Individual.Mode2 = (2 * l_eff * m_plRd + n * sumFtRd) / (m + n);
                    results.Individual.Mode3 = sumFtRd;
                }
                else if (boltRows == 4)
                {
                    // 1. Individual rows (rows 1-3)
                    double l_eff_indiv = Math.Min(2 * Math.PI * m, 4 * m + 1.25 * e);
                    results.Individual.Mode1 = (4 * l_eff_indiv * m_plRd) / m;
                    results.Individual.Mode2 = (2 * l_eff_indiv * m_plRd + n * sumFtRd) / (m + n);
                    results.Individual.Mode3 = sumFtRd;

                    // 2. Group 1+2
                    double l_eff_group12 = Math.Min(
                        Math.PI * m + p,          // Circular pattern
                        2 * m + 0.625 * e + 0.5 * p // Non-circular pattern
                    );
                    results.Group12.Mode1 = (4 * l_eff_group12 * m_plRd) / m;
                    results.Group12.Mode2 = (2 * l_eff_group12 * m_plRd + n * sumFtRd) / (m + n);
                    results.Group12.Mode3 = sumFtRd;

                    // 3. Group 1+2+3
                    double l_eff_group123 = 2 * m + 0.625 * e + p;
                    results.Group123.Mode1 = (4 * l_eff_group123 * m_plRd) / m;
                    results.Group123.Mode2 = (2 * l_eff_group123 * m_plRd + n * sumFtRd) / (m + n);
                    results.Group123.Mode3 = sumFtRd;

                    // 4. Group 2+3
                    double l_eff_group23 = p; // For internal rows
                    results.Group23.Mode1 = (4 * l_eff_group23 * m_plRd) / m;
                    results.Group23.Mode2 = (2 * l_eff_group23 * m_plRd + n * sumFtRd) / (m + n);
                    results.Group23.Mode3 = sumFtRd;
                }
                else
                {
                    throw new ArgumentException("Only 2 or 4 bolt rows are supported");
                }

                return results;
            }

            public static double GetDesignResistance(
                (ResistanceResults Individual, ResistanceResults Group12,
                 ResistanceResults Group123, ResistanceResults Group23) results,
                int activeRows)
            {
                return activeRows switch
                {
                    1 => results.Individual.Governing,
                    2 => Math.Min(
                        results.Individual.Governing * 2,
                        results.Group12.Governing),
                    3 => Math.Min(
                        results.Individual.Governing * 3,
                        results.Group123.Governing),
                    4 => Math.Min(
                        Math.Min(
                            results.Individual.Governing * 4,
                            results.Group123.Governing),
                        results.Group12.Governing + results.Group23.Governing),
                    _ => throw new ArgumentException("Invalid number of active rows")
                };
            }
        }
    }
}
