// See https://aka.ms/new-console-template for more information
using System.Reflection;

Console.WriteLine("Hello, World!");
List<SteelProfile> profiles = Models.SteelProfile.LoadProfilesFromJson("profiles.json");

// Get user input
Console.WriteLine("Enter profile type (HEA, HEB, IPE): ");
string type = "HEA";// Console.ReadLine();

Console.WriteLine("Enter size (e.g., 120, 140): ");
string number = Console.ReadLine();
int size = 140; // int.Parse(number);
if (size < 0)
{
    Console.WriteLine("Invalid size input!");
    return;
}
// Search for the profile
SteelProfile profile = profiles.Find(p => p.ProfileType == type && p.Size == size);
if (profile != null)
{
    Console.WriteLine($"\nProfile: {profile.ProfileType}{profile.Size}");
}
else
{
    Console.WriteLine("Profile not found!");
}
