using System;

namespace SteelConnection.Operations.Result
{
    internal static class JointClassifier
    {
        /// <summary>
        /// Classifies joint stiffness per EN 1993-1-8 §5.2.2.5
        /// </summary>
        public static JointStiffnessClass ClassifyStiffness(
            double Sj_ini,     // Initial rotational stiffness (kNm/rad)
            double EI_b,       // Beam flexural rigidity (kNm²)
            double L_b,        // Beam length (m)
            bool isBraced = false)
        {
            double boundary = EI_b / L_b;

            if (isBraced)
            {
                return Sj_ini >= 8 * boundary ? JointStiffnessClass.Rigid
                     : Sj_ini <= 0.5 * boundary ? JointStiffnessClass.Pinned
                     : JointStiffnessClass.SemiRigid;
            }
            else
            {
                return Sj_ini >= 25 * boundary ? JointStiffnessClass.Rigid
                     : Sj_ini <= 0.5 * boundary ? JointStiffnessClass.Pinned
                     : JointStiffnessClass.SemiRigid;
            }
        }

        /// <summary>
        /// Classifies joint strength per EN 1993-1-8 §5.2.2.4
        /// </summary>
        public static JointStrengthClass ClassifyStrength(
            double Mj_Rd,      // Joint design moment resistance (kNm)
            double Mpl_Rd)     // Beam plastic moment capacity (kNm)
        {
            return Mj_Rd >= Mpl_Rd ? JointStrengthClass.FullStrength
                 : Mj_Rd <= 0.25 * Mpl_Rd ? JointStrengthClass.Pinned
                 : JointStrengthClass.PartialStrength;
        }

        public enum JointStiffnessClass { Pinned, SemiRigid, Rigid }
        public enum JointStrengthClass { Pinned, PartialStrength, FullStrength }
    }
}
