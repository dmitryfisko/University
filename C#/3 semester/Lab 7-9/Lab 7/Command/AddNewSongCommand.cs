using System;
using System.Windows.Input;
using Lab_7.ViewModel;

namespace Lab_7.Command
{
    public class AddNewSongCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Music",
                Filter = "MP3|*.mp3|AAC|*.m4a;*.flac|All files|*.*",
                RestoreDirectory = true,
                Multiselect = true
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var playlist = parameter as PlaylistViewModel;
                foreach (var songPath in dialog.FileNames)
                {
                    playlist?.Add(songPath);
                }
            }
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
