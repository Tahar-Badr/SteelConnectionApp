namespace SteelConnection.Models
{
    public class Bolt
    {
        public enum BoltType { M12, M16, M20, M24, M27, M30, M36 }
        public BoltType Type { get; set; }
        public double Diameter { get; set; }   // in mm
        public int NumberOfRows { get; set; }

        // New properties for tension resistance
        public string Grade { get; set; }      // "4.6", "8.8", etc.
        public bool IsCountersunk { get; set; } // Affects tension resistance

        public Bolt(BoltType type, string grade, int numberOfRows, bool isCountersunk = false)
        {
            Type = type;
            Grade = grade;
            NumberOfRows = numberOfRows;
            IsCountersunk = isCountersunk;
            Diameter = GetDiameterFromType(type);
        }

        public static double GetDiameterFromType(BoltType type)
        {
            return type switch
            {
                BoltType.M12 => 12,
                BoltType.M16 => 16,
                BoltType.M20 => 20,
                BoltType.M24 => 24,
                BoltType.M27 => 27,
                BoltType.M30 => 30,
                BoltType.M36 => 36,
                _ => throw new ArgumentException("Invalid bolt type.")
            };
        }
    }
}