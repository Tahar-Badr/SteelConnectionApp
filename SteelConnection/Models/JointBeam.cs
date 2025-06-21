using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelConnection.Models
{
    public class JointBeam
    {
        public SteelProfile Profile { get; set; }
        public double YieldStrength => Profile.YieldStrength;
        public double Height => Profile.Height;
        public double FlangeThickness => Profile.FlangeThickness;
        public double WebThickness => Profile.WebThickness;
    }
}
