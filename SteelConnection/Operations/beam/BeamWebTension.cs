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

            /// <summary>
            /// Governing resistance: minimum of (Row2 + Row3) vs Group mechanism (EN 1993-1-8 §6.2.6.8)
            /// </summary>
            public double Governing => Math.Min(
                Row2Individual + Row3Individual,
                Rows2And3Group);

            public string GoverningFailureMode =>
                (Row2Individual + Row3Individual) < Rows2And3Group
                ? "Individual (Row2 + Row3)"
                : "Group (Plastic mechanism)";
        }

        /// <summary>
        /// Calculates tension resistance for beam web in tension
        /// according to EN 1993-1-8 §6.2.6.8 (Component Method - Component 08)
        /// </summary>
        public static ResistanceResults Calculate(
            double t_wb,        // Beam web thickness [mm]
            double f_ywb,       // Beam web yield strength [MPa]
            double b_eff_row2,  // Effective width for Row 2 [mm]
            double b_eff_row3,  // Effective width for Row 3 [mm]
            double b_eff_group) // Effective width for Group (Rows 2+3) [mm]
        {
            var results = new ResistanceResults
            {
                // Eq. 6.26 – Individual tension resistance (Row 2)
                Row2Individual = (b_eff_row2 * t_wb * f_ywb) / SafetyFactors.GammaM0,

                // Eq. 6.26 – Individual tension resistance (Row 3)
                Row3Individual = (b_eff_row3 * t_wb * f_ywb) / SafetyFactors.GammaM0,

                // Eq. 6.27 – Group resistance (combined plastic mechanism Rows 2+3)
                Rows2And3Group = (b_eff_group * t_wb * f_ywb) / SafetyFactors.GammaM0
            };

            return results;
        }

        /// <summary>
        /// Calculates effective widths for individual rows and group mechanism.
        /// Based on EN 1993-1-8 §6.2.6.8 and plastic mechanism assumptions.
        /// </summary>
        public static (double b_eff_row2, double b_eff_row3, double b_eff_group)
            CalculateEffectiveWidths(
                double m,       // Distance from bolt to face of web [mm]
                double e,       // End distance from bolt center to beam end [mm]
                double p,       // Vertical pitch between bolts [mm]
                bool isLastRow = false)
        {
            // Effective width for individual circular mechanism (rows 2 or 3)
            // Usually: min(2πm, 4m + 1.25e)
            double b_eff_indiv = Math.Min(
                2 * Math.PI * m,
                4 * m + 1.25 * e);

            // Effective width for group plastic mechanism
            // Based on assumed yield lines across two rows (from literature/practice)
            double b_eff_group = isLastRow
                ? Math.PI * m + p             // Last row scenario (end connection)
                : 2 * m + 0.625 * e + 0.5 * p; // Internal group (intermediate rows)

            return (b_eff_indiv, b_eff_indiv, b_eff_group);
        }
    }
}