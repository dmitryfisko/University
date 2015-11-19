using System;
using System.ComponentModel;
using System.Threading;

namespace Lab_7.Model
{
    [Serializable]
    public class SongModel : INotifyPropertyChanged
    {
        public SongModel() {}

        public string ID { set; get; }
        public string Artist { set; get; }
        public string SongName { set; get; }
        public string Duration { set; get; }
        public string SongPath { set; get; }
        public long TotalDuration { set; get; }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged("IsPlaying");
            }
        }


        public SongModel(string songPath)
        {
            ID = Guid.NewGuid().ToString();
            SongPath = songPath;
            var tagFile = TagLib.File.Create(songPath);
            Artist = tagFile.Tag.FirstPerformer;
            SongName = tagFile.Tag.Title;
            var duration = tagFile.Properties.Duration;
            Duration = "" + duration.Minutes + ":" + duration.Seconds.ToString("D2");
            TotalDuration = duration.Ticks;
        }

        [field : NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = Volatile.Read(ref PropertyChanged);
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
