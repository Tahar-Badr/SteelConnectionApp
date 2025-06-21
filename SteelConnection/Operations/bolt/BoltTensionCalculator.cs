using System;
using SteelConnection.data;
using SteelConnection.Models;

namespace SteelConnection.Operations.Bolt
{
    internal static class BoltTensionCalculator
    {
        public static double Calculate(SteelConnection.Models.Bolt bolt)
        {
            var resistanceTable = BoltResistanceLoader.Load();

            string boltTypeStr = bolt.Type.ToString(); // e.g., "M16"

            if (!resistanceTable.TryGetValue(bolt.Grade, out var gradeDict))
                throw new InvalidOperationException($"Bolt grade '{bolt.Grade}' not found in resistance table.");

            if (!gradeDict.TryGetValue(boltTypeStr, out double baseResistance))
                throw new InvalidOperationException($"Bolt type '{boltTypeStr}' not found for grade '{bolt.Grade}'.");

            // Adjust for countersunk bolts: use factor 0.63/0.9 = 0.7
            double adjustmentFactor = bolt.IsCountersunk ? (0.63 / 0.9) : 1.0;

            // Total resistance = base × adjustment × number of rows
            return baseResistance * adjustmentFactor * bolt.NumberOfRows;
        }
    }
}
