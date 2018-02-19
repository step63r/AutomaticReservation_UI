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
        #endregion
    }
}
