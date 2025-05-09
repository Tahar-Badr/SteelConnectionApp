namespace SteelConnection
{
    // Unified SteelProfile class with all required properties
    public class SteelProfiles
    {
        // Beam properties
        public double t_jb { get; set; }      // Beam flange thickness (mm)
        public double q_f { get; set; }       // Distance related to beam flange (mm)
        public double h_b { get; set; }       // Beam height (mm)
        public double b_b_w { get; set; }     // Beam web width (mm)
        public double t_wb { get; set; }      // Beam web thickness (mm)
        public double f_yb { get; set; }      // Yield strength of the beam (N/mm²)
        public double f_ywb { get; set; }     // Yield strength of beam web (N/mm²)
        public double W_pl_y { get; set; }    // Plastic section modulus about y-axis (mm³)
        public double I_b { get; set; }       // Moment of inertia of the beam (mm⁴)
        public double L_b { get; set; }       // Span of the beam (mm)

        // Column properties
        public double t_fc { get; set; }      // Column flange thickness (mm)
        public double r_c { get; set; }       // Column root radius (mm)
        public double s_p { get; set; }       // Distance related to stiffeners or spacing (mm)
        public double b_eff_avg { get; set; } // Average effective width (mm)
        public double d_wc { get; set; }      // Column web depth (mm)
        public double fy_wc { get; set; }     // Yield strength of the column web (N/mm²)
        public double t_wc { get; set; }      // Column web thickness (mm)
        public double b_c_w { get; set; }     // Column web width (mm)
        public double b_c { get; set; }       // Column width (mm)
        public double A_vc { get; set; }      // Shear area of the column web (mm²)
        public double f_ywc { get; set; }     // Yield strength of the column web for shear (N/mm²)

        // End plate properties
        public double t_p { get; set; }       // End plate thickness (mm)
        public double f_yp { get; set; }      // Yield strength of the end plate (N/mm²)
        public double b_p { get; set; }       // Width of the end plate (mm)

        // Bolt properties
        public double A_s { get; set; }       // Tensile stress area of the bolt (mm²)
        public double f_ub { get; set; }      // Ultimate tensile strength of the bolt (N/mm²)
        public double L_b_bolt { get; set; }  // Bolt length (mm)

        // General properties
        public double E { get; set; }         // Modulus of elasticity (N/mm²)
        public double f_y { get; set; }       // Yield strength (N/mm²)
        public double gamma_m0 { get; set; }  // Partial safety factor
        public double gamma_m2 { get; set; }  // Partial safety factor for bolts

        // Geometric properties for effective lengths
        public double m { get; set; }         // Effective distance (mm)
        public double w { get; set; }         // Web width or related dimension (mm)
        public double a_f { get; set; }       // Fillet radius (mm)
        public double e_x { get; set; }       // Distance to bolt from edge (mm)
        public double b_eff_c_wc { get; set; } // Effective length for column web in compression (mm)
    }
}