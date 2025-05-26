using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Operations
{
    internal class JointMomentResistance
    {
        public static class JointMomentResistance
        {
            /// <summary>
            /// Calculates all stiffness components for moment resistance evaluation
            /// </summary>
            public static (double K1, double K2, double[] K3, double[] K4, double[] K5, double K10)
                CalculateStiffnessComponents(
                    double A_vc, double beta, double z_eq,
                    double b_eff_c, double t_wc, double h_wc,
                    double[] leff_tfc, double t_fc, double[] m_fc,
                    double[] leff_tp, double t_p, double[] m_p,
                    double A_s, double L_b)
            {
                // K1: Column web panel in shear
                double K1 = (0.38 / beta) * (A_vc / z_eq);

                // K2: Column web in compression
                double K2 = (0.7 * b_eff_c * t_wc) / h_wc;

                // K3: Column web in tension (per bolt row)
                double[] K3 = leff_tfc.Select(_ => (0.7 * b_eff_c * t_wc) / h_wc).ToArray();

                // K4: Column flange in bending (per bolt row)
                double[] K4 = leff_tfc.Zip(m_fc, (leff, m) =>
                    0.9 * leff * Math.Pow(t_fc, 3) / Math.Pow(m, 3)).ToArray();

                // K5: End plate in bending (per bolt row)
                double[] K5 = leff_tp.Zip(m_p, (leff, m) =>
                    0.9 * leff * Math.Pow(t_p, 3) / Math.Pow(m, 3)).ToArray();

                // K10: Bolts in tension
                double K10 = (1.6 * A_s) / L_b;

                return (K1, K2, K3, K4, K5, K10);
            }

            /// <summary>
            /// Calculates the equivalent lever arm and stiffness
            /// </summary>
            public static (double z_eq, double K_eq) CalculateEquivalentLeverArm(
                double[] K_eff, double[] h)
            {
                double sumKh = K_eff.Zip(h, (k, hi) => k * hi).Sum();
                double sumKh2 = K_eff.Zip(h, (k, hi) => k * Math.Pow(hi, 2)).Sum();

                double z_eq = sumKh2 / sumKh;
                double K_eq = sumKh / z_eq;

                return (z_eq, K_eq);
            }

            /// <summary>
            /// Evaluates the full moment resistance of the joint
            /// </summary>
            public static (double M_jRd, double M_eRd, double S_jini, double S_j)
                EvaluateMomentResistance(
                    double[] F_Rd, double[] h,
                    double E, double K1, double K2, double K_eq)
            {
                // Plastic moment resistance (Eq. 6.25)
                double M_jRd = F_Rd.Zip(h, (f, hi) => f * hi).Sum();

                // Elastic moment resistance (2/3 rule)
                double M_eRd = (2.0 / 3.0) * M_jRd;

                // Initial rotational stiffness (Eq. 6.27)
                double S_jini = (E * Math.Pow(h.Average(), 2)) /
                               (1 / K1 + 1 / K2 + 1 / double.PositiveInfinity + 1 / K_eq);

                // Secant stiffness (50% reduction)
                double S_j = S_jini / 2;

                return (M_jRd, M_eRd, S_jini, S_j);
            }

            /// <summary>
            /// Main calculation method combining all steps
            /// </summary>
            public static JointDesignResults CalculateJointDesign(
                double A_vc, double beta, double b_eff_c, double t_wc, double h_wc,
                double[] leff_tfc, double t_fc, double[] m_fc,
                double[] leff_tp, double t_p, double[] m_p,
                double A_s, double L_b, double[] F_Rd, double[] h, double E)
            {
                // Step 1: Calculate stiffness components
                var stiffness = CalculateStiffnessComponents(
                    A_vc, beta, h.Average(), b_eff_c, t_wc, h_wc,
                    leff_tfc, t_fc, m_fc, leff_tp, t_p, m_p, A_s, L_b);

                // Step 2: Calculate effective stiffness per bolt row
                double[] K_eff = new double[F_Rd.Length];
                for (int i = 0; i < F_Rd.Length; i++)
                {
                    K_eff[i] = 1.0 / (1 / stiffness.K3[i] + 1 / stiffness.K4[i] +
                                      1 / stiffness.K10 + 1 / stiffness.K5[i]);
                }

                // Step 3: Determine equivalent properties
                var (z_eq, K_eq) = CalculateEquivalentLeverArm(K_eff, h);

                // Step 4: Evaluate moment resistance
                var (M_jRd, M_eRd, S_jini, S_j) =
                    EvaluateMomentResistance(F_Rd, h, E, stiffness.K1, stiffness.K2, K_eq);

                return new JointDesignResults
                {
                    PlasticMomentResistance = M_jRd,
                    ElasticMomentResistance = M_eRd,
                    InitialStiffness = S_jini,
                    SecantStiffness = S_j,
                    EquivalentLeverArm = z_eq
                };
            }
        }

        public class JointDesignResults
        {
            public double PlasticMomentResistance { get; set; } // M_jRd (kNm)
            public double ElasticMomentResistance { get; set; } // M_eRd (kNm)
            public double InitialStiffness { get; set; }        // S_jini (kNm/rad)
            public double SecantStiffness { get; set; }         // S_j (kNm/rad)
            public double EquivalentLeverArm { get; set; }      // z_eq (mm)
        }
    }
}
