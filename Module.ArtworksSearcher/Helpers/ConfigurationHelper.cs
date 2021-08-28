﻿using System.IO;
using Root;

namespace Module.ArtworksSearcher.Helpers
{
    public class ConfigurationHelper
    {
        public static string GetSettingsFilePath(MusicBeeApiInterface mbApi)
        {
            var dataPath = mbApi.Setting_GetPersistentStoragePath();
            
            return Path.Combine(dataPath, SettingsDirName, SettingsFileName);
        }

        private static string SettingsDirName => "Laiser399_ArtworksSearcher";

        private static string SettingsFileName => "settings.json";
    }
}