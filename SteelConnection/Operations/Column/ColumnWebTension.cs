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

        // A
        public static ResistanceResults Calculate(
            double m, double t_wc, double r_c, double e,
            double b_c, double b_b, double w, double f_ywc,
            int boltRows, double p1, double p2, double Avc,
            bool isFirstRow = false)
        {
            var results = new ResistanceResults();

            // Keep your original variables here
            double m_fc = (m - t_wc) / 2 - 0.8 * r_c;
            double e_wc = Math.Min((b_c - w) / 2, (b_b - w) / 2);
            double omega = 0.837; // When β = 1

            // --- Start of row effect calculations ---

            // Effective lengths for bolt rows based on provided formulas
            double leff_row1 = 2 * m + 0.625 * e + 0.5 * p1;
            double leff_row2 = 2 * m + 0.625 * e + 0.5 * p2;
            double leff_row3 = 4 * m + 1.25 * ((p1 + p2) / 2);

            // Calculate omega for each row based on effective length
            double CalcOmega(double beff)
            {
                return 1.0 / Math.Sqrt(1 + 1.3 * Math.Pow(beff / t_wc * Avc, 2));
            }

            double omega1 = CalcOmega(leff_row1);
            double omega2 = CalcOmega(leff_row2);
            double omega3 = CalcOmega(leff_row3);

            // Calculate resistance for each row
            double F_row1 = omega1 * leff_row1 * t_wc * f_ywc;
            double F_row2 = omega2 * leff_row2 * t_wc * f_ywc;
            double F_row3 = omega3 * leff_row3 * t_wc * f_ywc;

            // Apply partial safety factor γ_m0
            double gamma_m0 = 1.0; // You can adjust this if needed
            F_row1 /= gamma_m0;
            F_row2 /= gamma_m0;
            F_row3 /= gamma_m0;

            // Assign calculated resistances
            results.IndividualRows = F_row1;
            results.Rows1And2 = F_row1 + F_row2;
            results.Rows1And2And3 = F_row1 + F_row2 + F_row3;
            results.Rows2And3 = F_row2 + F_row3;

            // --- End of row effect calculations ---

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
