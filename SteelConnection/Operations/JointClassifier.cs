namespace SteelConnection
{
    public class JointClassifier
    {
        public static (string StiffnessClassification, string StrengthClassification) ClassifyJoint(
            SteelProfiles profile, double S_j_ini, double M_j_Rd)
        {
            if (S_j_ini < 0)
                throw new ArgumentException("S_j,ini must be non-negative.");
            if (M_j_Rd < 0)
                throw new ArgumentException("M_j,Rd must be non-negative.");
            if (profile.L_b <= 0)
                throw new ArgumentException("L_b must be greater than zero.");
            if (profile.I_b <= 0)
                throw new ArgumentException("I_b must be greater than zero.");
            if (profile.W_pl_y <= 0)
                throw new ArgumentException("W_pl_y must be greater than zero.");
            if (profile.gamma_m0 <= 0)
                throw new ArgumentException("gamma_m0 must be greater than zero.");

            double EI_b_over_L_b = (profile.E * profile.I_b) / profile.L_b;

            double pinnedLimit = 0.5 * EI_b_over_L_b;
            double rigidLimitUnbraced = 8 * EI_b_over_L_b;
            double rigidLimitBraced = 25 * EI_b_over_L_b;

            string stiffnessClassification;
            if (S_j_ini <= pinnedLimit)
            {
                stiffnessClassification = "Pinned";
            }
            else if (S_j_ini >= rigidLimitBraced)
            {
                stiffnessClassification = "Rigid (for both braced and unbraced frames)";
            }
            else if (S_j_ini >= rigidLimitUnbraced)
            {
                stiffnessClassification = "Rigid (for unbraced frames), Semi-Rigid (for braced frames)";
            }
            else
            {
                stiffnessClassification = "Semi-Rigid";
            }

            double M_b_pl_Rd = (profile.W_pl_y * profile.f_yb) / profile.gamma_m0;

            string strengthClassification;
            if (M_j_Rd >= M_b_pl_Rd)
            {
                strengthClassification = "Full-Strength";
            }
            else
            {
                strengthClassification = "Partial-Strength";
            }

            Console.WriteLine("=== Classification of Joints ===");
            Console.WriteLine($"--- Stiffness Classification ---");
            Console.WriteLine($"S_j,ini = {S_j_ini:F2} kN·m/rad");
            Console.WriteLine($"EI_b / L_b = {EI_b_over_L_b:F2} kN·m/rad");
            Console.WriteLine($"Pinned Limit (0.5 * EI_b / L_b) = {pinnedLimit:F2} kN·m/rad");
            Console.WriteLine($"Rigid Limit (Unbraced, k_b = 8) = {rigidLimitUnbraced:F2} kN·m/rad");
            Console.WriteLine($"Rigid Limit (Braced, k_b = 25) = {rigidLimitBraced:F2} kN·m/rad");
            Console.WriteLine($"Stiffness Classification: {stiffnessClassification}");
            Console.WriteLine($"--- Strength Classification ---");
            Console.WriteLine($"M_j,Rd = {M_j_Rd:F2} kN·m");
            Console.WriteLine($"M_b,pl,Rd = {M_b_pl_Rd:F2} kN·m");
            Console.WriteLine($"Strength Classification: {strengthClassification}");

            return (stiffnessClassification, strengthClassification);
        }
    }
}