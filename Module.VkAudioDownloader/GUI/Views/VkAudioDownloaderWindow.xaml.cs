﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Module.VkAudioDownloader.GUI.Comparers;
using Module.VkAudioDownloader.GUI.ViewModels;
using Module.VkAudioDownloader.Helpers;
using Module.VkAudioDownloader.Settings;
using Root.Collections;
using VkNet.Abstractions;

namespace Module.VkAudioDownloader.GUI.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class VkAudioDownloaderWindow : Window
    {
        public VkAudioDownloaderWindowVM ViewModel { get; }

        private readonly IVkApi _vkApi;
        private readonly IMusicDownloaderSettings _settings;

        public VkAudioDownloaderWindow(VkAudioDownloaderWindowVM viewModel,
            IVkApi vkApi,
            IMusicDownloaderSettings settings)
        {
            InitializeComponent();

            ViewModel = viewModel;
            _vkApi = vkApi;
            _settings = settings;

            DataContext = ViewModel;
        }

        private void VkAudioDownloaderWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sortedAudiosSource = (CollectionViewSource) Resources["SortedAudios"];
            var sortedAudiosView = (ListCollectionView) sortedAudiosSource.View;
            sortedAudiosView.CustomSort = new ReverseComparer(new AudioVMComparer());
        }

        public new void ShowDialog()
        {
            if (!_vkApi.IsAuthorized)
            {
                var token = _settings.AccessToken;
                if (!_vkApi.TryAuth(token))
                {
                    if (_vkApi.TryAuth(TryInputAuthData, TryInputCode))
                    {
                        _settings.AccessToken = _vkApi.Token;
                        _settings.Save();
                    }
                    else
                    {
                        MessageBox.Show("Auth error.");
                        return;
                    }
                }
            }

            base.ShowDialog();
        }

        // TODO move out
        private bool TryInputAuthData(out string? login, out string? password)
        {
            var dialog = new AuthDialog();

            return dialog.ShowDialog(out login, out password);
        }

        // TODO move out
        private bool TryInputCode(out string? code)
        {
            var dialog = new InputDialog();

            return dialog.ShowDialog("Enter code:", out code);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            ICommand cmd = ViewModel.RefreshCmd;
            cmd.Execute(null);
            ViewModel.RefreshCmd.Execute(null);

            base.OnContentRendered(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel.IsDownloading)
            {
                if (MessageBox.Show("Downloading in process. Are you sure to close window?", "!!!",
                        MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }
    }
}