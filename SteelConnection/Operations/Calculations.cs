using System;

namespace SteelConnection
{
    // Class representing the steel profile with all required properties
    public class SteelProfile
    {
        // Beam flange thickness (mm)
        public double t_jb { get; set; }

        // Distance related to beam flange (mm)
        public double q_f { get; set; }

        // Column flange thickness (mm)
        public double t_fc { get; set; }

        // Column root radius (mm)
        public double r_c { get; set; }

        // Distance related to stiffeners or spacing (mm)
        public double s_p { get; set; }

        // Average effective width (mm)
        public double b_eff_avg { get; set; }

        // Column web depth (mm)
        public double d_wc { get; set; }

        // Yield strength of the column web (N/mm²)
        public double fy_wc { get; set; }

        // Modulus of elasticity (N/mm²)
        public double E { get; set; }

        // Plastic section modulus about y-axis (mm³)
        public double W_pl_y { get; set; }

        // Yield strength of the beam (N/mm²)
        public double f_yb { get; set; }

        // Beam height (mm)
        public double h_b { get; set; }

        // Distance related to bolt row geometry (mm)
        public double m { get; set; }

        // Column web thickness (mm)
        public double t_wc { get; set; }

        // Column web width (mm)
        public double b_c_w { get; set; }

        // Beam web width (mm)
        public double b_b_w { get; set; }

        // Column width (mm)
        public double b_c { get; set; }

        // Web width or related dimension (mm)
        public double w { get; set; }

        // Flange thickness (mm)
        public double t_f { get; set; }

        // Yield strength (N/mm²)
        public double f_y { get; set; }

        // Bolt cross-sectional area (mm²)
        public double A_s { get; set; }

        // Bolt ultimate strength (N/mm²)
        public double f_ub { get; set; }

        // Partial safety factor
        public double gamma_m0 { get; set; }

        // Partial safety factor for bolts
        public double gamma_m2 { get; set; }
    }

    // Class containing all calculations for steel connection components
    public class Calculations
    {
        // Component 2: Column web in compression
        public class Component2Calculator
        {
            /// <summary>
            /// Calculates the design resistance of the column web in compression (Component 2) according to Eurocode 03.
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <returns>Design resistance F_Rd,2,(1) in Newtons.</returns>
            /// <exception cref="ArgumentException">Thrown if some variables are zero.</exception>
            public static double CalculateComponent2(SteelProfile profile)
            {
                // Validate inputs to avoid division by zero
                if (profile.b_eff_avg == 0)
                    throw new ArgumentException("b_eff_avg cannot be zero.");
                if (profile.E == 0 || profile.t_fc == 0)
                    throw new ArgumentException("E and t_fc cannot be zero.");

                // Step 1: Calculate b_eff,c,wc (effective width of the column web in compression)
                double b_eff_c_wc = profile.t_jb + 2 * profile.q_f * Math.Sqrt(2)
                                    + 5 * (profile.t_fc + profile.r_c) + profile.s_p;

                // Step 2: Calculate ω₁ (factor for effective width)
                double omega_1 = 1 / Math.Sqrt(1 + 1.3 * Math.Pow(b_eff_c_wc / profile.b_eff_avg, 2));

                // Step 3: Calculate λ̄_p (slenderness of the web)
                double lambda_p = 0.932 * Math.Sqrt((b_eff_c_wc * profile.d_wc * profile.fy_wc) / (profile.E * profile.t_fc));

                // Step 4: Calculate ρ (reduction factor)
                double rho = lambda_p > 0.72 ? (lambda_p - 0.2) / lambda_p : 1;

                // Step 5: Calculate F_Rd,2,(1) (design resistance)
                double F_Rd_2_1 = (omega_1 * b_eff_c_wc * profile.fy_wc * rho) / profile.gamma_m0;

                // Output for debugging purposes
                Console.WriteLine("=== Component 2: Column Web in Compression ===");
                Console.WriteLine($"b_eff_c_wc = {b_eff_c_wc:F4} mm");
                Console.WriteLine($"omega_1 = {omega_1:F4}");
                Console.WriteLine($"lambda_p = {lambda_p:F4}");
                Console.WriteLine($"rho = {rho:F4}");
                Console.WriteLine($"F_Rd_2_1 = {F_Rd_2_1:F2} N");

                return F_Rd_2_1;
            }
        }

        // Component 7: Beam flange and web in compression
        public class Component7Calculator
        {
            /// <summary>
            /// Calculates the design resistance of the beam flange and web in compression (Component 7) according to Eurocode 03.
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <returns>Design resistance F_Rd,7,(1) in Newtons.</returns>
            /// <exception cref="ArgumentException">Thrown if some variables are zero.</exception>
            public static double CalculateComponent7(SteelProfile profile)
            {
                // Validate inputs to avoid division by zero
                if (profile.gamma_m0 == 0)
                    throw new ArgumentException("gamma_m0 cannot be zero.");
                if (profile.h_b - profile.t_jb == 0)
                    throw new ArgumentException("(h_b - t_jb) cannot be zero.");

                // Step 1: Calculate M_b,pl,Rd(class1)
                double M_b_pl_Rd_class1 = (profile.W_pl_y * profile.f_yb) / profile.gamma_m0;

                // Step 2: Calculate F_Rd,7,(1)
                double F_Rd_7_1 = M_b_pl_Rd_class1 / (profile.h_b - profile.t_jb);

                // Output for debugging purposes
                Console.WriteLine("=== Component 7: Beam Flange and Web in Compression ===");
                Console.WriteLine($"M_b_pl_Rd_class1 = {M_b_pl_Rd_class1:F2} N.mm");
                Console.WriteLine($"F_Rd_7_1 = {F_Rd_7_1:F2} N");

                return F_Rd_7_1;
            }
        }

        // Component 3: Column web in tension
        public class Component3Calculator
        {
            /// <summary>
            /// Calculates the effective length l_eff for a bolt row based on its position and specific case.
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <param name="row">Row number (1, 2, or 3).</param>
            /// <param name="caseType">Specific case ("individual", "first", "internal", "last", "first_combined_with_next").</param>
            /// <param name="p">Spacing between bolt rows (mm).</param>
            /// <returns>Tuple containing l_eff,cp and l_eff,nc in mm.</returns>
            public static (double l_eff_cp, double l_eff_nc) CalculateEffectiveLengthForComponent3(SteelProfile profile, int row, string caseType, double p)
            {
                double m = profile.m;
                double e = (profile.b_c - profile.w) / 2;

                double l_eff_cp, l_eff_nc;

                switch (row)
                {
                    case 1:
                        if (caseType == "individual")
                        {
                            // Row 1 - Individual effective lengths & as first bolt row of a group
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "first_combined_with_next")
                        {
                            // Row 1 - Effective lengths as first bolt row of a group (combined with Row 2)
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
                            // Row 2 - Individual effective lengths
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "internal")
                        {
                            // Row 2 - Effective lengths as internal bolt row of a group
                            l_eff_cp = 2 * p;
                            l_eff_nc = p;
                        }
                        else if (caseType == "first" || caseType == "last")
                        {
                            // Row 2 - Effective lengths as first/last bolt row of a group
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
                            // Row 3 - Individual effective lengths
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "last")
                        {
                            // Row 3 - Effective lengths as last bolt row of a group
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 3. Use 'individual' or 'last'.");
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid row number. Use 1, 2, or 3.");
                }

                // Output for debugging purposes
                Console.WriteLine($"=== Effective Lengths for Component 3 (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff_cp = {l_eff_cp:F2} mm");
                Console.WriteLine($"l_eff_nc = {l_eff_nc:F2} mm");

                return (l_eff_cp, l_eff_nc);
            }

            /// <summary>
            /// Calculates the design resistance F_Rd,3,(1) for the column web in tension (Component 3) for two rows of bolts.
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <returns>Design resistance F_Rd,3,(1) in Newtons.</returns>
            public static double CalculateComponent3TwoRows(SteelProfile profile)
            {
                if (profile.gamma_m0 == 0)
                    throw new ArgumentException("gamma_m0 cannot be zero.");

                double m_fc = (profile.m - profile.t_wc) / 2 - 0.8 * profile.r_c;
                double e_wc = Math.Min(profile.b_c_w / 2, profile.b_b_w / 2);
                double b_eff_t_wc = Math.Min((4 * profile.m + 1.25 * e_wc), 2 * Math.PI * profile.m);
                double beta = 1;
                double w_t = 0.837;
                double F_Rd_3_1 = (w_t * b_eff_t_wc * profile.fy_wc) / profile.gamma_m0;

                Console.WriteLine("=== Component 3: Column Web in Tension (Two Rows of Bolts) ===");
                Console.WriteLine($"m_fc = {m_fc:F2} mm");
                Console.WriteLine($"e_wc = {e_wc:F2} mm");
                Console.WriteLine($"b_eff_t_wc = {b_eff_t_wc:F2} mm");
                Console.WriteLine($"beta = {beta:F2}");
                Console.WriteLine($"w_t = {w_t:F2}");
                Console.WriteLine($"F_Rd_3_1 = {F_Rd_3_1:F2} N");

                return F_Rd_3_1;
            }

            /// <summary>
            /// Calculates the resistance of an individual bolt row for the column web in tension (Component 3).
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <param name="row">Row number (1, 2, or 3).</param>
            /// <param name="caseType">Specific case ("individual", "first", "internal", "last", "first_combined_with_next").</param>
            /// <param name="n_bolts">Number of bolts in the row.</param>
            /// <param name="p">Spacing between bolt rows (mm).</param>
            /// <returns>Resistance F_t,wc,Rd in Newtons.</returns>
            public static double CalculateIndividualBoltRowResistance(SteelProfile profile, int row, string caseType, int n_bolts, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                // Step 1: Calculate effective lengths l_eff,cp and l_eff,nc
                var (l_eff_cp, l_eff_nc) = CalculateEffectiveLengthForComponent3(profile, row, caseType, p);
                double l_eff = Math.Min(l_eff_cp, l_eff_nc);

                // Step 2: Calculate m_pl,Rd (plastic bending resistance of the T-stub flange)
                double b_eff = l_eff; // Assuming b_eff = l_eff as per the document
                double m_pl_Rd = (0.25 * b_eff * profile.t_f * profile.t_f * profile.f_y) / profile.gamma_m0;

                // Step 3: Calculate bolt resistance (F_t,Rd per bolt)
                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                // Step 4: Calculate resistances for each failure mode
                // Mode 1: Yielding of the flange
                double F_T_Rd_1 = (4 * l_eff * m_pl_Rd) / profile.m;

                // Mode 3: Failure of the bolts
                double F_T_Rd_3 = sum_F_t_Rd;

                // Mode 2: Failure of the bolts further to partial yielding of the flange
                double n = Math.Min((profile.b_c - profile.w) / 2, 1.25 * profile.m);
                double F_T_Rd_2 = (2 * l_eff * m_pl_Rd + n * sum_F_t_Rd) / (profile.m + n);

                // Step 5: Take the minimum resistance
                double F_t_wc_Rd = Math.Min(Math.Min(F_T_Rd_1, F_T_Rd_2), F_T_Rd_3);

                // Output for debugging purposes
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

            /// <summary>
            /// Calculates the resistance of combined bolt rows for the column web in tension (Component 3).
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <param name="rows">Description of the combined rows (e.g., "Rows 1 and 2").</param>
            /// <param name="caseType">Specific case for the combined rows ("first_combined_with_next", "internal", "last").</param>
            /// <param name="n_bolts">Total number of bolts in the combined rows.</param>
            /// <param name="p">Spacing between bolt rows (mm).</param>
            /// <returns>Resistance F_t,wc,Rd in Newtons.</returns>
            public static double CalculateCombinedBoltRowsResistance(SteelProfile profile, string rows, string caseType, int n_bolts, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                // Determine the row number based on the rows description
                int row;
                if (rows.Contains("Rows 1 and 2"))
                    row = 1; // Use Row 1 for "first_combined_with_next"
                else if (rows.Contains("Rows 2 and 3"))
                    row = 2; // Use Row 2 for "last"
                else if (rows.Contains("Rows 1, 2, and 3"))
                    row = 1; // Use Row 1 for "first_combined_with_next"
                else
                    throw new ArgumentException("Invalid rows description. Use 'Rows 1 and 2', 'Rows 2 and 3', or 'Rows 1, 2, and 3'.");

                // Step 1: Calculate effective lengths l_eff,cp and l_eff,nc
                var (l_eff_cp, l_eff_nc) = CalculateEffectiveLengthForComponent3(profile, row, caseType, p);
                double l_eff = Math.Min(l_eff_cp, l_eff_nc);

                // Step 2: Calculate m_pl,Rd
                double b_eff = l_eff;
                double m_pl_Rd = (0.25 * b_eff * profile.t_f * profile.t_f * profile.f_y) / profile.gamma_m0;

                // Step 3: Calculate bolt resistance
                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                // Step 4: Calculate resistances for each failure mode
                double F_T_Rd_1 = (4 * l_eff * m_pl_Rd) / profile.m;
                double F_T_Rd_3 = sum_F_t_Rd;
                double n = Math.Min((profile.b_c - profile.w) / 2, 1.25 * profile.m);
                double F_T_Rd_2 = (2 * l_eff * m_pl_Rd + n * sum_F_t_Rd) / (profile.m + n);

                // Step 5: Take the minimum resistance
                double F_t_wc_Rd = Math.Min(Math.Min(F_T_Rd_1, F_T_Rd_2), F_T_Rd_3);

                // Output for debugging purposes
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
            /// <summary>
            /// Calculates the effective length l_eff for a bolt row based on its position and specific case.
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <param name="row">Row number (1, 2, or 3).</param>
            /// <param name="caseType">Specific case ("individual", "first", "internal", "last", "first_combined_with_next").</param>
            /// <param name="p">Spacing between bolt rows (mm).</param>
            /// <returns>Tuple containing l_eff,cp and l_eff,nc in mm.</returns>
            public static (double l_eff_cp, double l_eff_nc) CalculateEffectiveLengthForComponent4(SteelProfile profile, int row, string caseType, double p)
            {
                double m = profile.m;
                double e = (profile.b_c - profile.w) / 2;

                double l_eff_cp, l_eff_nc;

                switch (row)
                {
                    case 1:
                        if (caseType == "individual")
                        {
                            // Row 1 - Individual effective lengths & as first bolt row of a group
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "first_combined_with_next")
                        {
                            // Row 1 - Effective lengths as first bolt row of a group (combined with Row 2)
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
                            // Row 2 - Individual effective lengths
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "internal")
                        {
                            // Row 2 - Effective lengths as internal bolt row of a group
                            l_eff_cp = 2 * p;
                            l_eff_nc = p;
                        }
                        else if (caseType == "first" || caseType == "last")
                        {
                            // Row 2 - Effective lengths as first/last bolt row of a group
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
                            // Row 3 - Individual effective lengths
                            l_eff_cp = 2 * Math.PI * m;
                            l_eff_nc = 4 * m + 1.25 * e;
                        }
                        else if (caseType == "last")
                        {
                            // Row 3 - Effective lengths as last bolt row of a group
                            l_eff_cp = Math.PI * m + p;
                            l_eff_nc = 2 * m + 0.625 * e + 0.5 * p;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid caseType for Row 3. Use 'individual' or 'last'.");
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid row number. Use 1, 2, or 3.");
                }

                // Output for debugging purposes
                Console.WriteLine($"=== Effective Lengths for Component 4 (Row {row}, Case: {caseType}) ===");
                Console.WriteLine($"l_eff_cp = {l_eff_cp:F2} mm");
                Console.WriteLine($"l_eff_nc = {l_eff_nc:F2} mm");

                return (l_eff_cp, l_eff_nc);
            }

            /// <summary>
            /// Calculates the design resistance F_Rd,4,(1) for the column flange in bending (Component 4) according to Eurocode 03.
            /// </summary>
            /// <param name="profile">Profile object containing geometric and material properties.</param>
            /// <param name="n_bolts">Number of bolts in the row.</param>
            /// <param name="row">Row number (1, 2, or 3).</param>
            /// <param name="caseType">Specific case ("individual", "first", "internal", "last", "first_combined_with_next").</param>
            /// <param name="p">Spacing between bolt rows (mm).</param>
            /// <returns>Design resistance F_Rd,4,(1) in Newtons.</returns>
            public static double CalculateComponent4(SteelProfile profile, int n_bolts, int row, string caseType, double p)
            {
                if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                    throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");

                // Step 1: Calculate effective lengths l_eff,cp and l_eff,nc
                var (l_eff_cp, l_eff_nc) = CalculateEffectiveLengthForComponent4(profile, row, caseType, p);

                // Step 2: Take the minimum of l_eff,cp and l_eff,nc as b_eff,t,fc
                double b_eff_t_fc = Math.Min(l_eff_cp, l_eff_nc);

                // Step 3: Calculate geometrical parameters
                double e = (profile.b_c - profile.w) / 2;
                double e_min = Math.Min(e, profile.w / 2);
                double n = Math.Min(e_min, 1.25 * profile.m);

                // Step 4: Calculate m_pl,Rd (plastic bending resistance)
                double m_pl_Rd = (0.25 * b_eff_t_fc * profile.t_f * profile.t_f * profile.f_y) / profile.gamma_m0;

                // Step 5: Calculate bolt resistance
                double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
                double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

                // Step 6: Calculate resistances for each failure mode
                double F_fc_Rd_1 = (4 * m_pl_Rd) / profile.m;
                double F_fc_Rd_2 = (2 * m_pl_Rd + n * sum_F_t_Rd) / (profile.m + n);

                // Step 7: Take the minimum resistance
                double F_Rd_4_1 = Math.Min(F_fc_Rd_1, F_fc_Rd_2);

                // Output for debugging purposes
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
            using System;

public class Component5Calculator
    {
        // Step 1: Calculate L values (Effective Lengths) for different patterns
        public static (double[] l_eff_1, double[] l_eff_2) CalculateLEffectivePatterns(SteelProfile profile, int row, string caseType, double p)
        {
            double m_x = profile.w / 2 - 0.8 * profile.a_f * Math.Sqrt(2) - profile.e_x;
            double m = profile.w / 2 - profile.t_wb / 2 - 0.8 * profile.a_f * Math.Sqrt(2);
            double e = (profile.b_p - profile.w) / 2;
            double alpha = 4; // Placeholder for alpha, typically provided in tables

            double[] l_eff_1 = new double[3]; // For Mode 1 patterns
            double[] l_eff_2 = new double[3]; // For Mode 2 patterns

            switch (row)
            {
                case 1:
                    if (caseType == "individual")
                    {
                        // Mode 1 patterns
                        l_eff_1[0] = 2 * Math.PI * m_x;                     // Pattern 1: 2πm_x
                        l_eff_1[1] = Math.PI * m_x + profile.w;             // Pattern 2: πm_x + w
                        l_eff_1[2] = Math.PI * m_x + 2 * e;                 // Pattern 3: πm_x + 2e

                        // Mode 2 patterns
                        l_eff_2[0] = 4 * m_x + 1.25 * profile.e_x;          // Pattern 1: 4m_x + 1.25e_x
                        l_eff_2[1] = e + 2 * m_x + 0.625 * profile.e_x;     // Pattern 2: e + 2m_x + 0.625e_x
                        l_eff_2[2] = Math.Min(0.5 * profile.b_p, 0.5 * profile.w + 2 * m_x + 0.625 * profile.e_x); // Pattern 3
                    }
                    else if (caseType == "first_combined_with_next")
                    {
                        l_eff_1[0] = Math.PI * m_x + p;                     // Pattern 1: πm_x + p
                        l_eff_1[1] = double.MaxValue;                        // No other patterns for Mode 1
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = e + 2 * m_x + 0.625 * profile.e_x + 0.5 * p; // Pattern 1: e + 2m_x + 0.625e_x + 0.5p
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid caseType for Row 1. Use 'individual' or 'first_combined_with_next'.");
                    }
                    break;

                case 2:
                    if (caseType == "individual")
                    {
                        l_eff_1[0] = 2 * Math.PI * m;                       // Pattern 1: 2πm
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = alpha * m;                             // Pattern 1: αm
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else if (caseType == "first_combined_with_next")
                    {
                        l_eff_1[0] = Math.PI * m + p;                       // Pattern 1: πm + p
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = 0.5 * p + alpha * m - (2 * m + 0.625 * e); // Pattern 1: 0.5p + αm - (2m + 0.625e)
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else if (caseType == "last_combined_with_prev")
                    {
                        l_eff_1[0] = Math.PI * m + p;                       // Pattern 1: πm + p
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = 2 * m + 0.625 * e + 0.5 * p;           // Pattern 1: 2m + 0.625e + 0.5p
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else if (caseType == "internal")
                    {
                        l_eff_1[0] = 2 * p;                                 // Pattern 1: 2p
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = p;                                     // Pattern 1: p
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid caseType for Row 2. Use 'individual', 'first_combined_with_next', 'last_combined_with_prev', or 'internal'.");
                    }
                    break;

                case 3:
                    if (caseType == "individual")
                    {
                        l_eff_1[0] = 2 * Math.PI * m;                       // Pattern 1: 2πm
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = 4 * m + 1.25 * e;                     // Pattern 1: 4m + 1.25e
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else if (caseType == "last_combined_with_prev")
                    {
                        l_eff_1[0] = Math.PI * m + p;                       // Pattern 1: πm + p
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = 2 * m + 0.625 * e + 0.5 * p;           // Pattern 1: 2m + 0.625e + 0.5p
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else if (caseType == "internal")
                    {
                        l_eff_1[0] = 2 * p;                                 // Pattern 1: 2p
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = p;                                     // Pattern 1: p
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid caseType for Row 3. Use 'individual', 'last_combined_with_prev', or 'internal'.");
                    }
                    break;

                case 4:
                    if (caseType == "individual")
                    {
                        l_eff_1[0] = 2 * Math.PI * m;                       // Pattern 1: 2πm
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = 4 * m + 1.25 * e;                     // Pattern 1: 4m + 1.25e
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else if (caseType == "last_combined_with_prev")
                    {
                        l_eff_1[0] = Math.PI * m + p;                       // Pattern 1: πm + p
                        l_eff_1[1] = double.MaxValue;
                        l_eff_1[2] = double.MaxValue;

                        l_eff_2[0] = 2 * m + 0.625 * e + 0.5 * p;           // Pattern 1: 2m + 0.625e + 0.5p
                        l_eff_2[1] = double.MaxValue;
                        l_eff_2[2] = double.MaxValue;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid caseType for Row 4. Use 'individual' or 'last_combined_with_prev'.");
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid row number. Use 1, 2, 3, or 4.");
            }

            // Print the calculated L values
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

        // Step 2: Calculate resistance for Component 5 considering Modes 1, 2, and 3
        public static double CalculateComponent5(SteelProfile profile, int n_bolts, int row, string caseType, double p)
        {
            if (profile.gamma_m0 == 0 || profile.gamma_m2 == 0)
                throw new ArgumentException("gamma_m0 and gamma_m2 cannot be zero.");
            if (profile.m == 0)
                throw new ArgumentException("m cannot be zero.");

            // Step 1: Get the effective lengths for Mode 1 and Mode 2
            var (l_eff_1, l_eff_2) = CalculateLEffectivePatterns(profile, row, caseType, p);

            // Find the minimum effective lengths for Mode 1 and Mode 2
            double l_eff_1_min = double.MaxValue;
            double l_eff_2_min = double.MaxValue;

            foreach (var l in l_eff_1)
            {
                if (l != double.MaxValue && l < l_eff_1_min)
                    l_eff_1_min = l;
            }

            foreach (var l in l_eff_2)
            {
                if (l != double.MaxValue && l < l_eff_2_min)
                    l_eff_2_min = l;
            }

            double m_x = profile.w / 2 - 0.8 * profile.a_f * Math.Sqrt(2) - profile.e_x;
            double n_x = Math.Min(profile.e_x, 1.25 * m_x);

            // Step 2: Calculate plastic moment resistance per unit length
            double m_pl_Rd = (profile.t_p * profile.t_p * profile.f_yp) / (4 * profile.gamma_m0);

            // Step 3: Calculate bolt resistance in tension
            double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;
            double sum_F_t_Rd = n_bolts * F_t_Rd_per_bolt;

            // Step 4: Calculate resistances for each mode
            // Mode 1: Yielding of the end plate
            double F_T_Rd_1 = (4 * l_eff_1_min * m_pl_Rd) / m_x;

            // Mode 2: Partial yielding of the end plate and bolt failure
            double F_T_Rd_2 = (2 * l_eff_2_min * m_pl_Rd + n_x * sum_F_t_Rd) / (m_x + n_x);

            // Mode 3: Bolt failure
            double F_T_Rd_3 = sum_F_t_Rd;

            // Step 5: Final resistance is the minimum of the three modes
            double F_Rd_5 = Math.Min(Math.Min(F_T_Rd_1, F_T_Rd_2), F_T_Rd_3);

            // Output the results
            Console.WriteLine($"=== Component 5: End Plate in Bending (Row {row}, Case: {caseType}) ===");
            Console.WriteLine($"Minimum l_eff_1 (Mode 1) = {l_eff_1_min:F2} mm");
            Console.WriteLine($"Minimum l_eff_2 (Mode 2) = {l_eff_2_min:F2} mm");
            Console.WriteLine($"m_pl_Rd = {m_pl_Rd:F2} N.mm/mm");
            Console.WriteLine($"F_T_Rd_1 (Mode 1) = {F_T_Rd_1:F2} N");
            Console.WriteLine($"F_T_Rd_2 (Mode 2) = {F_T_Rd_2:F2} N");
            Console.WriteLine($"F_T_Rd_3 (Mode 3) = {F_T_Rd_3:F2} N");
            Console.WriteLine($"F_Rd_5 = {F_Rd_5:F2} N");

            return F_Rd_5;
        }
        using System;

public class Component8Calculator
    {
        // Step 1: Use the effective length (b_eff,t,wb) from Component 3 or 4
        // For simplicity, we assume b_eff,t,wb is provided or calculated elsewhere (e.g., same as l_eff from Component 3)
        public static double CalculateComponent8(SteelProfile profile, int row, double b_eff_t_wb, string caseType)
        {
            if (profile.gamma_m0 == 0)
                throw new ArgumentException("gamma_m0 cannot be zero.");

            // Step 2: Calculate resistance of the beam web in tension (F_t,wb,Rd)
            double F_t_wb_Rd = (b_eff_t_wb * profile.t_wb * profile.f_ywb) / profile.gamma_m0;

            // Output the results
            Console.WriteLine($"=== Component 8: Beam Web in Tension (Row {row}, Case: {caseType}) ===");
            Console.WriteLine($"Effective Length (b_eff,t,wb) = {b_eff_t_wb:F2} mm");
            Console.WriteLine($"Beam Web Thickness (t_wb) = {profile.t_wb:F2} mm");
            Console.WriteLine($"Yield Strength of Beam Web (f_y,wb) = {profile.f_ywb:F2} N/mm²");
            Console.WriteLine($"F_t,wb,Rd = {F_t_wb_Rd:F2} N");

            return F_t_wb_Rd;
        }
        using System;

public class Component10Calculator
    {
        // Calculate resistance of bolts in tension (F_t,Rd) for a given row
        public static double CalculateComponent10(SteelProfile profile, int n_bolts, int row, string caseType)
        {
            if (profile.gamma_m2 == 0)
                throw new ArgumentException("gamma_m2 cannot be zero.");

            // Step 1: Calculate the tensile resistance of a single bolt (F_t,Rd)
            double F_t_Rd_per_bolt = (0.9 * profile.A_s * profile.f_ub) / profile.gamma_m2;

            // Step 2: Multiply by the number of bolts in the row
            double F_t_Rd = n_bolts * F_t_Rd_per_bolt;

            // Output the results
            Console.WriteLine($"=== Component 10: Bolts in Tension (Row {row}, Case: {caseType}) ===");
            Console.WriteLine($"Tensile Stress Area of Bolt (A_s) = {profile.A_s:F2} mm²");
            Console.WriteLine($"Ultimate Tensile Strength of Bolt (f_ub) = {profile.f_ub:F2} N/mm²");
            Console.WriteLine($"Number of Bolts in Row = {n_bolts}");
            Console.WriteLine($"F_t,Rd per Bolt = {F_t_Rd_per_bolt:F2} N");
            Console.WriteLine($"Total F_t,Rd for Row = {F_t_Rd:F2} N");

            return F_t_Rd;
        }
        using System;

public class Component1Calculator
    {
        // Calculate resistance of the column web panel in shear (F_Rd,1)
        public static double CalculateComponent1(SteelProfile profile)
        {
            if (profile.gamma_m0 == 0)
                throw new ArgumentException("gamma_m0 cannot be zero.");

            // Step 1: Calculate the shear resistance of the column web panel (F_Rd,1)
            double F_Rd_1 = (0.9 * profile.A_vc * profile.f_ywc) / (Math.Sqrt(3) * profile.gamma_m0);

            // Output the results
            Console.WriteLine("=== Component 1: Column Web Panel in Shear ===");
            Console.WriteLine($"Shear Area of Column Web (A_vc) = {profile.A_vc:F2} mm²");
            Console.WriteLine($"Yield Strength of Column Web (f_y,wc) = {profile.f_ywc:F2} N/mm²");
            Console.WriteLine($"Partial Safety Factor (γ_M0) = {profile.gamma_m0:F2}");
            Console.WriteLine($"F_Rd,1 = {F_Rd_1:F2} N");

            return F_Rd_1;
        }
    

    // Supporting class for SteelProfile (assumed to be defined elsewhere)
    public class SteelProfile
    {
        public double A_vc { get; set; }     // Shear area of the column web (mm²)
        public double f_ywc { get; set; }    // Yield strength of the column web (N/mm²)
        public double gamma_m0 { get; set; } // Partial safety factor
    }
}

    // Supporting class for SteelProfile (assumed to be defined elsewhere)
    public class SteelProfile
    {
        public double A_s { get; set; }      // Tensile stress area of the bolt (mm²)
        public double f_ub { get; set; }     // Ultimate tensile strength of the bolt (N/mm²)
        public double gamma_m2 { get; set; } // Partial safety factor
    }
    // Supporting class for SteelProfile (assumed to be defined elsewhere)
    public class SteelProfile
    {
        public double t_wb { get; set; }      // Beam web thickness (mm)
        public double f_ywb { get; set; }     // Yield strength of beam web (N/mm²)
        public double gamma_m0 { get; set; }  // Partial safety factor
    }

    // Supporting class for SteelProfile (assumed to be defined elsewhere)
    public class SteelProfile
    {
        public double w { get; set; }
        public double a_f { get; set; }
        public double e_x { get; set; }
        public double t_wb { get; set; }
        public double b_p { get; set; }
        public double t_p { get; set; }
        public double f_yp { get; set; }
        public double A_s { get; set; }
        public double f_ub { get; set; }
        public double gamma_m0 { get; set; }
        public double gamma_m2 { get; set; }
        public double m { get; set; }

public class ComponentAssembler
    {
        public static (double F_1, double F_2, double F_3, double F_4) AssembleComponents(SteelProfile profile, double p)
        {
            // Step 1: Calculate global resistances (Component 1, 2, 7)
            double F_Rd_1 = Component1Calculator.CalculateComponent1(profile); // Column Web in Shear
            double F_Rd_2 = Component2Calculator.CalculateComponent2(profile); // Column Web in Compression
            double F_Rd_7 = Component7Calculator.CalculateComponent7(profile); // Beam Flange and Web in Compression

            // Step 2: Calculate individual bolt row resistances
            // Row 1
            double F_t_wc_Rd_row1 = Component3Calculator.CalculateIndividualBoltRowResistance(profile, 1, "individual", 2, p); // Component 3
            double F_t_fc_Rd_row1 = Component4Calculator.CalculateComponent4(profile, 2, 1, "individual", p); // Component 4
            double F_t_ep_Rd_row1 = Component5Calculator.CalculateComponent5(profile, 2, 1, "individual", p); // Component 5
            double F_t_Rd_row1 = Component10Calculator.CalculateComponent10(profile, 2, 1, "individual"); // Component 10
            double F_1 = Math.Min(Math.Min(F_t_wc_Rd_row1, F_t_fc_Rd_row1), Math.Min(F_t_ep_Rd_row1, F_t_Rd_row1));

            // Row 2
            double F_t_wc_Rd_row2 = Component3Calculator.CalculateIndividualBoltRowResistance(profile, 2, "individual", 2, p);
            double F_t_fc_Rd_row2 = Component4Calculator.CalculateComponent4(profile, 2, 2, "individual", p);
            double F_t_ep_Rd_row2 = Component5Calculator.CalculateComponent5(profile, 2, 2, "individual", p);
            double b_eff_t_wb_row2 = F_t_wc_Rd_row2; // Assuming b_eff,t,wb is the same as l_eff from Component 3
            double F_t_wb_Rd_row2 = Component8Calculator.CalculateComponent8(profile, 2, b_eff_t_wb_row2, "individual");
            double F_t_Rd_row2 = Component10Calculator.CalculateComponent10(profile, 2, 2, "individual");
            double F_2 = Math.Min(Math.Min(F_t_wc_Rd_row2, F_t_fc_Rd_row2), Math.Min(F_t_ep_Rd_row2, Math.Min(F_t_wb_Rd_row2, F_t_Rd_row2)));

            // Row 3
            double F_t_wc_Rd_row3 = Component3Calculator.CalculateIndividualBoltRowResistance(profile, 3, "individual", 2, p);
            double F_t_fc_Rd_row3 = Component4Calculator.CalculateComponent4(profile, 2, 3, "individual", p);
            double F_t_ep_Rd_row3 = Component5Calculator.CalculateComponent5(profile, 2, 3, "individual", p);
            double b_eff_t_wb_row3 = F_t_wc_Rd_row3; // Assuming b_eff,t,wb is the same as l_eff from Component 3
            double F_t_wb_Rd_row3 = Component8Calculator.CalculateComponent8(profile, 3, b_eff_t_wb_row3, "individual");
            double F_t_Rd_row3 = Component10Calculator.CalculateComponent10(profile, 2, 3, "individual");
            double F_3 = Math.Min(Math.Min(F_t_wc_Rd_row3, F_t_fc_Rd_row3), Math.Min(F_t_ep_Rd_row3, Math.Min(F_t_wb_Rd_row3, F_t_Rd_row3)));

            // Row 4
            double F_t_wc_Rd_row4 = Component3Calculator.CalculateIndividualBoltRowResistance(profile, 4, "individual", 2, p);
            double F_t_fc_Rd_row4 = Component4Calculator.CalculateComponent4(profile, 2, 4, "individual", p);
            double F_t_ep_Rd_row4 = Component5Calculator.CalculateComponent5(profile, 2, 4, "individual", p);
            double b_eff_t_wb_row4 = F_t_wc_Rd_row4; // Assuming b_eff,t,wb is the same as l_eff from Component 3
            double F_t_wb_Rd_row4 = Component8Calculator.CalculateComponent8(profile, 4, b_eff_t_wb_row4, "individual");
            double F_t_Rd_row4 = Component10Calculator.CalculateComponent10(profile, 2, 4, "individual");
            double F_4 = Math.Min(Math.Min(F_t_wc_Rd_row4, F_t_fc_Rd_row4), Math.Min(F_t_ep_Rd_row4, Math.Min(F_t_wb_Rd_row4, F_t_Rd_row4)));

            // Step 3: Group Effects
            // Rows 1-2
            double F_t_wc_Rd_rows1_2 = Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 1 and 2", "first_combined_with_next", 4, p);
            double F_t_fc_Rd_rows1_2 = Component4Calculator.CalculateComponent4(profile, 4, 1, "first_combined_with_next", p);
            double F_1_2 = Math.Min(F_t_wc_Rd_rows1_2, F_t_fc_Rd_rows1_2);
            double sum_F_1_2 = F_1 + F_2;
            double F_2_reduced = sum_F_1_2 > F_1_2 ? F_1_2 - F_1 : F_2;

            // Rows 1-2-3
            double F_t_wc_Rd_rows1_2_3 = Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 1, 2, and 3", "first_combined_with_next", 6, p);
            double F_t_fc_Rd_rows1_2_3 = Component4Calculator.CalculateComponent4(profile, 6, 1, "first_combined_with_next", p);
            double F_1_2_3 = Math.Min(F_t_wc_Rd_rows1_2_3, F_t_fc_Rd_rows1_2_3);
            double sum_F_1_2_3 = F_1 + F_2 + F_3;
            double F_3_reduced = sum_F_1_2_3 > F_1_2_3 ? F_1_2_3 - F_1 - F_2 : F_3;

            // Rows 2-3
            double F_t_wc_Rd_rows2_3 = Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 2 and 3", "first_combined_with_next", 4, p);
            double F_t_fc_Rd_rows2_3 = Component4Calculator.CalculateComponent4(profile, 4, 2, "first_combined_with_next", p);
            double F_2_3 = Math.Min(F_t_wc_Rd_rows2_3, F_t_fc_Rd_rows2_3);
            double sum_F_2_3 = F_2 + F_3;
            double F_3_reduced_2_3 = sum_F_2_3 > F_2_3 ? F_2_3 - F_2 : F_3;

            // Rows 1-2-3-4
            double F_t_wc_Rd_rows1_2_3_4 = Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 1, 2, 3, and 4", "first_combined_with_next", 8, p);
            double F_t_fc_Rd_rows1_2_3_4 = Component4Calculator.CalculateComponent4(profile, 8, 1, "first_combined_with_next", p);
            double F_1_2_3_4 = Math.Min(F_t_wc_Rd_rows1_2_3_4, F_t_fc_Rd_rows1_2_3_4);
            double sum_F_1_2_3_4 = F_1 + F_2 + F_3 + F_4;
            double F_4_reduced = sum_F_1_2_3_4 > F_1_2_3_4 ? F_1_2_3_4 - F_1 - F_2 - F_3 : F_4;

            // Use the most restrictive F_3 from group effects
            F_3_reduced = Math.Min(F_3_reduced, F_3_reduced_2_3);

            // Step 4: Compare with global resistances
            double F_glob_min = Math.Min(F_Rd_1, Math.Min(F_Rd_2, F_Rd_7));
            double sum_F_tension = F_1 + F_2 + F_3_reduced + F_4_reduced;

            // Step 5: Adjust forces if necessary
            if (sum_F_tension > F_glob_min)
            {
                // Reduce forces starting from the lowest row (Row 4, then Row 3, etc.)
                if (F_4_reduced > 0)
                {
                    F_4_reduced = 0; // Set Row 4 to zero
                    sum_F_tension = F_1 + F_2 + F_3_reduced;
                }

                if (sum_F_tension > F_glob_min && F_3_reduced > 0)
                {
                    F_3_reduced = F_glob_min - F_1 - F_2;
                    if (F_3_reduced < 0) F_3_reduced = 0;
                    sum_F_tension = F_1 + F_2 + F_3_reduced;
                }

                if (sum_F_tension > F_glob_min && F_2 > 0)
                {
                    F_2 = F_glob_min - F_1;
                    if (F_2 < 0) F_2 = 0;
                    F_3_reduced = 0;
                    sum_F_tension = F_1 + F_2;
                }

                if (sum_F_tension > F_glob_min && F_1 > 0)
                {
                    F_1 = F_glob_min;
                    F_2 = 0;
                    F_3_reduced = 0;
                    sum_F_tension = F_1;
                }
            }

            // Output the results
            Console.WriteLine("=== Assembling of Components ===");
            Console.WriteLine($"Individual Row Forces:");
            Console.WriteLine($"F_1 (Row 1) = {F_1:F2} N");
            Console.WriteLine($"F_2 (Row 2) = {F_2:F2} N");
            Console.WriteLine($"F_3 (Row 3) = {F_3:F2} N");
            Console.WriteLine($"F_4 (Row 4) = {F_4:F2} N");
            Console.WriteLine($"Group Effects:");
            Console.WriteLine($"Rows 1-2 Resistance = {F_1_2:F2} N (Sum Individual = {sum_F_1_2:F2} N)");
            Console.WriteLine($"Rows 1-2-3 Resistance = {F_1_2_3:F2} N (Sum Individual = {sum_F_1_2_3:F2} N)");
            Console.WriteLine($"Rows 2-3 Resistance = {F_2_3:F2} N (Sum Individual = {sum_F_2_3:F2} N)");
            Console.WriteLine($"Rows 1-2-3-4 Resistance = {F_1_2_3_4:F2} N (Sum Individual = {sum_F_1_2_3_4:F2} N)");
            Console.WriteLine($"Reduced F_3 = {F_3_reduced:F2} N");
            Console.WriteLine($"Reduced F_4 = {F_4_reduced:F2} N");
            Console.WriteLine($"Global Resistance (min of F_Rd_1, F_Rd_2, F_Rd_7) = {F_glob_min:F2} N");
            Console.WriteLine($"Final Sum of Tension Forces = {sum_F_tension:F2} N");
            Console.WriteLine($"Final Row Forces:");
            Console.WriteLine($"F_1 = {F_1:F2} N");
            Console.WriteLine($"F_2 = {F_2:F2} N");
            Console.WriteLine($"F_3 = {F_3_reduced:F2} N");
            Console.WriteLine($"F_4 = {F_4_reduced:F2} N");

            return (F_1, F_2, F_3_reduced, F_4_reduced);
        }
    }

    // Placeholder classes for other components (assumed to be defined elsewhere)
    public class Component1Calculator
    {
        public static double CalculateComponent1(SteelProfile profile)
        {
            throw new NotImplementedException("Component 1 Calculator should be implemented.");
        }
    }

    public class Component2Calculator
    {
        public static double CalculateComponent2(SteelProfile profile)
        {
            throw new NotImplementedException("Component 2 Calculator should be implemented.");
        }
    }

    public class Component3Calculator
    {
        public static double CalculateIndividualBoltRowResistance(SteelProfile profile, int row, string caseType, int n_bolts, double p)
        {
            throw new NotImplementedException("Component 3 Calculator should be implemented.");
        }

        public static double CalculateCombinedBoltRowsResistance(SteelProfile profile, string rowGroup, string caseType, int n_bolts, double p)
        {
            throw new NotImplementedException("Component 3 Calculator for combined rows should be implemented.");
        }
    }

    public class Component4Calculator
    {
        public static double CalculateComponent4(SteelProfile profile, int n_bolts, int row, string caseType, double p)
        {
            throw new NotImplementedException("Component 4 Calculator should be implemented.");
        }
    }

    public class Component5Calculator
    {
        public static double CalculateComponent5(SteelProfile profile, int n_bolts, int row, string caseType, double p)
        {
            throw new NotImplementedException("Component 5 Calculator should be implemented.");
        }
    }

    public class Component7Calculator
    {
        public static double CalculateComponent7(SteelProfile profile)
        {
            throw new NotImplementedException("Component 7 Calculator should be implemented.");
        }
    }

    public class Component8Calculator
    {
        public static double CalculateComponent8(SteelProfile profile, int row, double b_eff_t_wb, string caseType)
        {
            throw new NotImplementedException("Component 8 Calculator should be implemented.");
        }
    }

    public class Component10Calculator
    {
        public static double CalculateComponent10(SteelProfile profile, int n_bolts, int row, string caseType)
        {
            throw new NotImplementedException("Component 10 Calculator should be implemented.");
        }
    }

    public class SteelProfile
    {
        // Properties are assumed to be defined in another class
    }


public class RotationalStiffnessCalculator
    {
        public static double CalculateRotationalStiffness(SteelProfile profile, double[] h, double[] b_eff_t_wc, double[] l_eff_fc, double[] l_eff_ep, double beta)
        {
            // Validate inputs
            if (h.Length == 0 || h.Length != b_eff_t_wc.Length || h.Length != l_eff_fc.Length || h.Length != l_eff_ep.Length)
                throw new ArgumentException("Input arrays (h, b_eff_t_wc, l_eff_fc, l_eff_ep) must have the same non-zero length.");
            if (beta <= 0)
                throw new ArgumentException("beta must be greater than zero.");

            // Step 1: Calculate stiffness coefficients (k) for each component except k_1 (since it depends on z_eq)
            // k_2: Column Web in Compression
            if (profile.d_wc == 0)
                throw new ArgumentException("d_wc cannot be zero.");
            double k_2 = (0.7 * profile.b_eff_c_wc * profile.t_wc) / profile.d_wc;

            // k_7: Beam Flange and Web in Compression (assumed infinite as per Eurocode 3 for rotational stiffness)
            double k_7 = double.PositiveInfinity;

            // Calculate k_3, k_4, k_5, k_10 for each row and calculate k_eff
            double[] k_eff = new double[h.Length];
            double sum_k_eff_h = 0;
            double sum_k_eff_h2 = 0;

            for (int i = 0; i < h.Length; i++)
            {
                // k_3: Column Web in Tension
                double k_3 = (0.7 * b_eff_t_wc[i] * profile.t_wc) / profile.d_wc;
                if (k_3 <= 0)
                    throw new ArgumentException($"k_3 for Row {i + 1} must be greater than zero.");

                // k_4: Column Flange in Bending
                if (profile.m == 0)
                    throw new ArgumentException("m cannot be zero.");
                double k_4 = (0.9 * l_eff_fc[i] * Math.Pow(profile.t_fc, 3)) / Math.Pow(profile.m, 3);
                if (k_4 <= 0)
                    throw new ArgumentException($"k_4 for Row {i + 1} must be greater than zero.");

                // k_5: End Plate in Bending
                double k_5 = (0.9 * l_eff_ep[i] * Math.Pow(profile.t_p, 3)) / Math.Pow(profile.m, 3);
                if (k_5 <= 0)
                    throw new ArgumentException($"k_5 for Row {i + 1} must be greater than zero.");

                // k_10: Bolts in Tension
                if (profile.L_b == 0)
                    throw new ArgumentException("L_b cannot be zero.");
                double k_10 = (1.6 * profile.A_s) / profile.L_b;
                if (k_10 <= 0)
                    throw new ArgumentException($"k_10 for Row {i + 1} must be greater than zero.");

                // k_eff for the row
                k_eff[i] = 1 / (1 / k_3 + 1 / k_4 + 1 / k_5 + 1 / k_10);
                sum_k_eff_h += k_eff[i] * h[i];
                sum_k_eff_h2 += k_eff[i] * h[i] * h[i];
            }

            // Step 2: Calculate z_eq (Equivalent Lever Arm)
            if (sum_k_eff_h == 0)
                throw new ArgumentException("Sum of k_eff * h cannot be zero.");
            double z_eq = sum_k_eff_h2 / sum_k_eff_h;

            // Step 3: Calculate k_1 (Column Web in Shear) now that z_eq is available
            if (z_eq == 0)
                throw new ArgumentException("z_eq cannot be zero.");
            double k_1 = (0.38 * profile.A_vc) / (beta * z_eq);
            if (k_1 <= 0)
                throw new ArgumentException("k_1 must be greater than zero.");

            // Step 4: Calculate k_eq
            double k_eq = sum_k_eff_h / z_eq;

            // Step 5: Calculate initial rotational stiffness (S_j,ini)
            double denominator = (1 / k_1) + (1 / k_2) + (1 / k_7) + (1 / k_eq);
            if (denominator == 0)
                throw new ArgumentException("Denominator for S_j,ini calculation cannot be zero.");
            double S_j_ini = (profile.E * z_eq * z_eq) / denominator;

            // Convert S_j,ini to kN·m/rad (since E is in N/mm² and z_eq in mm)
            S_j_ini = S_j_ini / 1000000; // Convert from N·mm/rad to kN·m/rad

            // Output the results
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
    

    // Supporting class for SteelProfile (assumed to be defined elsewhere)
    public class SteelProfile
    {
        public double A_vc { get; set; }      // Shear area of the column web (mm²)
        public double b_eff_c_wc { get; set; } // Effective length for column web in compression (mm)
        public double t_wc { get; set; }      // Column web thickness (mm)
        public double d_wc { get; set; }      // Depth of the column web (mm)
        public double t_fc { get; set; }      // Column flange thickness (mm)
        public double t_p { get; set; }       // End plate thickness (mm)
        public double m { get; set; }         // Effective distance (mm)
        public double A_s { get; set; }       // Tensile stress area of the bolt (mm²)
        public double L_b { get; set; }       // Bolt length (mm)
        public double E { get; set; }         // Modulus of elasticity (N/mm²)
    }


public class JointClassifier
    {
        public static (string StiffnessClassification, string StrengthClassification) ClassifyJoint(
            SteelProfile profile, double S_j_ini, double M_j_Rd)
        {
            // Step 1: Validate Inputs
            if (S_j_ini < 0)
                throw new ArgumentException("S_j,ini must be non-negative.");
            if (M_j_Rd < 0)
                throw new ArgumentException("M_j,Rd must be non-negative.");
            if (profile.L_b <= 0)
                throw new ArgumentException("L_b must be greater than zero.");
            if (profile.I_b <= 0)
                throw new ArgumentException("I_b must be greater than zero.");
            if (profile.W_pl <= 0)
                throw new ArgumentException("W_pl must be greater than zero.");
            if (profile.gamma_m0 <= 0)
                throw new ArgumentException("gamma_m0 must be greater than zero.");

            // Step 2: Stiffness Classification
            // Calculate EI_b / L_b
            double EI_b_over_L_b = (profile.E * profile.I_b) / profile.L_b;

            // Define boundaries for stiffness classification
            double pinnedLimit = 0.5 * EI_b_over_L_b;
            double rigidLimitUnbraced = 8 * EI_b_over_L_b;  // k_b = 8 for unbraced frames
            double rigidLimitBraced = 25 * EI_b_over_L_b;   // k_b = 25 for braced frames

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

            // Step 3: Strength Classification
            // Calculate M_b,pl,Rd (Plastic moment resistance of the beam)
            double M_b_pl_Rd = (profile.W_pl * profile.f_y) / profile.gamma_m0;

            string strengthClassification;
            if (M_j_Rd >= M_b_pl_Rd)
            {
                strengthClassification = "Full-Strength";
            }
            else
            {
                strengthClassification = "Partial-Strength";
            }

            // Output the results
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

    // Supporting class for SteelProfile (assumed to be defined elsewhere)
    public class SteelProfile
    {
        public double E { get; set; }         // Modulus of elasticity (N/mm²)
        public double I_b { get; set; }       // Moment of inertia of the beam (mm⁴)
        public double L_b { get; set; }       // Span of the beam (mm)
        public double W_pl { get; set; }      // Plastic section modulus of the beam (mm³)
        public double f_y { get; set; }       // Yield strength of the beam (N/mm²)
        public double gamma_m0 { get; set; }  // Partial safety factor
    }
}





  
    

    