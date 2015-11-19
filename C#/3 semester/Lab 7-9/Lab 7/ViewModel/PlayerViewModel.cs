using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Lab_7.Command;
using Lab_7.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Lab_7.Resources;
using Lab_7.View;

namespace Lab_7.ViewModel
{
    public sealed class PlayerViewModel : IEnumerable<PlaylistModel>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly IPlayerModel _player;

        private PlaylistModel _openedPlaylist;
        public PlaylistModel OpenedPlaylist
        {
            get { return _openedPlaylist; }
            set
            {
                _openedPlaylist = value;
                RaisePropertyChanged("OpenedPlaylist");
            }
        }

        public PlayerViewModel()
        {
            _player = SimpleIoc.Default.GetInstance<IPlayerModel>();
            ;
            ((INotifyPropertyChanged) _player).PropertyChanged += ItemPropertyChanged;
            ((INotifyCollectionChanged) _player).CollectionChanged += ItemCollectionChanged;
            OpenedPlaylist = _player.OpenedPlaylist;

            CultureResources.ResourceProvider.DataChanged += EmptyFunctionRequiredForLocalizationWorking;
            Messenger.Default.Register<AddNewPlaylistMessageModel>(this, action => Add(action.NewPlaylistName));
        }

        private void Click(int index) => _player.Click(index);

        public void Play() => _player.Play();

        public void Pause() => _player.Pause();

        public void Stop() => _player.Stop();

        private void SaveState() => _player.SaveState();

        private void Add(string playlistName)
        {
            _player.Add(playlistName);
            _player.OpenPlaylist(_player.IndexOf(_player.Last()));
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OpenedPlaylist = _player.OpenedPlaylist;
            //RaisePropertyChanged("OpenedPlaylist");
        }

        private void ItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator GetEnumerator()
        {
            return _player.GetEnumerator();
        }

        IEnumerator<PlaylistModel> IEnumerable<PlaylistModel>.GetEnumerator()
        {
            return _player.GetEnumerator();
        }

        static void EmptyFunctionRequiredForLocalizationWorking(object sender, EventArgs e) {}

        #region Commands

        public MenuNextCommand MenuNextCommand { get; set; } = new MenuNextCommand();
        public MenuPlayCommand MenuPlayCommand { get; set; } = new MenuPlayCommand();
        public MenuPauseCommand MenuPauseCommand { get; set; } = new MenuPauseCommand();
        public MenuStopCommand MenuStopCommand { get; set; } = new MenuStopCommand();

        public RelayCommand<CancelEventArgs> WindowClosingCommand => new RelayCommand<CancelEventArgs>(e =>
        {
            Stop();
            SaveState();
        });

        public RelayCommand<object> PlaylistViewTextClickCommand => new RelayCommand<object>(sender =>
        {
            var index = PlaylistViewClickedItemPosition(sender);
            if (index == 0)
            {
                AddNewPlaylist();
            }
            else
            {
                OpenPlaylist(index);
            }
        });

        public RelayCommand<object> PlaylistViewImageClickCommand => new RelayCommand<object>(sender =>
        {
            var index = PlaylistViewClickedItemPosition(sender);
            Click(index);
        });

        #endregion

        private int PlaylistViewClickedItemPosition(object sender) => _player.IndexOf(sender as PlaylistModel);

        private void OpenPlaylist(int openIndex)
        {
            _player.OpenPlaylist(openIndex);
        }

        private static void AddNewPlaylist()
        {
            var dialog = new EnterDialog();
            dialog.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var handler = Volatile.Read(ref CollectionChanged);
            handler?.Invoke(this, args);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = Volatile.Read(ref PropertyChanged);
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
