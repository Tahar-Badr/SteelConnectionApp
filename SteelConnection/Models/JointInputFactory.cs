using System;
using System.Collections.Generic;
using System.Linq;
using SteelConnection.data;

namespace SteelConnection.Models
{
    public static class JointInputFactory
    {
        private static SteelProfile GetProfile(string profileType, int size, string jsonPath)
        {
            var profiles = ProfileLoader.Load(jsonPath);

            var profile = profiles.FirstOrDefault(p =>
                p.ProfileType.Equals(profileType, StringComparison.OrdinalIgnoreCase)
                && p.Size == size);

            if (profile == null)
            {
                Console.WriteLine($"[ERROR] Steel profile not found: {profileType} {size}.");
                throw new ArgumentException($"Steel profile {profileType} {size} not found in file: {jsonPath}");
            }

            return profile;
        }

        public static JointInput Create(
            string beamType, int beamSize,
            string columnType, int columnSize,
            string jsonPath,
            List<Bolt> bolts,
            EndPlate endPlate,
            double leverArmZ,
            bool isBraced)
        {
            return new JointInput
            {
                Beam = new JointBeam { Profile = GetProfile(beamType, beamSize, jsonPath) },
                Column = new JointColumn { Profile = GetProfile(columnType, columnSize, jsonPath) },
                Bolts = bolts,
                EndPlate = endPlate,
                LeverArmZ = leverArmZ,
                IsBracedFrame = isBraced
            };
        }
    }
}
