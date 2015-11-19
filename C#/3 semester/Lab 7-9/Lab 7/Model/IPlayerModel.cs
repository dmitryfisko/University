using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Lab_7.ViewModel;

namespace Lab_7.Model
{
    public interface IPlayerModel: IEnumerable<PlaylistModel>
    {
        PlaylistModel OpenedPlaylist { get; set; }
        void Click(int index);
        void Play(int num);
        void Play();
        void Pause();
        void Stop();
        void SaveState();
        void OpenPlaylist(int openIndex);
        int IndexOf(PlaylistModel item);
        void Add(string playlistName);
    }
}
