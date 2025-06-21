using SteelConnection.Models;
using System.Text.Json;

private JointInput BuildJointInputFromForm()
{
    var input = new JointInput
    {
        A_vc = double.Parse(txt_Avc.Text),
        Beta = double.Parse(txt_Beta.Text),
        A_f = double.Parse(txt_Af.Text),
        B_eff_c = double.Parse(txt_Beffc.Text),
        T_wc = double.Parse(txt_Twc.Text),
        H_wc = double.Parse(txt_Hwc.Text),
        T_fb = double.Parse(txt_Tfb.Text),
        T_fc = double.Parse(txt_Tfc.Text),
        Leff_tfc = txt_LeffTfc.Text.Split(',').Select(double.Parse).ToArray(),
        S_p = double.Parse(txt_Sp.Text),
        Sj_ini = double.Parse(txt_SjIni.Text),
        R_c = double.Parse(txt_Rc.Text),
        M_fc = txt_Mfc.Text.Split(',').Select(double.Parse).ToArray(),
        M_p = txt_Mp.Text.Split(',').Select(double.Parse).ToArray(),
       // Mpl_Rd = double.Parse(txt_MplRd.Text),
        LeverArmZ = double.Parse(txt_LeverArmZ.Text),
        //IsBraced = chk_IsBraced.Checked,
        //IsBracedFrame = chk_IsBracedFrame.Checked,
        f_ywc = double.Parse(txt_fywc.Text),
        Leff_tp = txt_LeffTp.Text.Split(',').Select(double.Parse).ToArray(),
        T_p = double.Parse(txt_Tp.Text),
        A_s = double.Parse(txt_As.Text),
        L_b = double.Parse(txt_Lb.Text),
        //EIb = double.Parse(txt_EIb.Text),
       // F_Rd = txt_FRd.Text.Split(',').Select(double.Parse).ToArray(),
        H = txt_H.Text.Split(',').Select(double.Parse).ToArray(),
        d_wc = double.Parse(txt_dwc.Text),
       // E = double.Parse(txt_E.Text),

        Column = new JointColumn
        {
            SteelProfile = new SteelProfile
            {
                ProfileType = cmb_ColProfileType.Text,
                Size = cmb_ColSize.Text
            }
        },
        Beam = new JointBeam
        {
            SteelProfile = new SteelProfile
            {
                ProfileType = cmb_BeamProfileType.Text,
                Size = cmb_BeamSize.Text
            }
        },
        EndPlate = new EndPlate
        {
            Thickness = double.Parse(txt_Tp.Text),
            EffectiveLength = double.Parse(txt_LeffTp.Text),
            MomentResistance = txt_Mp.Text.Split(',').Select(double.Parse).ToArray()
        },
        Bolts = new List<Bolt>
        {
            new Bolt
            {
                Type = cmb_BoltType.Text,
                Grade = cmb_BoltGrade.Text,
                NumberOfRows = int.Parse(txt_BoltRows.Text),
                IsCountersunk = chk_IsCountersunk.Checked
            }
        }
    };

    return input;
}
