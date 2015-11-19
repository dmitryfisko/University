using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Lab_7.Model
{
    [Serializable]
    public sealed class PlayerModel : ObservableCollection<PlaylistModel>, IPlayerModel
    {
        private readonly DataService<List<PlaylistModel>>.Options _fileOptions;

        private static readonly object _syncRoot = new object();
        private volatile static PlayerModel _instance;
        //public static PlayerModel Instance => _instance ?? (_instance = new PlayerModel());
        // if (_instance == null)
        //      _instance = new ...
        // return _instance;

        public static PlayerModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new PlayerModel();
                        }
                    }        
                }
                return _instance;
            }
        }

        private PlaylistModel _openedPlaylist;
        public PlaylistModel OpenedPlaylist
        {
            get { return _openedPlaylist; }
            set
            {
                _openedPlaylist = value;
                OnPropertyChanged(new PropertyChangedEventArgs("OpenedPlaylist"));
            }
        }

        public PlayerModel()
        {
            CollectionChanged += ObservableCollectionChanged;

            _fileOptions = new DataService<List<PlaylistModel>>
                   .Options(DataService<List<PlaylistModel>>.Options.Modes.Binary, false, false);
            var loadedList = DataService<List<PlaylistModel>>.Load(_fileOptions);

            if (loadedList.Count == 0)
            {
                InitEmpty();
            }
            else
            {
                foreach (var item in loadedList)
                {
                    Add(item);
                }
            }
            OpenPlaylist(Count > 1 ? 1 : 0);

            foreach (var playlist in this)
            {
                ((INotifyPropertyChanged)playlist).PropertyChanged += ItemPropertyChanged;
            }
        }

        public void Click(int index)
        {
            var playlist = this[index];
            if (playlist.State == PlaylistModel.States.Playing)
            {
                playlist.Pause();
            }
            else
            {
                playlist.Play();
            }

        }

        public void Play(int num)
        {
            if (num == 0)
            {
                foreach (var playlist in this)
                {
                    playlist.Play();
                }
            }
            else if (1 <= num && num <= Count)
            {
                this[num - 1].Play();
            }
        }

        public void Play()
        {
            foreach (var playlist in this)
            {
                playlist.Play();
            }
        }

        public void Pause()
        {
            foreach (var playlist in this)
            {
                playlist.Pause();
            }
        }

        public void Stop()
        {
            foreach (var playlist in this)
            {
                playlist.Stop();
            }
        }

        public void SaveState()
        {
            DataService<List<PlaylistModel>>.Save(this.ToList(), _fileOptions);
        }

        private void InitEmpty()
        {
            Add(new PlaylistModel("           +"));
        }

        public void Add(string playlistName)
        {
            Add(new PlaylistModel(playlistName));
        }

        private void ObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                    ((INotifyCollectionChanged)item).CollectionChanged -= ItemCollectionChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                    ((INotifyCollectionChanged)item).CollectionChanged += ItemCollectionChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((PlaylistModel)sender));
            OnCollectionChanged(args);
        }

        private void ItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void OpenPlaylist(int openIndex)
        {
            var prevOpenedPlaylistIndex = IndexOf(OpenedPlaylist);
            if (prevOpenedPlaylistIndex != -1)
            {
                this[prevOpenedPlaylistIndex].Opened = false;
            }
            OpenedPlaylist = this[openIndex];
            OpenedPlaylist.Opened = true;
        }
    }
}
