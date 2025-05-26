using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SteelConnection.Models;

namespace SteelConnection.data
{
    public static class ProfileLoader
    {
        private static List<SteelProfile> _cachedProfiles;

        /// <summary>
        /// Loads steel profiles from a JSON file. Uses cache unless forceReload is true.
        /// </summary>
        /// <param name="jsonFileName">JSON file name (e.g., "profiles.json")</param>
        /// <param name="forceReload">Reload from file even if already loaded</param>
        /// <returns>List of SteelProfile</returns>
        public static List<SteelProfile> Load(string jsonFileName, bool forceReload = false)
        {
            if (_cachedProfiles != null && !forceReload)
                return _cachedProfiles;

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(basePath, "Resources", jsonFileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Profile JSON file not found at path: {fullPath}");

            string json = File.ReadAllText(fullPath);
            _cachedProfiles = JsonConvert.DeserializeObject<List<SteelProfile>>(json);

            if (_cachedProfiles == null || _cachedProfiles.Count == 0)
                throw new Exception("Failed to load profiles or profile list is empty.");

            return _cachedProfiles;
        }
    }
}
