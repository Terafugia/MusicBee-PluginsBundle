﻿using System.Windows;

namespace ArtworksSearcher.GUI
{
    public partial class SettingsDialog : Window
    {
        private readonly SettingsDialogVM _viewModel;

        public SettingsDialog(Settings settings)
        {
            InitializeComponent();
            _viewModel = new SettingsDialogVM(this, settings);
            DataContext = _viewModel;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SaveChanges())
                DialogResult = true;
        }
    }
}