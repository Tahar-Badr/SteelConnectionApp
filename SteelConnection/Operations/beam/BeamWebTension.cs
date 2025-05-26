using System;
using SteelConnection.Models;

namespace SteelConnection.Operations.Beam
{
    internal static class BeamWebTension
    {
        public class ResistanceResults
        {
            public double Row2Individual { get; set; }
            public double Row3Individual { get; set; }
            public double Rows2And3Group { get; set; }

            public double Governing => Math.Min(
                Row2Individual + Row3Individual,
                Rows2And3Group);
        }

        /// <summary>
        /// Calculates tension resistance for beam web (EN 1993-1-8 §6.2.6.8)
        /// </summary>
        public static ResistanceResults Calculate(
            double t_wb,        // Beam web thickness (mm)
            double f_ywb,       // Beam web yield strength (MPa)
            double b_eff_row2,  // Effective width for row 2 (mm)
            double b_eff_row3,  // Effective width for row 3 (mm)
            double b_eff_group) // Effective width for group (mm)
        {
            var results = new ResistanceResults
            {
                // Individual resistances (Eq. 6.26)
                Row2Individual = (b_eff_row2 * t_wb * f_ywb) / SafetyFactors.GammaM0,
                Row3Individual = (b_eff_row3 * t_wb * f_ywb) / SafetyFactors.GammaM0,

                // Group resistance (rows 2+3)
                Rows2And3Group = (b_eff_group * t_wb * f_ywb) / SafetyFactors.GammaM0
            };

            return results;
        }

        /// <summary>
        /// Calculates effective widths for common cases
        /// </summary>
        public static (double b_eff_row2, double b_eff_row3, double b_eff_group)
            CalculateEffectiveWidths(
                double m,       // Distance from bolt to web
                double e,       // Edge distance
                double p,       // Bolt pitch
                bool isLastRow = false)
        {
            // Individual rows (circular yield)
            double b_eff_indiv = Math.Min(
                2 * Math.PI * m,
                4 * m + 1.25 * e);

            // Group effect (non-circular yield)
            double b_eff_group = isLastRow
                ? Math.PI * m + p             // Last row group
                : 2 * m + 0.625 * e + 0.5 * p; // Internal row group

            return (b_eff_indiv, b_eff_indiv, b_eff_group);
        }
    }
}
