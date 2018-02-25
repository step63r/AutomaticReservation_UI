using GalaSoft.MvvmLight;

namespace AutomaticReservation_UI.ViewModel
{
    public class MaterialDialogOkViewModel : ViewModelBase
    {
        #region コマンド・プロパティ
        private string _message;
        /// <summary>
        /// 通知テキスト
        /// </summary>
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
        #endregion
    }
}
