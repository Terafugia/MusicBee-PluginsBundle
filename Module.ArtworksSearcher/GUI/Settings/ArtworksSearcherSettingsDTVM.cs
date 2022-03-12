﻿using System;
using System.Windows.Input;
using Root.MVVM;

namespace Module.ArtworksSearcher.GUI.Settings
{
    public class ArtworksSearcherSettingsDTVM : IArtworksSearcherSettingsVM
    {
        public bool IsLoaded => true;

        public string GoogleCX { get; set; } = "{private google cx}";
        public string GoogleKey { get; set; } = "{private google key}";
        public int MaxParallelDownloadsCount => 999;
        public int ParallelDownloadsCount { get; set; } = 6;
        public string OsuSongsDir { get; set; } = @"C:\Games\osu!\Songs";
        public long MinOsuImageByteSize { get; set; } = 100_000;

        public ICommand ChangeOsuSongsDirCmd { get; } =
            new RelayCommand(_ => throw new NotSupportedException());

        public bool Load()
        {
            throw new NotSupportedException();
        }

        public bool Save()
        {
            throw new NotSupportedException();
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}