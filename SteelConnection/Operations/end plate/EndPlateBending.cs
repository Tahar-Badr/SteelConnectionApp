using System;
using SteelConnection.Models;

namespace SteelConnection.Operations.EndPlate
{
    internal static class EndPlateBending
    {
        public class ResistanceResults
        {
            public double Mode1 { get; set; }  // Plate yielding
            public double Mode2 { get; set; }  // Bolt failure with plate yielding
            public double Mode3 { get; set; }  // Bolt failure
            public double Governing => Math.Min(Mode1, Math.Min(Mode2, Mode3));
        }

        public static (ResistanceResults Row1, ResistanceResults Row2,
                       ResistanceResults Row3, ResistanceResults Group23) Calculate(
            double t_p, double f_yp,
            double e, double e_x, double w, double b_p, double p,
            double u2, double t_wb, double t_fb, double a_f,
            double B_tRd)
        {
            // Derived geometry from Eurocode
            double m_x = u2 - 0.8 * a_f * Math.Sqrt(2) - e_x;
            double m = (w / 2) - (t_wb / 2) - 0.8 * a_f * Math.Sqrt(2);
            double m2 = e_x + e - u2 - t_fb - 0.8 * a_f * Math.Sqrt(2);  // Not used here

            double m_plRd = (Math.Pow(t_p, 2) * f_yp) / (4 * SafetyFactors.GammaM0);
            double n_x = Math.Min(e_x, 1.25 * m_x);

            double lambda1 = m / (m + e);
            double lambda2 = (e_x + e - 0.8 * a_f * Math.Sqrt(2)) / (m + e);
            double alpha = Math.Min(6 * lambda1, Math.Pow(1.5 * lambda2, 0.8));

            return (
                Row1: CalculateRow1(m_x, e_x, w, b_p, m_plRd, n_x, B_tRd),
                Row2: CalculateRow2(m, e, p, alpha, m_plRd, n_x, B_tRd),
                Row3: CalculateRow3(m, e, p, m_plRd, n_x, B_tRd),
                Group23: CalculateGroup23(m, e, p, m_plRd, n_x, B_tRd)
            );
        }

        private static ResistanceResults CalculateRow1(
            double m_x, double e_x, double w, double b_p,
            double m_plRd, double n_x, double B_tRd)
        {
            var res = new ResistanceResults();

            double l_eff_cp = Math.Min(
                2 * Math.PI * m_x,
                Math.Min(Math.PI * m_x + w, Math.PI * m_x + 2 * e_x)
            );

            double l_eff_nc = Math.Min(
                Math.Min(4 * m_x + 1.25 * e_x,
                         e_x + 2 * m_x + 0.625 * e_x),
                Math.Min(0.5 * b_p, 0.5 * w + 2 * m_x + 0.625 * e_x)
            );

            res.Mode1 = (4 * l_eff_cp * m_plRd) / m_x;
            res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * 2 * B_tRd) / (m_x + n_x);
            res.Mode3 = 2 * B_tRd;

            return res;
        }

        private static ResistanceResults CalculateRow2(
            double m, double e, double p, double alpha,
            double m_plRd, double n_x, double B_tRd)
        {
            var res = new ResistanceResults();

            double l_eff_cp = Math.Min(2 * Math.PI * m, Math.PI * m + p);
            double l_eff_nc = Math.Min(alpha * m, 0.5 * p + alpha * m - (2 * m + 0.625 * e));

            res.Mode1 = (4 * l_eff_cp * m_plRd) / m;
            res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * 2 * B_tRd) / (m + n_x);
            res.Mode3 = 2 * B_tRd;

            return res;
        }

        private static ResistanceResults CalculateRow3(
            double m, double e, double p,
            double m_plRd, double n_x, double B_tRd)
        {
            var res = new ResistanceResults();

            double l_eff_cp = Math.Min(2 * Math.PI * m, Math.PI * m + p);
            double l_eff_nc = Math.Min(4 * m + 1.25 * e, 2 * m + 0.625 * e + 0.5 * p);

            res.Mode1 = (4 * l_eff_cp * m_plRd) / m;
            res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * 2 * B_tRd) / (m + n_x);
            res.Mode3 = 2 * B_tRd;

            return res;
        }

        private static ResistanceResults CalculateGroup23(
            double m, double e, double p,
            double m_plRd, double n_x, double B_tRd)
        {
            var res = new ResistanceResults();

            double l_eff_cp = 2 * p;
            double l_eff_nc = p;

            res.Mode1 = (4 * l_eff_cp * m_plRd) / m;
            res.Mode2 = (2 * l_eff_nc * m_plRd + n_x * 2 * B_tRd) / (m + n_x);
            res.Mode3 = 2 * B_tRd;

            return res;
        }

        public static double GetDesignResistance(
            (ResistanceResults Row1, ResistanceResults Row2,
             ResistanceResults Row3, ResistanceResults Group23) results,
            int activeRows)
        {
            return activeRows switch
            {
                1 => results.Row1.Governing,
                2 => results.Row2.Governing,
                3 => Math.Min(results.Row3.Governing, results.Group23.Governing),
                _ => throw new ArgumentException("Supported active rows: 1-3")
            };
        }
    }
}