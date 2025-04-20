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
        }
    }

    