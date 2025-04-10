using System;
using SteelConnection.Data; // ? ??????? ????????? ???? ??? ???? Profile

namespace SteelConnection.Operations
{
    public class Calculations
    {
        public class Component2Calculator
        {
            public static void CalculateComponent2(Profile profile)
            {
                // Step 1: b_eff,c,wc
                double b_eff_c_wc = profile.t_jb + 2 * profile.q_f * Math.Sqrt(2)
                                    + 5 * (profile.t_fc + profile.r_c) + profile.s_p;

                // Step 2: ??
                double omega_1 = 1 / Math.Sqrt(1 + 1.3 * Math.Pow(b_eff_c_wc / profile.b_eff_avg, 2));

                // Step 3: ??_p
                double lambda_p = 0.932 * Math.Sqrt((b_eff_c_wc * profile.d_wc * profile.fy_wc) / (profile.E * profile.t_fc));

                // Step 4: ?
                double rho = lambda_p > 0.72 ? (lambda_p - 0.2) / lambda_p : 1;

                // Step 5: F_Rd,2,(1)
                double F_Rd_2_1 = (omega_1 * b_eff_c_wc * profile.fy_wc * rho) / profile.gamma_m0;

                // Output
                Console.WriteLine("=== Component 2: Column Web in Compression ===");
                Console.WriteLine($"b_eff_c_wc = {b_eff_c_wc:F4} mm");
                Console.WriteLine($"omega_1 = {omega_1:F4}");
                Console.WriteLine($"lambda_p = {lambda_p:F4}");
                Console.WriteLine($"rho = {rho:F4}");
                Console.WriteLine($"F_Rd_2_1 = {F_Rd_2_1:F2} N");
            }
            public class Component7Calculator
            {
                /// <summary>
                /// Calculates the design resistance of the beam flange and web in compression (Component 7) according to Eurocode 03.
                /// </summary>
                /// <param name="profile">Profile object containing geometric and material properties.</param>
                /// <returns>Design resistance F_Rd,7,(1) in Newtons.</returns>
                /// <exception cref="ArgumentException">Thrown if some variables are zero.</exception>
                public static double CalculateComponent7(Profile profile)
                {
                    // Validate inputs to avoid division by zero
                    if (profile.gamma_m0 == 0)
                        throw new ArgumentException("gamma_m0 cannot be zero.");
                    if (profile.h_b - profile.t_jb == 0)
                        throw new ArgumentException("(h_b - t_jb) cannot be zero.");

                    // Step 1: Calculate M_b,pl,Rd(class1)
                    // W_pl,y: Plastic section modulus about the y-axis, f_yb: Yield strength of the beam, gamma_m0: Partial safety factor
                    double M_b_pl_Rd_class1 = (profile.W_pl_y * profile.f_yb) / profile.gamma_m0;

                    // Step 2: Calculate F_Rd,7,(1)
                    // h_b: Beam height, t_jb: Beam flange thickness
                    // Note: We assume M_b,Rd = M_b,pl,Rd(class1) since the section is Class 1
                    double F_Rd_7_1 = M_b_pl_Rd_class1 / (profile.h_b - profile.t_jb);

                    // Output for debugging purposes
                    Console.WriteLine("=== Component 7: Beam Flange and Web in Compression ===");
                    Console.WriteLine($"M_b_pl_Rd_class1 = {M_b_pl_Rd_class1:F2} N.mm");
                    Console.WriteLine($"F_Rd_7_1 = {F_Rd_7_1:F2} N");

                    return F_Rd_7_1;
                }
            }
        }
    }
} 
