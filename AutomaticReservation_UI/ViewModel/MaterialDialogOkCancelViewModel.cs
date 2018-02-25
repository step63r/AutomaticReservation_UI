using GalaSoft.MvvmLight;

namespace AutomaticReservation_UI.ViewModel
{
    /// <summary>
    /// OK / Cancel のボタンを持つ UserControl モデルクラス
    /// </summary>
    public class MaterialDialogOkCancelViewModel : ViewModelBase
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
