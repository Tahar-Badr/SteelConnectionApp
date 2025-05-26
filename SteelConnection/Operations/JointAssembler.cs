using System;
using SteelConnection.Models;
using SteelConnection.Operations.Column;
using SteelConnection.Operations.Result;

namespace SteelConnection.Operations
{
    internal static class JointAssembler
    {
        public static JointOutput Calculate(JointInput input)
        {
            var results = new JointOutput();

            // Component calculations
            results.V_wpRd = ColumnWebPanelShear.Calculate(input);
            results.F_cwcRd = ColumnWebCompression.Calculate(input);
            results.Mj_Rd = CalculateMomentResistance(input);
            results.Sj_ini = input.Sj_ini; // Capture from calculation

            // Classification
            results.Classification = new
            {
                Stiffness = JointClassifier.ClassifyStiffness(
                    results.Sj_ini, input.EIb, input.L_b, input.IsBraced),
                Strength = JointClassifier.ClassifyStrength(
                    results.Mj_Rd, input.Mpl_Rd)
            };

            return results;
        }

        private static double CalculateMomentResistance(JointInput input)
        {
            var designResults = JointMomentResistance.CalculateJointDesign(
                input.A_vc, input.Beta, input.B_eff_c, input.T_wc, input.H_wc,
                input.Leff_tfc, input.T_fc, input.M_fc,
                input.Leff_tp, input.T_p, input.M_p,
                input.A_s, input.L_b, input.F_Rd, input.H, input.E);

            input.Sj_ini = designResults.InitialStiffness;
            return designResults.PlasticMomentResistance;
        }
    }
}
