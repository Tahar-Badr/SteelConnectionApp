using Newtonsoft.Json;
using SteelConnection.Models;
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
            List<Models.SteelProfile> profiles = Models.SteelProfile.LoadProfilesFromJson("profiles.json");

           
            string type = "HEA";// Console.ReadLine();
            
            string number = Console.ReadLine();
            int size = 140; // int.Parse(number);
            if (size<0)
            {

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

           

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
           // ApplicationConfiguration.Initialize();
            //Application.Run(new MainForm());
        }
       
    }
}