using System;
using System.Windows.Input;
using Lab_7.Model;

namespace Lab_7.Command
{
    public class MenuNextCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var playlist = parameter as PlaylistModel;
            playlist?.NextSong();
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
