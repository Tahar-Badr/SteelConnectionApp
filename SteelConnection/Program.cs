using Newtonsoft.Json;
using SteelConnection.data;
using SteelConnection.Models;
using SteelConnection.Operations;
using SteelConnection.Views;
using System;
using System.Collections.Generic;
using System.IO;


namespace SteelConnection
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            List<SteelProfile> profiles = ProfileLoader.Load("profiles.json");

            // Sample input — you can later get these dynamically
            string beamType = "HEA";
            int beamSize = 140;

            string columnType = "HEA";
            int columnSize = 200;

            // Find the profiles
            SteelProfile beamProfile = profiles.Find(p => p.ProfileType == beamType && p.Size == beamSize);
            SteelProfile columnProfile = profiles.Find(p => p.ProfileType == columnType && p.Size == columnSize);

            if (beamProfile == null || columnProfile == null)
            {
                Console.WriteLine("Beam or Column profile not found!");
                return;
            }

            Console.WriteLine($"\nBeam: {beamProfile.DisplayName}");
            Console.WriteLine($"Column: {columnProfile.DisplayName}");

            // Create bolt setup
            var bolts = new List<Bolt>
    {
        new Bolt(Bolt.BoltType.M20, "8.8", 2),
        new Bolt(Bolt.BoltType.M20, "8.8", 2)
    };

            // Create JointInput object
            var input = new JointInput
            {
                Beam = new JointBeam { Profile = beamProfile },
                Column = new JointColumn { Profile = columnProfile },
                Bolts = bolts,
                A_vc = 500,
                Beta = 0.9,
                B_eff_c = 100,
                T_wc = columnProfile.WebThickness,
                H_wc = columnProfile.Height,
                Leff_tfc = new double[] { 100, 100 },
                T_fc = beamProfile.FlangeThickness,
                M_fc = new double[] { 10e6, 10e6 },
                Leff_tp = new double[] { 100, 100 },
                T_p = 10,
                M_p = new double[] { 5e6, 5e6 },
                A_s = 1000,
                L_b = 3000,
                F_Rd = new double[] { 150e3, 150e3 },
                H = new double[] { 250, 250 },
                E = 210000, // MPa
                EIb = 1e9,
                Mpl_Rd = 3e6,
                IsBraced = true
            };

            // Run the calculation
            var result = JointAssembler.Calculate(input);
            Console.WriteLine($"\nMoment Resistance (Mj,Rd): {result.Mj_Rd}");
            Console.WriteLine($"Initial Stiffness (Sj_ini): {result.Sj_ini}");
            Console.WriteLine($"Joint Classification - Strength: {result.Classification.Strength}, Stiffness: {result.Classification.Stiffness}");
        }

    }
}