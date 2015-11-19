using System.ComponentModel;
using System.Threading;
using Lab_7.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;


namespace Lab_7.ViewModel
{
    public sealed class DialogViewModel : INotifyPropertyChanged
    {

        private bool _dialogResult;
        public bool DialogResult
        {
            get { return _dialogResult; }
            set
            {
                _dialogResult = value; 
                RaisePropertyChanged("DialogResult");
            }
        }

        private string _responseText;
        public string ResponseText
        {
            get
            {
                return _responseText;
            }
            set
            {
                _responseText = value;
                RaisePropertyChanged("ResponseText");
                RaisePropertyChanged("NewPlayListEnterTextCommand");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var handler = Volatile.Read(ref PropertyChanged);
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands

        public RelayCommand<string> NewPlayListEnterTextCommand
        {
            get
            {
                return new RelayCommand<string>(param =>
                {
                    var message = new AddNewPlaylistMessageModel()
                    {
                        NewPlaylistName = param
                    };
                    Messenger.Default.Send(message);
                    DialogResult = true;
                }, param => param?.Length > 1);
            }
        }

        #endregion
    }
}
