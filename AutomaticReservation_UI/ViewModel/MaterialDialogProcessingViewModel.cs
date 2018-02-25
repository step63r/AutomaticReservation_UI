using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;

namespace AutomaticReservation_UI.ViewModel
{
    public class MaterialDialogProcessingViewModel : ViewModelBase
    {
        #region コマンド・プロパティ
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }
        private Action _executableAction;
        public Action ExecutableAction
        {
            get
            {
                return _executableAction;
            }
            set
            {
                _executableAction = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public void Loaded()
        {
            ExecutableAction();
        }
    }
}
