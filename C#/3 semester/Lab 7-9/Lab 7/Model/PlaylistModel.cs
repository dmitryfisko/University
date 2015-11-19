using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Media;
using CSCore.Codecs;
using CSCore.SoundOut;
using GalaSoft.MvvmLight.Command;

namespace Lab_7.Model
{
    [Serializable]
    public sealed class PlaylistModel : ObservableCollection<SongModel>
    {
        [NonSerialized]
        private static readonly Color ActiveColor = Colors.DodgerBlue;
        [NonSerialized]
        private static readonly Color InactiveColor = Colors.Black;
        private const string EmptyIconPath = "";
        private const string PlayIconPath = "../Resources/Images/button_play.png";
        private const string PauseIconPath = "../Resources/Images/button_pause.png";

        public enum States
        {
            Playing,
            Paused,
            Stopped
        };


        [NonSerialized]
        public States State = States.Stopped;
        [NonSerialized]
        private Thread _thread;
        [NonSerialized]
        private ISoundOut _soundOut;
        private int _currentSongNum;

        [NonSerialized]
        private Color _foregroundColor;
        public Color ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                _foregroundColor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ForegroundColor"));
            }
        }

        public bool Opened
        {
            set
            {
                ForegroundColor = value ? ActiveColor : InactiveColor;
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }

        public string ItemIconPath
        {
            get
            {
                if (Count == 0)
                {
                    return EmptyIconPath;
                }
                if (State == States.Playing)
                {
                    return PauseIconPath;
                }
                return PlayIconPath;
            }
        }

        public PlaylistModel()
        {
            CollectionChanged += ObservableCollectionChanged;
        }

        public PlaylistModel(string name) : this()
        {
            Name = name;
            ForegroundColor = InactiveColor;
        }

        public PlaylistModel(IEnumerable<SongModel> pItems) : this()
        {
            foreach (var item in pItems)
            {
                Add(item);
            }
        }

        [OnDeserialized]
        void OnDeserializing(StreamingContext ctx)
        {
            CollectionChanged += ObservableCollectionChanged;
            foreach (var item in this)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }

            ForegroundColor = InactiveColor;
            State = States.Stopped;
        }

        public void ChangeSongState(int songIndex)
        {
            if (songIndex == _currentSongNum)
            {
                if (State == States.Playing)
                {
                    Stop();
                }
                else
                {
                    Stop();
                    Play();
                }
            }
            else
            {
                Stop();
                Play(songIndex);
            }
        }

        private void ChangeCurSongNum(int newPosition)
        {
            if (newPosition == -1)
            {
                return;
            }
            if (_currentSongNum != -1)
            {
                this[_currentSongNum].IsPlaying = false;
            }
            this[newPosition].IsPlaying = true;
            _currentSongNum = newPosition;
        }

        public void NextSong()
        {
            var played = State == States.Playing ? true : false;
            NextSongNum();
            if (played)
            {
                Stop();
                Play();
            }
        }

        public void Play()
        {
            if (Count < 1 || State == States.Playing)
            {
                return;
            }

            State = States.Playing;

            _thread = new Thread(Listen);
            _thread.Start();
            OnPropertyChanged(new PropertyChangedEventArgs("ItemIconPath"));
        }

        public void Play(int songNum)
        {
            ChangeCurSongNum(songNum);
            Play();
        }

        public void Pause()
        {
            if (Count < 1)
            {
                return;
            }
            State = States.Paused;
            _soundOut?.Pause();
            OnPropertyChanged(new PropertyChangedEventArgs("ItemIconPath"));
        }

        public void Stop()
        {
            if (Count < 1)
            {
                return;
            }
            State = States.Stopped;

            _thread?.Abort();
            _soundOut?.Stop();
            _soundOut?.Dispose();
            _soundOut = null;
            OnPropertyChanged(new PropertyChangedEventArgs("ItemIconPath"));
        }

        private void Listen()
        {
            while (Thread.CurrentThread.IsAlive && State == States.Playing)
            {
                InitPlayer();
                do
                {
                    _soundOut.Play();
                    Thread.Sleep(1000);
                } while (Thread.CurrentThread.IsAlive && _soundOut?.PlaybackState == PlaybackState.Playing);

                if (State == States.Playing)
                {
                    NextSongNum();
                }
            }
        }

        private void NextSongNum()
        {
            var newPosition = _currentSongNum;
            ++newPosition;
            if (newPosition == Count)
            {
                newPosition = 0;
            }
            ChangeCurSongNum(newPosition);
        }

        private void InitPlayer()
        {
            if (_soundOut == null)
            {
                _soundOut = new WasapiOut();
            }
            if (_soundOut.PlaybackState == PlaybackState.Stopped)
            {
                var soundSource = CodecFactory.Instance.GetCodec(this[_currentSongNum].SongPath);
                _soundOut.Initialize(soundSource);
            }
        }

        public void Add(string songPath)
        {
            Add(new SongModel(songPath));
            if (Count == 1)
            {
                ChangeCurSongNum(0);
                Stop();
            }
        }

        private void ObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SongModel item in e.NewItems)
                {
                    item.PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (SongModel item in e.OldItems)
                {
                    item.PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((SongModel)sender));
            OnCollectionChanged(args);
        }
    }
}
