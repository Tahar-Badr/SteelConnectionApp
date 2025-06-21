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

            // 1. Bolt and material values
            double gammaM0 = SafetyFactors.GammaM0;
            double gammaM2 = SafetyFactors.GammaM2;

            // 2. Bolt tension resistance per bolt (assuming uniform)
            double BtRd = 0.9 * (sumFtRd / n) / gammaM2;

            // 3. Individual Row 1 (assume leff_cp = m)
            double leff_cp_1 = m;
            double MplRd_1 = 0.25 * leff_cp_1 * Math.Pow(t_fc, 2) * f_yfc / gammaM0;

            results.Individual.Mode1 = (4 * MplRd_1) / m;
            results.Individual.Mode2 = (2 * MplRd_1 + n * BtRd) / (m + n);
            results.Individual.Mode3 = 2 * BtRd;

            // 4. Group 1 + 2
            double leff_nc_12 = e + p; // first + last rows
            double MplRd_12 = 0.25 * leff_nc_12 * Math.Pow(t_fc, 2) * f_yfc / gammaM0;

            results.Group12.Mode1 = (4 * MplRd_12) / m;
            results.Group12.Mode2 = (2 * MplRd_12 + n * BtRd) / (m + n);
            results.Group12.Mode3 = 4 * BtRd;

            // 5. Group 1 + 2 + 3
            double leff_nc_123 = e + p + p; // first + internal + last
            double MplRd_123 = 0.25 * leff_nc_123 * Math.Pow(t_fc, 2) * f_yfc / gammaM0;

            results.Group123.Mode1 = (4 * MplRd_123) / m;
            results.Group123.Mode2 = (2 * MplRd_123 + n * BtRd) / (m + n);
            results.Group123.Mode3 = 6 * BtRd;

            // 6. Group 2 + 3
            double leff_nc_23 = p + e; // first + last of group
            double MplRd_23 = 0.25 * leff_nc_23 * Math.Pow(t_fc, 2) * f_yfc / gammaM0;

            results.Group23.Mode1 = (4 * MplRd_23) / m;
            results.Group23.Mode2 = (2 * MplRd_23 + n * BtRd) / (m + n);
            results.Group23.Mode3 = 4 * BtRd;

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
