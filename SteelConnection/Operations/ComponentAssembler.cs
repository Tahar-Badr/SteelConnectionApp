namespace SteelConnection
{
    public class ComponentAssembler
    {
        public static (double F_1, double F_2, double F_3, double F_4) AssembleComponents(SteelProfiles profile, double p)
        {
            // Step 1: Calculate global resistances
            double F_Rd_1 = Calculations.Component1Calculator.CalculateComponent1(profile);
            double F_Rd_2 = Calculations.Component2Calculator.CalculateComponent2(profile);
            double F_Rd_7 = Calculations.Component7Calculator.CalculateComponent7(profile);

            // Step 2: Calculate effective lengths for Component 3
            var (l_eff_row2_ind_cp, l_eff_row2_ind_nc) = Calculations.Component3Calculator.CalculateEffectiveLengthForComponent3(profile, 2, "individual", p);
            double l_eff_row2_ind = Math.Min(l_eff_row2_ind_cp, l_eff_row2_ind_nc);
            var (l_eff_row3_ind_cp, l_eff_row3_ind_nc) = Calculations.Component3Calculator.CalculateEffectiveLengthForComponent3(profile, 3, "individual", p);
            double l_eff_row3_ind = Math.Min(l_eff_row3_ind_cp, l_eff_row3_ind_nc);
            var (l_eff_row4_ind_cp, l_eff_row4_ind_nc) = Calculations.Component3Calculator.CalculateEffectiveLengthForComponent3(profile, 4, "individual", p);
            double l_eff_row4_ind = Math.Min(l_eff_row4_ind_cp, l_eff_row4_ind_nc);

            // Step 3: Calculate individual bolt row resistances
            // Row 1
            double F_t_wc_Rd_row1 = Calculations.Component3Calculator.CalculateIndividualBoltRowResistance(profile, 1, "individual", 2, p);
            double F_t_fc_Rd_row1 = Calculations.Component4Calculator.CalculateComponent4(profile, 2, 1, "individual", p);
            double F_t_ep_Rd_row1 = Calculations.Component5Calculator.CalculateComponent5(profile, 2, 1, "individual", p);
            double F_t_Rd_row1 = Calculations.Component10Calculator.CalculateComponent10(profile, 2, 1, "individual");
            double F_1 = Math.Min(Math.Min(F_t_wc_Rd_row1, F_t_fc_Rd_row1), Math.Min(F_t_ep_Rd_row1, F_t_Rd_row1));

            // Row 2
            double F_t_wc_Rd_row2 = Calculations.Component3Calculator.CalculateIndividualBoltRowResistance(profile, 2, "individual", 2, p);
            double F_t_fc_Rd_row2 = Calculations.Component4Calculator.CalculateComponent4(profile, 2, 2, "individual", p);
            double F_t_ep_Rd_row2 = Calculations.Component5Calculator.CalculateComponent5(profile, 2, 2, "individual", p);
            double F_t_wb_Rd_row2 = Calculations.Component8Calculator.CalculateComponent8(profile, 2, l_eff_row2_ind, "individual");
            double F_t_Rd_row2 = Calculations.Component10Calculator.CalculateComponent10(profile, 2, 2, "individual");
            double F_2 = Math.Min(Math.Min(F_t_wc_Rd_row2, F_t_fc_Rd_row2), Math.Min(F_t_ep_Rd_row2, Math.Min(F_t_wb_Rd_row2, F_t_Rd_row2)));

            // Row 3
            double F_t_wc_Rd_row3 = Calculations.Component3Calculator.CalculateIndividualBoltRowResistance(profile, 3, "individual", 2, p);
            double F_t_fc_Rd_row3 = Calculations.Component4Calculator.CalculateComponent4(profile, 2, 3, "individual", p);
            double F_t_ep_Rd_row3 = Calculations.Component5Calculator.CalculateComponent5(profile, 2, 3, "individual", p);
            double F_t_wb_Rd_row3 = Calculations.Component8Calculator.CalculateComponent8(profile, 3, l_eff_row3_ind, "individual");
            double F_t_Rd_row3 = Calculations.Component10Calculator.CalculateComponent10(profile, 2, 3, "individual");
            double F_3 = Math.Min(Math.Min(F_t_wc_Rd_row3, F_t_fc_Rd_row3), Math.Min(F_t_ep_Rd_row3, Math.Min(F_t_wb_Rd_row3, F_t_Rd_row3)));

            // Row 4
            double F_t_wc_Rd_row4 = Calculations.Component3Calculator.CalculateIndividualBoltRowResistance(profile, 4, "individual", 2, p);
            double F_t_fc_Rd_row4 = Calculations.Component4Calculator.CalculateComponent4(profile, 2, 4, "individual", p);
            double F_t_ep_Rd_row4 = Calculations.Component5Calculator.CalculateComponent5(profile, 2, 4, "individual", p);
            double F_t_wb_Rd_row4 = Calculations.Component8Calculator.CalculateComponent8(profile, 4, l_eff_row4_ind, "individual");
            double F_t_Rd_row4 = Calculations.Component10Calculator.CalculateComponent10(profile, 2, 4, "individual");
            double F_4 = Math.Min(Math.Min(F_t_wc_Rd_row4, F_t_fc_Rd_row4), Math.Min(F_t_ep_Rd_row4, Math.Min(F_t_wb_Rd_row4, F_t_Rd_row4)));

            // Step 4: Group Effects
            // Rows 1-2
            double F_t_wc_Rd_rows1_2 = Calculations.Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 1 and 2", "first_combined_with_next", 4, p);
            double F_t_fc_Rd_rows1_2 = Calculations.Component4Calculator.CalculateComponent4(profile, 4, 1, "first_combined_with_next", p);
            double F_1_2 = Math.Min(F_t_wc_Rd_rows1_2, F_t_fc_Rd_rows1_2);
            double sum_F_1_2 = F_1 + F_2;
            double F_2_reduced = sum_F_1_2 > F_1_2 ? F_1_2 - F_1 : F_2;

            // Rows 1-2-3
            double F_t_wc_Rd_rows1_2_3 = Calculations.Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 1, 2, and 3", "first_combined_with_next", 6, p);
            double F_t_fc_Rd_rows1_2_3 = Calculations.Component4Calculator.CalculateComponent4(profile, 6, 1, "first_combined_with_next", p);
            double F_1_2_3 = Math.Min(F_t_wc_Rd_rows1_2_3, F_t_fc_Rd_rows1_2_3);
            double sum_F_1_2_3 = F_1 + F_2_reduced + F_3;
            double F_3_reduced = sum_F_1_2_3 > F_1_2_3 ? F_1_2_3 - F_1 - F_2_reduced : F_3;

            // Rows 2-3
            double F_t_wc_Rd_rows2_3 = Calculations.Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 2 and 3", "first_combined_with_next", 4, p);
            double F_t_fc_Rd_rows2_3 = Calculations.Component4Calculator.CalculateComponent4(profile, 4, 2, "first_combined_with_next", p);
            double F_2_3 = Math.Min(F_t_wc_Rd_rows2_3, F_t_fc_Rd_rows2_3);
            double sum_F_2_3 = F_2_reduced + F_3;
            double F_3_reduced_2_3 = sum_F_2_3 > F_2_3 ? F_2_3 - F_2_reduced : F_3;

            // Rows 1-2-3-4
            double F_t_wc_Rd_rows1_2_3_4 = Calculations.Component3Calculator.CalculateCombinedBoltRowsResistance(profile, "Rows 1, 2, 3, and 4", "first_combined_with_next", 8, p);
            double F_t_fc_Rd_rows1_2_3_4 = Calculations.Component4Calculator.CalculateComponent4(profile, 8, 1, "first_combined_with_next", p);
            double F_1_2_3_4 = Math.Min(F_t_wc_Rd_rows1_2_3_4, F_t_fc_Rd_rows1_2_3_4);
            double sum_F_1_2_3_4 = F_1 + F_2_reduced + F_3 + F_4;
            double F_4_reduced = sum_F_1_2_3_4 > F_1_2_3_4 ? F_1_2_3_4 - F_1 - F_2_reduced - F_3 : F_4;

            // Use the most restrictive F_3
            F_3_reduced = Math.Min(F_3_reduced, F_3_reduced_2_3);

            // Step 5: Compare with global resistances
            double F_glob_min = Math.Min(F_Rd_1, Math.Min(F_Rd_2, F_Rd_7));
            double sum_F_tension = F_1 + F_2_reduced + F_3_reduced + F_4_reduced;

            // Step 6: Adjust forces if necessary
            if (sum_F_tension > F_glob_min)
            {
                if (F_4_reduced > 0)
                {
                    F_4_reduced = 0;
                    sum_F_tension = F_1 + F_2_reduced + F_3_reduced;
                }

                if (sum_F_tension > F_glob_min && F_3_reduced > 0)
                {
                    F_3_reduced = F_glob_min - F_1 - F_2_reduced;
                    if (F_3_reduced < 0) F_3_reduced = 0;
                    sum_F_tension = F_1 + F_2_reduced + F_3_reduced;
                }

                if (sum_F_tension > F_glob_min && F_2_reduced > 0)
                {
                    F_2_reduced = F_glob_min - F_1;
                    if (F_2_reduced < 0) F_2_reduced = 0;
                    F_3_reduced = 0;
                    sum_F_tension = F_1 + F_2_reduced;
                }

                if (sum_F_tension > F_glob_min && F_1 > 0)
                {
                    F_1 = F_glob_min;
                    F_2_reduced = 0;
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
            Console.WriteLine($"Reduced F_2 = {F_2_reduced:F2} N");
            Console.WriteLine($"Reduced F_3 = {F_3_reduced:F2} N");
            Console.WriteLine($"Reduced F_4 = {F_4_reduced:F2} N");
            Console.WriteLine($"Global Resistance (min of F_Rd_1, F_Rd_2, F_Rd_7) = {F_glob_min:F2} N");
            Console.WriteLine($"Final Sum of Tension Forces = {sum_F_tension:F2} N");
            Console.WriteLine($"Final Row Forces:");
            Console.WriteLine($"F_1 = {F_1:F2} N");
            Console.WriteLine($"F_2 = {F_2_reduced:F2} N");
            Console.WriteLine($"F_3 = {F_3_reduced:F2} N");
            Console.WriteLine($"F_4 = {F_4_reduced:F2} N");

            return (F_1, F_2_reduced, F_3_reduced, F_4_reduced);
        }
    }
}