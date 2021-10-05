﻿using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Module.ArtworksSearcher.Factories;
using Module.DataExporter.Exceptions;
using Module.DataExporter.Services;
using Module.InboxAdder.Services;
using Module.PlaylistsExporter.Services;
using Module.VkAudioDownloader.Factories;
using Root;

namespace MusicBeePlugin.Services
{
    public class PluginActions : IPluginActions
    {
        private readonly MusicBeeApiInterface _mbApi;
        private readonly IDataExportService _dataExportService;
        private readonly IPlaylistsExportService _playlistsExportService;
        private readonly IInboxAddService _inboxAddService;
        private readonly ISearchWindowFactory _searchWindowFactory;
        private readonly IVkAudioDownloaderWindowFactory _vkAudioDownloaderWindowFactory;

        public PluginActions(
            MusicBeeApiInterface mbApi, 
            IDataExportService dataExportService, 
            IPlaylistsExportService playlistsExportService, 
            IInboxAddService inboxAddService, 
            ISearchWindowFactory searchWindowFactory, 
            IVkAudioDownloaderWindowFactory vkAudioDownloaderWindowFactory)
        {
            _mbApi = mbApi;
            _dataExportService = dataExportService;
            _playlistsExportService = playlistsExportService;
            _inboxAddService = inboxAddService;
            _searchWindowFactory = searchWindowFactory;
            _vkAudioDownloaderWindowFactory = vkAudioDownloaderWindowFactory;
        }

        public void SearchArtworks()
        {
            var queryRes = _mbApi.Library_QueryFilesEx.Invoke("domain=SelectedFiles", out var files);

            if (!queryRes)
            {
                MessageBox.Show("Library_QueryFilesEx error.");
                return;
            }
            if (files.Length != 1)
            {
                MessageBox.Show("You must select single composition.");
                return;
            }
            
            var filePath = files[0];
            
            var artist = _mbApi.Library_GetFileTag.Invoke(filePath, MetaDataType.Artist);
            var title = _mbApi.Library_GetFileTag.Invoke(filePath, MetaDataType.TrackTitle);

            var searchWindow = _searchWindowFactory.Create();
            if (searchWindow.ShowDialog(artist, title, out var imageData))
            {
                if (!_mbApi.Library_SetArtworkEx.Invoke(filePath, 0, imageData))
                {
                    MessageBox.Show("Обложка не была сохранена.", "Ошибка");
                }
            }
        }

        public void DownloadVkAudios()
        {
            _vkAudioDownloaderWindowFactory
                .Create()
                .ShowDialog();
        }

        public void AddSelectedFileToLibrary()
        {
            var queryRes = _mbApi.Library_QueryFilesEx.Invoke("domain=SelectedFiles", out var files);

            if (!queryRes || files.Length != 1)
            {
                MessageBox.Show("You must select single item.");
                return;
            }

            _inboxAddService.AddToLibrary(files[0]);

            _mbApi.MB_RefreshPanels();
        }

        public void ExportPlaylists()
        {
            try
            {
                _playlistsExportService.Export();

                MessageBox.Show("Export done successfully.", "(ง ͠° ͟ل͜ ͡°)ง");
            }
            catch (Exception e)
            {
                // TODO dialog
                Console.WriteLine(e);
                throw;
            }
        }

        public void ExportLibraryData()
        {
            using var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                EnsurePathExists = true
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            try
            {
                _dataExportService.Export(dialog.FileName);

                MessageBox.Show("Экспорт выполнен успешно.", "Ок");
            }
            catch (MusicBeeApiException e)
            {
                MessageBox.Show(e.Message, "Error");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Unknown Error");
            }
        }
    }
}