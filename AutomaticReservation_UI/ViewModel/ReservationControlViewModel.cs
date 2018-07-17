using AutomaticReservation_UI.Common;
using AutomaticReservation_UI.ToyokoInn;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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

        private bool _enableAutoRetry;
        /// <summary>
        /// 自動リトライアイコンを表示
        /// </summary>
        public bool EnableAutoRetry
        {
            get { return _enableAutoRetry; }
            set
            {
                _enableAutoRetry = value;
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

        private RelayCommand _btnClose;
        /// <summary>
        /// 閉じるボタン
        /// </summary>
        public RelayCommand BtnClose
        {
            get
            {
                if (_btnClose == null)
                {
                    _btnClose = new RelayCommand(CloseControl, CanCloseControl);
                }
                return _btnClose;
            }
        }

        private RelayCommand _cmdLoaded;
        /// <summary>
        /// UserControl初期化完了後のイベントコマンド
        /// </summary>
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

        private string _groupBoxToolTip;
        /// <summary>
        /// UserControlのツールチップ
        /// </summary>
        public string GroupBoxToolTip
        {
            get { return _groupBoxToolTip; }
            set
            {
                _groupBoxToolTip = value;
                RaisePropertyChanged();
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

        private bool _canExecCommand;
        /// <summary>
        /// ボタンが押下可能かどうかを格納
        /// </summary>
        public bool CanExecCommand
        {
            get { return _canExecCommand; }
            set
            {
                _canExecCommand = value;
                RaisePropertyChanged();
            }
        }

        private CancellationTokenSource _cancelTokenSource;
        /// <summary>
        /// 非同期処理をキャンセルするトークンソース
        /// </summary>
        public CancellationTokenSource CancelTokenSource
        {
            get { return _cancelTokenSource; }
            set
            {
                _cancelTokenSource = value;
                RaisePropertyChanged();
            }
        }

        private CancellationToken _cancelToken;
        /// <summary>
        /// 非同期処理をキャンセルするためのトークン
        /// </summary>
        public CancellationToken CancelToken
        {
            get { return _cancelToken; }
            set
            {
                _cancelToken = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _controlVisibility;
        /// <summary>
        /// UserControlのVisibility
        /// </summary>
        public Visibility ControlVisibility
        {
            get { return _controlVisibility; }
            set
            {
                _controlVisibility = value;
                RaisePropertyChanged();
            }
        }

        // スクリーンショット設定
        private ScrConfig _myScrConfig;
        // Modelに渡すスクリーンショット保存パス
        private string _scrShotPath;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="procFormat">予約データクラス</param>
        public ReservationControlViewModel(ProcessFormat procFormat)
        {
            FinderProcessFormat = procFormat;
            // ヘッダ（YYYY/MM/DD HotelName）
            Title = String.Format("{0} {1}", FinderProcessFormat.CheckinDate.ToShortDateString(), FinderProcessFormat.HotelID.HotelName);
            // ツールチップ
            GroupBoxToolTip = GetGroupBoxToolTipText();
            // スクリーンショット保存先パス
            _myScrConfig = Load();
            _scrShotPath = GetScrShotFilePath(_myScrConfig.ScrPath);
            // 自動リトライアイコン
            EnableAutoRetry = FinderProcessFormat.EnableAutoRetry;

            ColorMode = ColorZoneMode.PrimaryLight;
            IconMode = PackIconKind.Play;
            ProgressBarVisibility = Visibility.Visible;
            ControlVisibility = Visibility.Visible;
        }

        private string GetGroupBoxToolTipText()
        {
            string strSmoke;
            // 禁煙・喫煙の文字列生成
            if (FinderProcessFormat.EnableNoSmoking && FinderProcessFormat.EnableSmoking)
            {
                if (FinderProcessFormat.SmokingFirst)
                {
                    strSmoke = "喫煙 > 禁煙";
                }
                else
                {
                    strSmoke = "禁煙 > 喫煙";
                }
            }
            else if (FinderProcessFormat.EnableNoSmoking)
            {
                strSmoke = "禁煙";
            }
            else
            {
                strSmoke = "喫煙";
            }

            return String.Format("{0} {1} {2} {3} {4}", FinderProcessFormat.CheckinDate.ToShortDateString(), FinderProcessFormat.HotelID.HotelName, FinderProcessFormat.Type.RoomTypeName, FinderProcessFormat.CheckinValue.CheckinName, strSmoke);
        }

        public void Loaded()
        {
            // キャンセルトークン取得
            CancelTokenSource = new CancellationTokenSource();
            CancelToken = CancelTokenSource.Token;

            Task.Run(() =>
            {
                // クラスを作って
                var model = new Reservation()
                {
                    // オブジェクトを渡して
                    ProcFormat = FinderProcessFormat,
                    CancelToken = CancelToken,
                    ScreenShotPath = _scrShotPath
                };
                try
                {
                    model.PropertyChanged += OnListenerPropertyChanged;
                    CanExecCommand = true;
                    // 動かす
                    bool ret = model.Execute();
                    CanExecCommand = false;
                    model.PropertyChanged -= OnListenerPropertyChanged;
                    if (ret)
                    {
                        // 正常終了
                        ColorMode = ColorZoneMode.PrimaryDark;
                        IconMode = PackIconKind.CheckCircleOutline;
                        ProgressBarVisibility = Visibility.Hidden;
                    }
                    else
                    {
                        // 戻り値が false の場合も異常とみなす
                        ColorMode = ColorZoneMode.Accent;
                        IconMode = PackIconKind.AlertOutline;
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
            try
            {
                Process.Start(_scrShotPath);
            }
            catch
            {
                // 特に何もしない
            }
        }
        /// <summary>
        /// スクリーンショット表示コマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanShowScreenShot()
        {
            return CanExecCommand;
        }

        /// <summary>
        /// キャンセルコマンドを実行
        /// </summary>
        public void Cancel()
        {
            // 処理をキャンセル
            CancelTokenSource.Cancel();
        }
        /// <summary>
        /// キャンセルコマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanCancel()
        {
            return CanExecCommand;
        }

        /// <summary>
        /// 閉じるコマンドを実行
        /// </summary>
        public void CloseControl()
        {
            // 完全に閉じることは難しいのでVisibilityだけ変える
            ControlVisibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 閉じるコマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanCloseControl()
        {
            return !CanExecCommand;
        }

        /// <summary>
        /// スクリーンショットのファイルパスを取得する
        /// </summary>
        /// <param name="dirPath">基底ディレクトリ</param>
        /// <returns></returns>
        private string GetScrShotFilePath(string dirPath)
        {
            // dirPath\現在日時_チェックイン日付_HotelID.png
            return String.Format(@"{0}\{1}_{2}_{3}.png", dirPath, DateTime.Now.ToString("yyyyMMddHHmmss"), FinderProcessFormat.CheckinDate.ToString("yyyyMMdd"), FinderProcessFormat.HotelID.HotelID);
        }

        /// <summary>
        /// プログレスバーの通知ハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// データをXMLから読み込む
        /// </summary>
        /// <returns></returns>
        private ScrConfig Load()
        {
            var ret = new ScrConfig();
            try
            {
                // ファイルが存在する
                ret = XmlConverter.DeSerialize<ScrConfig>(String.Format(@"{0}\ScrConfig.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                XmlConverter.Serialize(ret, String.Format(@"{0}\ScrConfig.xml", SiteConfig.BASE_DIR));
            }

            return ret;
        }
    }
}
