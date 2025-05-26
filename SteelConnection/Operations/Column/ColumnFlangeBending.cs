using System;
using SteelConnection.Models;

namespace SteelConnection.Operations.Column
{
    /// <param name="b_c">Column flange width (mm)</param>
    /// <param name="w">Bolt group width (mm)</param>
    /// <param name="m">Distance from bolt to flange edge (mm)</param>
    /// <param name="t_fc">Flange thickness (mm)</param>
    /// <param name="f_yfc">Flange yield strength (MPa)</param>
    /// <param name="p">Bolt pitch (mm)</param>
    /// <param name="e">Edge distance (mm)</param>
    /// <param name="n">Number of bolts in group</param>
    /// <param name="sumFtRd">Sum of bolt tension resistances (kN)</param>
    /// <param name="boltRows">Number of bolt rows (must be 2 or 4)</param>

    internal static class ColumnFlangeBending
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

            // Common plastic moment resistance (Eq. 6.22 logic)
            double m_plRd = (Math.Pow(t_fc, 2) * f_yfc) / (4 * SafetyFactors.GammaM0);

            if (boltRows == 2)
            {
                double b_eff = Math.Min(2 * Math.PI * m, 4 * m + 1.25 * e);
                double l_eff = b_eff;

                results.Individual.Mode1 = (4 * l_eff * m_plRd) / m;
                results.Individual.Mode2 = (2 * l_eff * m_plRd + n * sumFtRd) / (m + n);
                results.Individual.Mode3 = sumFtRd;
            }
            else if (boltRows == 4)
            {
                double l_eff_indiv = Math.Min(2 * Math.PI * m, 4 * m + 1.25 * e);
                results.Individual.Mode1 = (4 * l_eff_indiv * m_plRd) / m;
                results.Individual.Mode2 = (2 * l_eff_indiv * m_plRd + n * sumFtRd) / (m + n);
                results.Individual.Mode3 = sumFtRd;

                double l_eff_group12 = Math.Min(Math.PI * m + p, 2 * m + 0.625 * e + 0.5 * p);
                results.Group12.Mode1 = (4 * l_eff_group12 * m_plRd) / m;
                results.Group12.Mode2 = (2 * l_eff_group12 * m_plRd + n * sumFtRd) / (m + n);
                results.Group12.Mode3 = sumFtRd;

                double l_eff_group123 = 2 * m + 0.625 * e + p;
                results.Group123.Mode1 = (4 * l_eff_group123 * m_plRd) / m;
                results.Group123.Mode2 = (2 * l_eff_group123 * m_plRd + n * sumFtRd) / (m + n);
                results.Group123.Mode3 = sumFtRd;

                double l_eff_group23 = p;
                results.Group23.Mode1 = (4 * l_eff_group23 * m_plRd) / m;
                results.Group23.Mode2 = (2 * l_eff_group23 * m_plRd + n * sumFtRd) / (m + n);
                results.Group23.Mode3 = sumFtRd;
            }
            else
            {
                throw new ArgumentException("Only 2 or 4 bolt rows are supported.");
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
                2 => Math.Min(results.Individual.Governing * 2, results.Group12.Governing),
                3 => Math.Min(results.Individual.Governing * 3, results.Group123.Governing),
                4 => Math.Min(
                        Math.Min(results.Individual.Governing * 4, results.Group123.Governing),
                        results.Group12.Governing + results.Group23.Governing),
                _ => throw new ArgumentException("Invalid number of active rows.")
            };
        }
    }
}
