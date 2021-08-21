﻿using Newtonsoft.Json.Linq;
using Root.Abstractions;

namespace Module.VkMusicDownloader.Settings
{
    public class MusicDownloaderSettings : BaseSettings, IMusicDownloaderSettings
    {
        public string DownloadDirTemplate { get; set; } = "";
        public string FileNameTemplate { get; set; } = "";
        public string AccessToken { get; set; } = "";
        
        public MusicDownloaderSettings(string filePath) : base(filePath, true)
        {
            
        }
        
        protected override void PropertiesFromJObject(JToken rootObj)
        {
            DownloadDirTemplate = rootObj.Value<string>(nameof(DownloadDirTemplate));
            FileNameTemplate = rootObj.Value<string>(nameof(FileNameTemplate));
            AccessToken = rootObj.Value<string>(nameof(AccessToken));
        }

        protected override JObject PropertiesToJObject()
        {
            return new()
            {
                [nameof(DownloadDirTemplate)] = DownloadDirTemplate,
                [nameof(FileNameTemplate)] = FileNameTemplate,
                [nameof(AccessToken)] = AccessToken
            };
        }
    }
}
