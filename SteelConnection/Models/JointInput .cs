using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Models
{
    public class JointInput
    {
        public double A_vc { get; set; }
        public double A_f { get; set; }

        public double Beta { get; set; }
        public double B_eff_c { get; set; }
        public double T_wc { get; set; }
        public double H_wc { get; set; }
        public double T_fb { get; set; }
        public double[] Leff_tfc { get; set; }
        public double T_fc { get; set; }
        public double S_p { get; set; }
        public double Sj_ini { get; set; }
        public double R_c { get; set; }             // Column root radius
        public double[] M_fc { get; set; }          // Moment in column flange 
        public double[] M_p { get; set; }           // Moment in end plate 
        public double Mpl_Rd { get; set; }          // Plastic moment resistance of the beam
        public double LeverArmZ { get; set; }
        public bool IsBraced { get; set; }
        public bool IsBracedFrame { get; set; }
        public double f_ywc { get; set; } 
        public double[] Leff_tp { get; set; }
        public double T_p { get; set; }
        public EndPlate EndPlate { get; set; }
        public double A_s { get; set; }
        public double L_b { get; set; }
        public double EIb { get; set; }  // Beam stiffness (E × I)
        public double[] F_Rd { get; set; }
        public double[] H { get; set; }
        public double d_wc { get; set; }  // Depth of column web
        public JointColumn Column { get; set; }
        public JointBeam Beam { get; set; }
        public double E { get; set; }
        public List<Bolt> Bolts { get; set; }

    }
}
