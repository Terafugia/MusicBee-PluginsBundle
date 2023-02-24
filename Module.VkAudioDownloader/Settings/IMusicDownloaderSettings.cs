﻿using Module.Settings.Entities.Abstract;

namespace Module.VkAudioDownloader.Settings;

public interface IMusicDownloaderSettings : ISettings
{
    string DownloadDirTemplate { get; set; }
    string FileNameTemplate { get; set; }
}