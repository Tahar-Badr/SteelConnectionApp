using System;
using System.IO;
using System.Text.Json;
using SteelConnection.Models;

public class JointIOHelper
{
    public static void SaveJointInputToFile(JointInput input)
    {
        try
        {
            // location and filename
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "joint_input.json");

            // Optional: Set serialization options (for better formatting)
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Convert the object to JSON string
            string jsonString = JsonSerializer.Serialize(input, options);

            // Write to file
            File.WriteAllText(filePath, jsonString);

            // Optional: Show confirmation
            MessageBox.Show("Joint input data saved to:\n" + filePath, "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error saving file:\n" + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
