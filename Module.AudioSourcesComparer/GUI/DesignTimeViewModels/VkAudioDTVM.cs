﻿using System.Windows.Input;
using Module.AudioSourcesComparer.GUI.AbstractViewModels;
using Root.MVVM;

namespace Module.AudioSourcesComparer.GUI.DesignTimeViewModels
{
    public class VkAudioDTVM : IVkAudioVM
    {
        public int Id { get; }
        public string Artist { get; }
        public string Title { get; }

        public ICommand DeleteCmd => new RelayCommand(_ => { });

        public VkAudioDTVM()
        {
            Id = 7729;
            Artist = "Some Artist";
            Title = "Some Song";
        }

        public VkAudioDTVM(int id, string artist, string title)
        {
            Id = id;
            Artist = artist;
            Title = title;
        }
    }
}