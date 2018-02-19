using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticReservation_UI.ViewModel
{
    public class ReservationControlViewModel : ViewModelBase
    {
        #region コマンド・プロパティ
        private string _title;
        /// <summary>
        /// GroupBoxのヘッダ
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        private ColorZoneMode _colorMode;
        /// <summary>
        /// GroupBoxの色定義
        /// {PrimaryLight, PrimaryDark, Accent}
        /// </summary>
        public ColorZoneMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                _colorMode = value;
                RaisePropertyChanged();
            }
        }

        private PackIconKind _iconMode;
        /// <summary>
        /// GroupBoxのアイコン定義
        /// {Play, CheckCircleOutline, AlertOutline}
        /// </summary>
        public PackIconKind IconMode
        {
            get { return _iconMode; }
            set
            {
                _iconMode = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _btnShowScreenShot;
        /// <summary>
        /// スクリーンショット表示ボタン
        /// </summary>
        public RelayCommand BtnShowScreenShot
        {
            get
            {
                if (_btnShowScreenShot == null)
                {
                    _btnShowScreenShot = new RelayCommand(ShowScreenShot, CanShowScreenShot);
                }
                return _btnShowScreenShot;
            }
        }

        private RelayCommand _btnCancel;
        /// <summary>
        /// キャンセルボタン
        /// </summary>
        public RelayCommand BtnCancel
        {
            get
            {
                if (_btnCancel == null)
                {
                    _btnCancel = new RelayCommand(Cancel, CanCancel);
                }
                return _btnCancel;
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ（引数あり）
        /// </summary>
        public ReservationControlViewModel(DateTime date, string name)
        {
            Title = String.Format("{0} {1}", date.ToShortDateString(), name);
            ColorMode = ColorZoneMode.PrimaryLight;
            IconMode = PackIconKind.Play;
        }

        /// <summary>
        /// スクリーンショット表示コマンドを実行
        /// </summary>
        public void ShowScreenShot()
        {

        }
        /// <summary>
        /// スクリーンショット表示コマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanShowScreenShot()
        {
            return true;
        }

        /// <summary>
        /// キャンセルコマンドを実行
        /// </summary>
        public void Cancel()
        {

        }
        /// <summary>
        /// キャンセルコマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanCancel()
        {
            return true;
        }
    }
}
