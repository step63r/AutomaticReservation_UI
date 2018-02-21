using AutomaticReservation_UI.Common;
using AutomaticReservation_UI.ToyokoInn;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private int _count;
        /// <summary>
        /// カウンタ
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged();
            }
        }

        private string _message;
        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
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

        private Visibility _progressBarVisibility;
        /// <summary>
        /// プログレスバーのVisibility
        /// </summary>
        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
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

        private RelayCommand _cmdLoaded;
        public RelayCommand CmdLoaded
        {
            get
            {
                if (_cmdLoaded == null)
                {
                    _cmdLoaded = new RelayCommand(Loaded, CanLoaded);
                }
                return _cmdLoaded;
            }
        }


        private ProcessFormat _finderProcessFormat;
        /// <summary>
        /// 検索条件
        /// </summary>
        public ProcessFormat FinderProcessFormat
        {
            get { return _finderProcessFormat; }
            set
            {
                _finderProcessFormat = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="procFormat">予約データクラス</param>
        public ReservationControlViewModel(ProcessFormat procFormat)
        {
            FinderProcessFormat = procFormat;
            Title = String.Format("{0} {1}", FinderProcessFormat.CheckinDate.ToShortDateString(), FinderProcessFormat.HotelID.HotelName);
            ColorMode = ColorZoneMode.PrimaryLight;
            IconMode = PackIconKind.Play;
            ProgressBarVisibility = Visibility.Visible;
        }

        public void Loaded()
        {
            Task.Run(() =>
            {
                // クラスを作って
                var model = new Reservation()
                {
                    // オブジェクトを渡して
                    ProcFormat = FinderProcessFormat
                };
                try
                {
                    model.PropertyChanged += OnListenerPropertyChanged;
                    // 動かす
                    bool ret = model.Execute();
                    model.PropertyChanged -= OnListenerPropertyChanged;
                    if (ret)
                    {
                        // 正常終了
                        ColorMode = ColorZoneMode.PrimaryDark;
                        IconMode = PackIconKind.CheckCircleOutline;
                        ProgressBarVisibility = Visibility.Hidden;
                    }
                }
                catch
                {
                    // 異常終了
                    ColorMode = ColorZoneMode.Accent;
                    IconMode = PackIconKind.AlertOutline;
                    ProgressBarVisibility = Visibility.Hidden;
                }
            });
        }
        public bool CanLoaded()
        {
            return true;
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
            // CancellationToken
            return true;
        }

        private void OnListenerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var obj = (IProgressBar)sender;
            switch (e.PropertyName)
            {
                case nameof(IProgressBar.Count):
                    Count = obj.Count;
                    break;
                case nameof(IProgressBar.Message):
                    Message = obj.Message;
                    break;
            }
        }
    }
}
