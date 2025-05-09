namespace SteelConnection
{
    public class RotationalStiffnessCalculator
    {
        public static double CalculateRotationalStiffness(SteelProfiles profile, double[] h, double[] b_eff_t_wc, double[] l_eff_fc, double[] l_eff_ep, double beta)
        {
            if (h.Length == 0 || h.Length != b_eff_t_wc.Length || h.Length != l_eff_fc.Length || h.Length != l_eff_ep.Length)
                throw new ArgumentException("Input arrays must have the same non-zero length.");
            if (beta <= 0)
                throw new ArgumentException("beta must be greater than zero.");

            if (profile.d_wc == 0)
                throw new ArgumentException("d_wc cannot be zero.");
            double k_2 = (0.7 * profile.b_eff_c_wc * profile.t_wc) / profile.d_wc;

            double k_7 = double.PositiveInfinity;

            double[] k_eff = new double[h.Length];
            double sum_k_eff_h = 0;
            double sum_k_eff_h2 = 0;

            for (int i = 0; i < h.Length; i++)
            {
                double k_3 = (0.7 * b_eff_t_wc[i] * profile.t_wc) / profile.d_wc;
                if (k_3 <= 0)
                    throw new ArgumentException($"k_3 for Row {i + 1} must be greater than zero.");

                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");
                double k_4 = (0.9 * l_eff_fc[i] * Math.Pow(profile.t_fc, 3)) / Math.Pow(profile.m, 3);
                if (k_4 <= 0)
                    throw new ArgumentException($"k_4 for Row {i + 1} must be greater than zero.");

                double k_5 = (0.9 * l_eff_ep[i] * Math.Pow(profile.t_p, 3)) / Math.Pow(profile.m, 3);
                if (k_5 <= 0)
                    throw new ArgumentException($"k_5 for Row {i + 1} must be greater than zero.");

                if (profile.L_b_bolt == 0)
                    throw new ArgumentException("L_b_bolt cannot be zero.");
                double k_10 = (1.6 * profile.A_s) / profile.L_b_bolt;
                if (k_10 <= 0)
                    throw new ArgumentException($"k_10 for Row {i + 1} must be greater than zero.");

                k_eff[i] = 1 / (1 / k_3 + 1 / k_4 + 1 / k_5 + 1 / k_10);
                sum_k_eff_h += k_eff[i] * h[i];
                sum_k_eff_h2 += k_eff[i] * h[i] * h[i];
            }

            if (sum_k_eff_h == 0)
                throw new ArgumentException("Sum of k_eff * h cannot be zero.");
            double z_eq = sum_k_eff_h2 / sum_k_eff_h;

            if (z_eq == 0)
                throw new ArgumentException("z_eq cannot be zero.");
            double k_1 = (0.38 * profile.A_vc) / (beta * z_eq);
            if (k_1 <= 0)
                throw new ArgumentException("k_1 must be greater than zero.");

            double k_eq = sum_k_eff_h / z_eq;

            double denominator = (1 / k_1) + (1 / k_2) + (1 / k_7) + (1 / k_eq);
            if (denominator == 0)
                throw new ArgumentException("Denominator for S_j,ini calculation cannot be zero.");
            double S_j_ini = (profile.E * z_eq * z_eq) / denominator;

            S_j_ini = S_j_ini / 1000000; // Convert to kN·m/rad

            Console.WriteLine("=== Determination of the Rotational Stiffness ===");
            Console.WriteLine($"k_1 (Column Web in Shear) = {k_1:F2}");
            Console.WriteLine($"k_2 (Column Web in Compression) = {k_2:F2}");
            Console.WriteLine($"k_7 (Beam Flange and Web in Compression) = {k_7:F2}");
            for (int i = 0; i < h.Length; i++)
            {
                Console.WriteLine($"k_eff (Row {i + 1}) = {k_eff[i]:F2}");
            }
            Console.WriteLine($"z_eq (Equivalent Lever Arm) = {z_eq:F2} mm");
            Console.WriteLine($"k_eq = {k_eq:F2}");
            Console.WriteLine($"Initial Rotational Stiffness (S_j,ini) = {S_j_ini:F2} kN·m/rad");

            return S_j_ini;
        }
    }
}