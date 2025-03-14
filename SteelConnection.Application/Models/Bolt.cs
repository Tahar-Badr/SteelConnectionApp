namespace SteelConnection.Models
{
    public class Bolt
    {
        public string Type { get; set; }
        public double Diameter { get; set; }
        public int NumRows { get; set; }

        public Bolt(string type, double diameter, int numRows)
        {
            Type = type;
            Diameter = diameter;
            NumRows = numRows;
        }


    }
}
