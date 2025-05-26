using System;
using SteelConnection.Models;

namespace SteelConnection.Operations.Column
{
    internal static class ColumnWebTension
    {
        public class ResistanceResults
        {
            public double IndividualRows { get; set; }
            public double Rows1And2 { get; set; }
            public double Rows1And2And3 { get; set; }
            public double Rows2And3 { get; set; }
        }

        public static ResistanceResults Calculate(
            double m, double t_wc, double r_c, double e,
            double b_c, double b_b, double w, double f_ywc,
            int boltRows, bool isFirstRow = false)
        {
            var results = new ResistanceResults();

            // Common parameters
            double m_fc = (m - t_wc) / 2 - 0.8 * r_c;
            double e_wc = Math.Min((b_c - w) / 2, (b_b - w) / 2);
            double omega = 0.837; // When β = 1

            if (boltRows == 2)
            {
                // Simplified 2-row calculation
                double b_eff = Math.Min(4 * m + 1.25 * e, 2 * Math.PI * m);
                results.IndividualRows = (omega * b_eff * t_wc * f_ywc) / SafetyFactors.GammaM0;
            }
            else if (boltRows == 4)
            {
                // Individual rows (identical for rows 1–3)
                double b_eff_indiv = Math.Min(4 * m + 1.25 * e, 2 * Math.PI * m);
                results.IndividualRows = (omega * b_eff_indiv * t_wc * f_ywc) / SafetyFactors.GammaM0;

                // Group 1+2
                double b_eff_group12_circ = Math.PI * m + 2 * e_wc;
                double b_eff_group12_nc = 4 * m + 1.25 * e_wc;
                double b_eff_group12 = Math.Min(b_eff_group12_circ, b_eff_group12_nc);
                results.Rows1And2 = (omega * b_eff_group12 * t_wc * f_ywc) / SafetyFactors.GammaM0;

                // Group 1+2+3
                double b_eff_group123 = 4 * m + 1.25 * e_wc + 2 * m;
                results.Rows1And2And3 = (omega * b_eff_group123 * t_wc * f_ywc) / SafetyFactors.GammaM0;

                // Group 2+3
                double b_eff_group23 = 4 * m + 1.25 * e_wc;
                results.Rows2And3 = (omega * b_eff_group23 * t_wc * f_ywc) / SafetyFactors.GammaM0;
            }
            else
            {
                throw new ArgumentException("Only 2 or 4 bolt rows are supported");
            }

            return results;
        }

        public static double GetDesignResistance(ResistanceResults results, int activeRows)
        {
            return activeRows switch
            {
                2 => Math.Min(results.IndividualRows * 2, results.Rows1And2),
                3 => Math.Min(results.IndividualRows * 3, results.Rows1And2And3),
                4 => Math.Min(
                        Math.Min(results.IndividualRows * 4, results.Rows1And2And3),
                        results.Rows1And2 + results.Rows2And3),
                _ => results.IndividualRows
            };
        }
    }
}
