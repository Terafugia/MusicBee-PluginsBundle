﻿using System.IO;
using Root;

namespace Module.PlaylistsExporter.Helpers
{
    internal static class ConfigurationHelper
    {
        public static string GetSettingsFilePath(MusicBeeApiInterface mbApi)
        {
            var dataPath = mbApi.Setting_GetPersistentStoragePath();
            
            return Path.Combine(dataPath, SettingsDirName, SettingsFileName);
        }
        
        private static string SettingsDirName => "Laiser399_PlaylistsExporter";

        private static string SettingsFileName => "settings.json";
    }
}