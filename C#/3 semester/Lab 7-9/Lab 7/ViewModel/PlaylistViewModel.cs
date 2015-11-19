using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Lab_7.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Lab_7.Command;
using Lab_7.Resources;


namespace Lab_7.ViewModel
{
    public sealed class PlaylistViewModel : IEnumerable<SongModel>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly IPlayerModel _player;
    
        public PlaylistViewModel()
        {
            _player = SimpleIoc.Default.GetInstance<IPlayerModel>();

            ((INotifyPropertyChanged)_player).PropertyChanged += ItemPropertyChanged;
            ((INotifyCollectionChanged)_player).CollectionChanged += ItemCollectionChanged;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("TotalAlbumDuration");
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void ItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public string TotalAlbumDuration => _player.OpenedPlaylist.TotalDuration();

        public void ChangeSongState(int songIndex) => _player.OpenedPlaylist.ChangeSongState(songIndex);

        public void Add(string songPath)
        {
            _player.OpenedPlaylist.Add(songPath);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator GetEnumerator()
        {
            return _player.OpenedPlaylist.GetEnumerator();
        }

        IEnumerator<SongModel> IEnumerable<SongModel>.GetEnumerator()
        {
            return _player.OpenedPlaylist.GetEnumerator();
        }

        #region Commands

        public AddNewSongCommand AddNewSongCommand { get; set; } = new AddNewSongCommand();

        public RelayCommand<int> PlaylistDoubleClickCommand => new RelayCommand<int>(ChangeSongState, param => true);

        public RelayCommand<int> LocalizationComboBoxChangedCommand => new RelayCommand<int>(selectedIndex =>
        {
            CultureResources.ChangeCulture(selectedIndex == 0
                ? Properties.Settings.Default.DefaultCulture
                : new CultureInfo("ru"));
        });

        #endregion

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
