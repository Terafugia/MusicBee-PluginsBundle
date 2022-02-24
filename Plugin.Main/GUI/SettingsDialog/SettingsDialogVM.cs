﻿using System.Collections.ObjectModel;
using System.Linq;
using Module.ArtworksSearcher.GUI.Settings;
using Module.PlaylistsExporter.GUI.Settings;
using Module.VkAudioDownloader.GUI.Settings;
using PropertyChanged;
using Root.Abstractions;
using Root.MVVM;

namespace MusicBeePlugin.GUI.SettingsDialog
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsDialogVM : ISettings
    {
        public bool IsLoaded => Settings.All(s => s.ModuleSettings.IsLoaded);
        
        public ObservableCollection<ModuleSettingsVM> Settings { get; } = new ();

        public ModuleSettingsVM SelectedSettingsModule { get; set; }
        
        private RelayCommand? _resetCmd;
        public RelayCommand ResetCmd
            => _resetCmd ??= new RelayCommand(_ => Reset());
        
        public SettingsDialogVM(
            IMusicDownloaderSettingsVM musicDownloaderSettingsVM,
            IArtworksSearcherSettingsVM artworksSearcherSettingsVM,
            IPlaylistsExporterSettingsVM playlistsExporterSettingsVM)
        {
            Settings.Add(new ModuleSettingsVM("Music downloader", 
                musicDownloaderSettingsVM));
            Settings.Add(new ModuleSettingsVM("Artworks searcher", 
                artworksSearcherSettingsVM));
            Settings.Add(new ModuleSettingsVM("Playlists exporter", 
                playlistsExporterSettingsVM));

            SelectedSettingsModule = Settings.First();
            
            Load();
        }

        public void Load()
        {
            foreach (var setting in Settings)
            {
                setting.ModuleSettings.Load();
            }
        }

        public bool Save()
        {
            foreach (var setting in Settings)
            {
                if (!setting.ModuleSettings.Save())
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            foreach (var setting in Settings)
            {
                setting.ModuleSettings.Reset();
            }
        }
    }
}