using System;
using System.Linq;

namespace SteelConnection
{
    public class Calculations
    {
        // Component 1: Column web panel in shear
        public class Component1Calculator
        {
            public static double CalculateComponent1(SteelProfiles profile)
            {
                if (profile.gamma_m0 == 0)
                    throw new ArgumentException("gamma_m0 cannot be zero.");

                double F_Rd_1 = (0.9 * profile.A_vc * profile.f_ywc) / (Math.Sqrt(3) * profile.gamma_m0);

                Console.WriteLine("=== Component 1: Column Web Panel in Shear ===");
                Console.WriteLine($"Shear Area of Column Web (A_vc) = {profile.A_vc:F2} mm²");
                Console.WriteLine($"Yield Strength of Column Web (f_y,wc) = {profile.f_ywc:F2} N/mm²");
                Console.WriteLine($"Partial Safety Factor (γ_M0) = {profile.gamma_m0:F2}");
                Console.WriteLine($"F_Rd,1 = {F_Rd_1:F2} N");

                return F_Rd_1;
            }
        }

        // Component 2: Column web in compression
        public class Component2Calculator
        {
            public static double CalculateComponent2(SteelProfiles profile)
            {
                if (profile.b_eff_avg == 0)
                    throw new ArgumentException("b_eff_avg cannot be zero.");
                if (profile.E == 0 || profile.t_fc == 0)
                    throw new ArgumentException("E and t_fc cannot be zero.");

                double b_eff_c_wc = profile.t_jb + 2 * profile.q_f * Math.Sqrt(2)
                                    + 5 * (profile.t_fc + profile.r_c) + profile.s_p;
                double omega_1 = 1 / Math.Sqrt(1 + 1.3 * Math.Pow(b_eff_c_wc / profile.b_eff_avg, 2));
                double lambda_p = 0.932 * Math.Sqrt((b_eff_c_wc * profile.d_wc * profile.fy_wc) / (profile.E * Math.Pow(profile.t_wc, 2))); // Corrected t_fc to t_wc
                double rho = lambda_p > 0.72 ? (lambda_p - 0.2) / lambda_p : 1;
                double F_Rd_2_1 = (omega_1 * b_eff_c_wc * profile.t_wc * profile.fy_wc * rho) / profile.gamma_m0; // Added t_wc

                Console.WriteLine("=== Component 2: Column Web in Compression ===");
                Console.WriteLine($"b_eff_c_wc = {b_eff_c_wc:F4} mm");
                Console.WriteLine($"omega_1 = {omega_1:F4}");
                Console.WriteLine($"lambda_p = {lambda_p:F4}");
                Console.WriteLine($"rho = {rho:F4}");
                Console.WriteLine($"F_Rd_2_1 = {F_Rd_2_1:F2} N");

                return F_Rd_2_1;
            }
        }

        // Component 3: Column web in tension
        public class Component3Calculator
        {
            public static (double l_eff_cp, double l_eff_nc) CalculateEffectiveLengthForComponent3(SteelProfiles profile, int row, string caseType, double p)
            {
                double m = profile.m;
                double e = (profile.b_c - profile.w) / 2;

                double l_eff_cp, l_eff_nc;

                switch (row)
                {
                    case 1:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "first_combined_with_next")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 1. Use 'individual' or 'first_combined_with_next'.");
                        }
                        break;

                    case 2:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "internal")
                        {
                            l_eff_cp = 2 * p;
                            l_eff_nc = p;
                        }
                        else if (caseType == "first" || caseType == "last")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 2. Use 'individual', 'internal', 'first', or 'last'.");
                        }
                        break;

                    case 3:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "last")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 3. Use 'individual' or 'last'.");
                        }
                        break;

                    case 4:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "last")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 4. Use 'individual' or 'last'.");
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid row number. Use 1, 2, 3, or 4.");
                }

                Console.WriteLine($"=== Effective Lengths for Component 3 (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff_cp = {l_eff_cp:F2} mm");
                Console.WriteLine($"l_eff_nc = {l_eff_nc:F2} mm");

                return (l_eff_cp, l_eff_nc);
            }

            public static double CalculateComponent3TwoRows(SteelProfiles profile)
            {
                if (profile.gamma_m0 == 0)
                    throw new ArgumentException("gamma_m0 cannot be zero.");

                double m_fc = (profile.m - profile.t_wc) / 2 - 0.8 * profile.r_c;
                double e_wc = Math.Min(profile.b_c_w / 2, profile.b_b_w / 2);
                double b_eff_t_wc = Math.Min((4 * profile.m + 1.25 * e_wc), 2 * Math.PI * profile.m);
                double beta = 1;
                double w_t = 0.837;
                double F_Rd_3_1 = (w_t * b_eff_t_wc * profile.t_wc * profile.fy_wc) / profile.gamma_m0;

                Console.WriteLine("=== Component 3: Column Web in Tension (Two Rows of Bolts) ===");
                Console.WriteLine($"m_fc = {m_fc:F2} mm");
                Console.WriteLine($"e_wc = {e_wc:F2} mm");
                Console.WriteLine($"b_eff_t_wc = {b_eff_t_wc:F2} mm");
                Console.WriteLine($"beta = {beta:F2}");
                Console.WriteLine($"w_t = {w_t:F2}");
                Console.WriteLine($"F_Rd_3_1 = {F_Rd_3_1:F2} N");

                return F_Rd_3_1;
            }

            public static double CalculateIndividualBoltRowResistance(SteelProfiles profile, int row, string caseType, int n_bolts, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                var (l_eff_cp, l_eff_nc) = CalculateEffectiveLengthForComponent3(profile, row, caseType, p);
                double l_eff = Math.Min(l_eff_cp, l_eff_nc);

                double b_eff = l_eff;
                double m_pl_Rd = (0.25 * l_eff * profile.t_wc * profile.t_wc * profile.fy_wc) / profile.gamma_m0; // Corrected to use t_wc

                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                double F_T_Rd_1 = (4 * l_eff * m_pl_Rd) / profile.m;
                double F_T_Rd_3 = sum_F_t_Rd;
                double n = Math.Min((profile.b_c - profile.w) / 2, 1.25 * profile.m);
                double F_T_Rd_2 = (2 * l_eff * m_pl_Rd + n * sum_F_t_Rd) / (profile.m + n);

                double F_t_wc_Rd = Math.Min(Math.Min(F_T_Rd_1, F_T_Rd_2), F_T_Rd_3);

                Console.WriteLine($"=== Component 3: Column Web in Tension (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff = {l_eff:F2} mm");
                Console.WriteLine($"m_pl_Rd = {m_pl_Rd:F2} N.mm");
                Console.WriteLine($"F_t_Rd (per bolt) = {F_t_Rd_per_bolt:F2} N");
                Console.WriteLine($"Sum F_t_Rd = {sum_F_t_Rd:F2} N");
                Console.WriteLine($"F_T_Rd_1 (Mode 1) = {F_T_Rd_1:F2} N");
                Console.WriteLine($"F_T_Rd_2 (Mode 2) = {F_T_Rd_2:F2} N");
                Console.WriteLine($"F_T_Rd_3 (Mode 3) = {F_T_Rd_3:F2} N");
                Console.WriteLine($"F_t_wc_Rd = {F_t_wc_Rd:F2} N");

                return F_t_wc_Rd;
            }

            public static double CalculateCombinedBoltRowsResistance(SteelProfiles profile, string rows, string caseType, int n_bolts, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                int row;
                if (rows.Contains("Rows 1 and 2"))
                    row = 1;
                else if (rows.Contains("Rows 2 and 3"))
                    row = 2;
                else if (rows.Contains("Rows 1, 2, and 3") || rows.Contains("Rows 1, 2, 3, and 4"))
                    row = 1;
                else
                    throw new ArgumentException("Invalid rows description.");

                var (l_eff_cp, l_eff_nc) = CalculateEffectiveLengthForComponent3(profile, row, caseType, p);
                double l_eff = Math.Min(l_eff_cp, l_eff_nc);

                double b_eff = l_eff;
                double m_pl_Rd = (0.25 * l_eff * profile.t_wc * profile.t_wc * profile.fy_wc) / profile.gamma_m0;

                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                double F_T_Rd_1 = (4 * l_eff * m_pl_Rd) / profile.m;
                double F_T_Rd_3 = sum_F_t_Rd;
                double n = Math.Min((profile.b_c - profile.w) / 2, 1.25 * profile.m);
                double F_T_Rd_2 = (2 * l_eff * m_pl_Rd + n * sum_F_t_Rd) / (profile.m + n);

                double F_t_wc_Rd = Math.Min(Math.Min(F_T_Rd_1, F_T_Rd_2), F_T_Rd_3);

                Console.WriteLine($"=== Component 3: Column Web in Tension (Combined {rows}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff = {l_eff:F2} mm");
                Console.WriteLine($"m_pl_Rd = {m_pl_Rd:F2} N.mm");
                Console.WriteLine($"F_t_Rd (per bolt) = {F_t_Rd_per_bolt:F2} N");
                Console.WriteLine($"Sum F_t_Rd = {sum_F_t_Rd:F2} N");
                Console.WriteLine($"F_T_Rd_1 (Mode 1) = {F_T_Rd_1:F2} N");
                Console.WriteLine($"F_T_Rd_2 (Mode 2) = {F_T_Rd_2:F2} N");
                Console.WriteLine($"F_T_Rd_3 (Mode 3) = {F_T_Rd_3:F2} N");
                Console.WriteLine($"F_t_wc_Rd = {F_t_wc_Rd:F2} N");

                return F_t_wc_Rd;
            }
        }

        // Component 4: Column flange in bending
        public class Component4Calculator
        {
            public static (double l_eff_cp, double l_eff_nc) CalculateEffectiveLengthForComponent4(SteelProfiles profile, int row, string caseType, double p)
            {
                double m = profile.m;
                double e = (profile.b_c - profile.w) / 2;

                double l_eff_cp, l_eff_nc;

                switch (row)
                {
                    case 1:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "first_combined_with_next")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 1.");
                        }
                        break;

                    case 2:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "internal")
                        {
                            l_eff_cp = 2 * p;
                            l_eff_nc = p;
                        }
                        else if (caseType == "first" || caseType == "last")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 2.");
                        }
                        break;

                    case 3:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "last")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 3.");
                        }
                        break;

                    case 4:
                        if (caseType == "individual")
                        {
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "last")
                        {
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 4.");
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid row number.");
                }

                Console.WriteLine($"=== Effective Lengths for Component 4 (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff_cp = {l_eff_cp:F2} mm");
                Console.WriteLine($"l_eff_nc = {l_eff_nc:F2} mm");

                return (l_eff_cp, l_eff_nc);
            }

            public static double CalculateComponent4(SteelProfiles profile, int n_bolts, int row, string caseType, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                var (l_eff_cp, l_eff_nc) = CalculateEffectiveLengthForComponent4(profile, row, caseType, p);
                double b_eff_t_fc = Math.Min(l_eff_cp, l_eff_nc);

                double e = (profile.b_c - profile.w) / 2;
                double e_min = Math.Min(e, profile.w / 2);
                double n = Math.Min(e_min, 1.25 * profile.m);

                double m_pl_Rd = (0.25 * b_eff_t_fc * profile.t_fc * profile.t_fc * profile.f_y) / profile.gamma_m0;

                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                double F_fc_Rd_1 = (4 * m_pl_Rd) / profile.m;
                double F_fc_Rd_2 = (2 * m_pl_Rd + n * sum_F_t_Rd) / (profile.m + n);
                double F_Rd_4_1 = Math.Min(F_fc_Rd_1, F_fc_Rd_2);

                Console.WriteLine($"=== Component 4: Column Flange in Bending (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff_cp = {l_eff_cp:F2} mm");
                Console.WriteLine($"l_eff_nc = {l_eff_nc:F2} mm");
                Console.WriteLine($"b_eff_t_fc = {b_eff_t_fc:F2} mm");
                Console.WriteLine($"e = {e:F2} mm");
                Console.WriteLine($"e_min = {e_min:F2} mm");
                Console.WriteLine($"n = {n:F2} mm");
                Console.WriteLine($"m_pl_Rd = {m_pl_Rd:F2} N.mm");
                Console.WriteLine($"F_t_Rd (per bolt) = {F_t_Rd_per_bolt:F2} N");
                Console.WriteLine($"Sum F_t_Rd = {sum_F_t_Rd:F2} N");
                Console.WriteLine($"F_fc_Rd_1 (Mode 1) = {F_fc_Rd_1:F2} N");
                Console.WriteLine($"F_fc_Rd_2 (Mode 2) = {F_fc_Rd_2:F2} N");
                Console.WriteLine($"F_Rd_4_1 = {F_Rd_4_1:F2} N");

                return F_Rd_4_1;
            }
        }

        // Component 5: End plate in bending
        public class Component5Calculator
        {
            private static double CalculateAlpha(double lambda, double mu)
            {
                // Placeholder for alpha calculation based on Eurocode 3 Table 6.11
                // Simplified assumption based on typical values
                if (lambda <= 0.5)
                    return 4;
                else if (lambda <= 1.0)
                    return 5;
                else
                    return 6;
            }

            public static (double[] l_eff_1, double[] l_eff_2) CalculateLEffectivePatterns(SteelProfiles profile, int row, string caseType, double p)
            {
                double m_x = profile.w / 2 - 0.8 * profile.a_f * Math.Sqrt(2) - profile.e_x;
                double m = profile.m;
                double e = (profile.b_p - profile.w) / 2;

                // Calculate alpha based on geometric ratios
                double lambda = e / m;
                double mu = profile.w / (2 * m);
                double alpha = CalculateAlpha(lambda, mu);

                double[] l_eff_1 = new double[3];
                double[] l_eff_2 = new double[3];

                switch (row)
                {
                    case 1:
                        if (caseType == "individual")
                        {
                            l_eff_1[0] = 2 * Math.PI * m_x;
                            l_eff_1[1] = Math.PI * m_x + profile.w;
                            l_eff_1[2] = Math.PI * m_x + 2 * e;

                            l_eff_2[0] = 4 * m_x + 1.25 * profile.e_x;
                            l_eff_2[1] = e + 2 * m_x + 0.625 * profile.e_x;
                            l_eff_2[2] = Math.Min(0.5 * profile.b_p, 0.5 * profile.w + 2 * m_x + 0.625 * profile.e_x);
                        }
                        else if (caseType == "first_combined_with_next")
                        {
                            l_eff_1[0] = Math.PI * m_x + p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 2 * m_x + 0.625 * profile.e_x + 0.5 * p; // Corrected formula
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 1.");
                        }
                        break;

                    case 2:
                        if (caseType == "individual")
                        {
                            l_eff_1[0] = 2 * Math.PI * m;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = alpha * m;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else if (caseType == "first_combined_with_next")
                        {
                            l_eff_1[0] = Math.PI * m + p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 0.5 * p + alpha * m - (2 * m + 0.625 * e);
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else if (caseType == "last_combined_with_prev")
                        {
                            l_eff_1[0] = Math.PI * m + p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 2 * m + 0.625 * e + 0.5 * p;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else if (caseType == "internal")
                        {
                            l_eff_1[0] = 2 * p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = p;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 2.");
                        }
                        break;

                    case 3:
                        if (caseType == "individual")
                        {
                            l_eff_1[0] = 2 * Math.PI * m;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 4 * m + 1.25 * e;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else if (caseType == "last_combined_with_prev")
                        {
                            l_eff_1[0] = Math.PI * m + p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 2 * m + 0.625 * e + 0.5 * p;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else if (caseType == "internal")
                        {
                            l_eff_1[0] = 2 * p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = p;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 3.");
                        }
                        break;

                    case 4:
                        if (caseType == "individual")
                        {
                            l_eff_1[0] = 2 * Math.PI * m;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 4 * m + 1.25 * e;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else if (caseType == "last_combined_with_prev")
                        {
                            l_eff_1[0] = Math.PI * m + p;
                            l_eff_1[1] = double.MaxValue;
                            l_eff_1[2] = double.MaxValue;

                            l_eff_2[0] = 2 * m + 0.625 * e + 0.5 * p;
                            l_eff_2[1] = double.MaxValue;
                            l_eff_2[2] = double.MaxValue;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 4.");
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid row number.");
                }

                Console.WriteLine($"=== Effective Lengths for Component 5 (Row {row}, Case: {caseType}) ===");
                Console.WriteLine("Mode 1 Patterns (l_eff_1):");
                for (int i = 0; i < l_eff_1.Length; i++)
                {
                    if (l_eff_1[i] != double.MaxValue)
                        Console.WriteLine($"Pattern {i + 1}: {l_eff_1[i]:F2} mm");
                }
                Console.WriteLine("Mode 2 Patterns (l_eff_2):");
                for (int i = 0; i < l_eff_2.Length; i++)
                {
                    if (l_eff_2[i] != double.MaxValue)
                        Console.WriteLine($"Pattern {i + 1}: {l_eff_2[i]:F2} mm");
                }

                return (l_eff_1, l_eff_2);
            }

            public static double CalculateComponent5(SteelProfiles profile, int n_bolts, int row, string caseType, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                var (l_eff_1, l_eff_2) = CalculateLEffectivePatterns(profile, row, caseType, p);

                double l_eff_1_min = l_eff_1.Where(l => l != double.MaxValue).Min();
                double l_eff_2_min = l_eff_2.Where(l => l != double.MaxValue).Min();

                double m_x = profile.w / 2 - 0.8 * profile.a_f * Math.Sqrt(2) - profile.e_x;
                double n_x = Math.Min(profile.e_x, 1.25 * m_x);

                double m_pl_Rd = (l_eff_1_min * profile.t_p * profile.t_p * profile.f_yp) / (4 * profile.gamma_m0);

                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                double F_T_Rd_1 = (4 * m_pl_Rd) / m_x;
                double F_T_Rd_2 = (2 * m_pl_Rd + n_x * sum_F_t_Rd) / (m_x + n_x);
                double F_T_Rd_3 = sum_F_t_Rd;

                double F_Rd_5 = Math.Min(Math.Min(F_T_Rd_1, F_T_Rd_2), F_T_Rd_3);

                Console.WriteLine($"=== Component 5: End Plate in Bending (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"Minimum l_eff_1 (Mode 1) = {l_eff_1_min:F2} mm");
                Console.WriteLine($"Minimum l_eff_2 (Mode 2) = {l_eff_2_min:F2} mm");
                Console.WriteLine($"m_pl_Rd = {m_pl_Rd:F2} N.mm");
                Console.WriteLine($"F_T_Rd_1 (Mode 1) = {F_T_Rd_1:F2} N");
                Console.WriteLine($"F_T_Rd_2 (Mode 2) = {F_T_Rd_2:F2} N");
                Console.WriteLine($"F_T_Rd_3 (Mode 3) = {F_T_Rd_3:F2} N");
                Console.WriteLine($"F_Rd_5 = {F_Rd_5:F2} N");

                return F_Rd_5;
            }
        }

        // Component 7: Beam flange and web in compression
        public class Component7Calculator
        {
            public static double CalculateComponent7(SteelProfiles profile)
            {
                if (profile.gamma_m0 == 0)
                    throw new ArgumentException("gamma_m0 cannot be zero.");
                if (profile.h_b - profile.t_jb == 0)
                    throw new ArgumentException("(h_b - t_jb) cannot be zero.");

                double M_b_pl_Rd_class1 = (profile.W_pl_y * profile.f_yb) / profile.gamma_m0;
                double F_Rd_7_1 = M_b_pl_Rd_class1 / (profile.h_b - profile.t_jb);

                Console.WriteLine("=== Component 7: Beam Flange and Web in Compression ===");
                Console.WriteLine($"M_b_pl_Rd_class1 = {M_b_pl_Rd_class1:F2} N.mm");
                Console.WriteLine($"F_Rd_7_1 = {F_Rd_7_1:F2} N");

                return F_Rd_7_1;
            }
        }

        // Component 8: Beam web in tension
        public class Component8Calculator
        {
            public static double CalculateComponent8(SteelProfiles profile, int row, double b_eff_t_wb, string caseType)
            {
                if (profile.gamma_m0 == 0)
                    throw new ArgumentException("gamma_m0 cannot be zero.");

                double F_t_wb_Rd = (b_eff_t_wb * profile.t_wb * profile.f_ywb) / profile.gamma_m0;

                Console.WriteLine($"=== Component 8: Beam Web in Tension (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"Effective Length (b_eff,t,wb) = {b_eff_t_wb:F2} mm");
                Console.WriteLine($"Beam Web Thickness (t_wb) = {profile.t_wb:F2} mm");
                Console.WriteLine($"Yield Strength of Beam Web (f_y,wb) = {profile.f_ywb:F2} N/mm²");
                Console.WriteLine($"F_t,wb,Rd = {F_t_wb_Rd:F2} N");

                return F_t_wb_Rd;
            }
        }

        // Component 10: Bolts in tension
        public class Component10Calculator
        {
            public static double CalculateComponent10(SteelProfiles profile, int n_bolts, int row, string caseType)
            {
                if (profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m2 cannot be zero.");

                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                Console.WriteLine($"=== Component 10: Bolts in Tension (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"Tensile Stress Area of Bolt (A_s) = {profile.A_s:F2} mm²");
                Console.WriteLine($"Ultimate Tensile Strength of Bolt (f_ub) = {profile.f_ub:F2} N/mm²");
                Console.WriteLine($"Number of Bolts in Row = {n_bolts}");
                Console.WriteLine($"F_t,Rd per Bolt = {F_t_Rd_per_bolt:F2} N");
                Console.WriteLine($"Total F_t,Rd for Row = {F_t_Rd:F2} N");

                return F_t_Rd;
            }
        }
    }
}