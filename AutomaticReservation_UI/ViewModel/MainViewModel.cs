using AutomaticReservation_UI.Common;
using AutomaticReservation_UI.Model;
using AutomaticReservation_UI.ToyokoInn;
using AutomaticReservation_UI.UserControls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticReservation_UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IHasDialog
    {
        #region コマンド・プロパティ
        private readonly IDataService _dataService;

        private RelayCommand _btnExecute;
        /// <summary>
        /// 予約実行コマンド
        /// </summary>
        public RelayCommand BtnExecute
        {
            get
            {
                if (_btnExecute == null)
                {
                    _btnExecute = new RelayCommand(Execute, CanExecute);
                }
                return _btnExecute;
            }
        }

        private RelayCommand _btnConfigure;
        /// <summary>
        /// 予約設定コマンド
        /// </summary>
        public RelayCommand BtnConfigure
        {
            get
            {
                if (_btnConfigure == null)
                {
                    _btnConfigure = new RelayCommand(ExecuteConfigure, CanExecuteConfigure);
                }
                return _btnConfigure;
            }
        }

        private RelayCommand _btnHotelUpdate;
        /// <summary>
        /// ホテル情報更新コマンド
        /// </summary>
        public RelayCommand BtnHotelUpdate
        {
            get
            {
                if (_btnHotelUpdate == null)
                {
                    _btnHotelUpdate = new RelayCommand(ExecuteHotelUpdate, CanExecuteHotelUpdate);
                }
                return _btnHotelUpdate;
            }
        }

        private RelayCommand _btnShowLicense;
        /// <summary>
        /// ライセンス表示コマンド
        /// </summary>
        public RelayCommand BtnShowLicense
        {
            get
            {
                if (_btnShowLicense == null)
                {
                    _btnShowLicense = new RelayCommand(ExecuteShowLicense, CanExecuteShowLicense);
                }
                return _btnShowLicense;
            }
        }

        #region 検索条件
        private ObservableCollection<PrefCode> _colPrefCode;
        /// <summary>
        /// 都道府県コードコレクション
        /// </summary>
        public ObservableCollection<PrefCode> ColPrefCode
        {
            get
            {
                return _colPrefCode;
            }
            set
            {
                _colPrefCode = value;
                RaisePropertyChanged();
            }
        }

        private PrefCode _selectedPrefCode;
        /// <summary>
        /// 選択された都道府県コード
        /// </summary>
        public PrefCode SelectedPrefCode
        {
            get
            {
                return _selectedPrefCode;
            }
            set
            {
                _selectedPrefCode = value;
                RaisePropertyChanged();

                // ホテルの表示内容を変更する
                ColLimitedHotel = new ObservableCollection<Hotel>(ColHotel.Where(item => item.PrefCode.Equals(value.ID)));
            }
        }

        private ObservableCollection<Hotel> _colHotel;
        /// <summary>
        /// ホテルコレクション
        /// </summary>
        public ObservableCollection<Hotel> ColHotel
        {
            get
            {
                return _colHotel;
            }
            set
            {
                _colHotel = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Hotel> _colLimitedHotel;
        /// <summary>
        /// 表示用のホテルコレクション
        /// </summary>
        public ObservableCollection<Hotel> ColLimitedHotel
        {
            get
            {
                return _colLimitedHotel;
            }
            set
            {
                _colLimitedHotel = value;
                RaisePropertyChanged();
            }
        }

        private Hotel _selectedHotel;
        /// <summary>
        /// 選択されたホテル
        /// </summary>
        public Hotel SelectedHotel
        {
            get
            {
                return _selectedHotel;
            }
            set
            {
                _selectedHotel = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _checkinDate;
        /// <summary>
        /// チェックイン日付
        /// </summary>
        public DateTime CheckinDate
        {
            get
            {
                return _checkinDate;
            }
            set
            {
                _checkinDate = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<RoomType> _colRoomType;
        /// <summary>
        /// 部屋タイプコレクション
        /// </summary>
        public ObservableCollection<RoomType> ColRoomType
        {
            get
            {
                return _colRoomType;
            }
            set
            {
                _colRoomType = value;
                RaisePropertyChanged();
            }
        }

        private RoomType _selectedRoomType;
        /// <summary>
        /// 選択された部屋タイプ
        /// </summary>
        public RoomType SelectedRoomType
        {
            get
            {
                return _selectedRoomType;
            }
            set
            {
                _selectedRoomType = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<CheckinTime> _colCheckinTime;
        /// <summary>
        /// チェックイン予定時刻コレクション
        /// </summary>
        public ObservableCollection<CheckinTime> ColCheckinTime
        {
            get
            {
                return _colCheckinTime;
            }
            set
            {
                _colCheckinTime = value;
                RaisePropertyChanged();
            }
        }

        private CheckinTime _selectedCheckinTime;
        /// <summary>
        /// 選択されたチェックイン予定時刻
        /// </summary>
        public CheckinTime SelectedCheckinTime
        {
            get
            {
                return _selectedCheckinTime;
            }
            set
            {
                _selectedCheckinTime = value;
                RaisePropertyChanged();
            }
        }

        private bool _chkNoSmoking;
        /// <summary>
        /// 禁煙を検索する
        /// </summary>
        public bool ChkNoSmoking
        {
            get
            {
                return _chkNoSmoking;
            }
            set
            {
                _chkNoSmoking = value;
                RaisePropertyChanged();
            }
        }

        private bool _chkSmoking;
        /// <summary>
        /// 喫煙を検索する
        /// </summary>
        public bool ChkSmoking
        {
            get
            {
                return _chkSmoking;
            }
            set
            {
                _chkSmoking = value;
                RaisePropertyChanged();
            }
        }

        private bool _isSmokingFirst;
        /// <summary>
        /// 喫煙を優先する
        /// </summary>
        public bool IsSmokingFirst
        {
            get
            {
                return _isSmokingFirst;
            }
            set
            {
                _isSmokingFirst = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Dialog系
        private DialogType _dType;
        /// <summary>
        /// 開いているダイアログ種別
        /// </summary>
        public DialogType DType
        {
            get
            {
                return _dType;
            }
            set
            {
                _dType = value;
                RaisePropertyChanged();
            }
        }

        private string _message;
        /// <summary>
        /// ダイアログメッセージ
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

        private object _dialogView;
        /// <summary>
        /// アクティブなDialogView
        /// </summary>
        public object DialogView
        {
            get
            {
                return _dialogView;
            }
            set
            {
                _dialogView = value;
                RaisePropertyChanged();
            }
        }

        private bool _isDialogOpen;
        /// <summary>
        /// Dialogが表示中かどうか
        /// </summary>
        public bool IsDialogOpen
        {
            get
            {
                return _isDialogOpen;
            }
            set
            {
                _isDialogOpen = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Config系
        private LoginInfo _currentLoginInfo;
        /// <summary>
        /// 現在のアカウント設定
        /// </summary>
        public LoginInfo CurrentLoginInfo
        {
            get
            {
                return _currentLoginInfo;
            }
            set
            {
                _currentLoginInfo = value;
                RaisePropertyChanged();
            }
        }
        private ScrConfig _currentScrConfig;
        /// <summary>
        /// 現在のスクリーンショット設定
        /// </summary>
        public ScrConfig CurrentScrConfig
        {
            get
            {
                return _currentScrConfig;
            }
            set
            {
                _currentScrConfig = value;
                RaisePropertyChanged();
            }
        }
        private string _currentAesPass;
        /// <summary>
        /// 暗号化された現在のパスワード（UserControlにバインド）
        /// </summary>
        public string CurrentAesPass
        {
            get
            {
                return _currentAesPass;
            }
            set
            {
                _currentAesPass = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public ObservableCollection<ReservationControlViewModel> ReservationList { get; set; } = new ObservableCollection<ReservationControlViewModel>();
        private ReservationControlViewModel _reservationViewModel;
        public ReservationControlViewModel ReservationViewModel
        {
            get
            {
                return _reservationViewModel;
            }
            set
            {
                _reservationViewModel = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });
            CheckinDate = DateTime.Now.AddDays(1);
            ChkNoSmoking = true;
            ChkSmoking = false;

            var displayTuple = Load();
            ColPrefCode = displayTuple.Item1;
            ColHotel = displayTuple.Item2;
            ColRoomType = displayTuple.Item3;
            ColCheckinTime = displayTuple.Item4;
        }

        /// <summary>
        /// コマンドを実行
        /// </summary>
        public void Execute()
        {
            // 検索クラス作成
            var finder = new ProcessFormat()
            {
                HotelID = new Hotel()
                {
                    HotelID = SelectedHotel.HotelID,
                    HotelName = SelectedHotel.HotelName,
                    PrefCode = SelectedHotel.PrefCode
                },
                CheckinDate = CheckinDate,
                Type = SelectedRoomType,
                CheckinValue = SelectedCheckinTime,
                EnableNoSmoking = ChkNoSmoking,
                EnableSmoking = ChkSmoking,
                SmokingFirst = IsSmokingFirst
            };
            var item = new ReservationControlViewModel(finder);
            ReservationList.Add(item);
        }
        /// <summary>
        /// コマンドが実行可能かどうかを判定
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            return !(SelectedPrefCode is null) && !(SelectedHotel is null) && !(!ChkNoSmoking && !ChkSmoking) && !(SelectedRoomType is null) && !(SelectedCheckinTime is null);
        }

        public void ExecuteConfigure()
        {
            // データ読み出し
            var configTuple = LoadConfig();
            CurrentScrConfig = configTuple.Item1;
            CurrentLoginInfo = configTuple.Item2;
            try
            {
                CurrentAesPass = AesEncrypt.DecryptFromBase64(configTuple.Item2.LoginPass, AesKeyConf.key, AesKeyConf.iv);
            }
            catch
            {
                //CurrentAesPass = new SecureString();
            }

            DType = DialogType.Configure;
            DialogView = new UserSettingsDialog()
            {
                DataContext = this
            };
            IsDialogOpen = true;
        }
        public bool CanExecuteConfigure()
        {
            return true;
        }

        // Dialogに表示するViewModelを生成
        public void ExecuteHotelUpdate()
        {
            Message = "公式ホームページから最新のホテル一覧を自動で取得します。\r\n回線速度によっては数十分かかることがあります。\r\n\r\n※取得した情報は次回起動時より有効になります。\r\n";
            DType = DialogType.HotelUpdate;
            DialogView = new MaterialDialogOkCancel()
            {
                DataContext = this
            };
            IsDialogOpen = true;
        }
        public bool CanExecuteHotelUpdate()
        {
            return true;
        }

        public void ExecuteShowLicense()
        {
            using (var fs = new StreamReader(String.Format(@"{0}\License.txt", CommonPath.CommonDir), Encoding.GetEncoding("utf-8")))
            {
                Message = fs.ReadToEnd();
            }
            DType = DialogType.License;
            DialogView = new MaterialDialogOkScroll()
            {
                DataContext = this
            };
            IsDialogOpen = true;
        }
        public bool CanExecuteShowLicense()
        {
            return true;
        }

        public async void AcceptDialog()
        {
            // ダイアログ種別で分岐
            switch (DType)
            {
                case DialogType.Configure:
                    // 設定を保存
                    XmlConverter.Serialize(CurrentScrConfig, String.Format(@"{0}\ScrConfig.xml", SiteConfig.BASE_DIR));
                    CurrentLoginInfo.LoginPass = AesEncrypt.EncryptToBase64(CurrentAesPass, AesKeyConf.key, AesKeyConf.iv);
                    XmlConverter.Serialize(CurrentLoginInfo, String.Format(@"{0}\LoginInfo.xml", SiteConfig.BASE_DIR));

                    // ダイアログを閉じる
                    IsDialogOpen = false;
                    DialogView = null;
                    break;

                case DialogType.HotelUpdate:
                    // Viewの型で更に分岐
                    switch (DialogView)
                    {
                        case MaterialDialogOkCancel okCancelDialog:
                            // ダイアログを閉じる
                            IsDialogOpen = false;
                            DialogView = null;

                            // 次のダイアログを表示する
                            Message = "処理中...";
                            DialogView = new MaterialDialogProcessing()
                            {
                                DataContext = this
                            };
                            IsDialogOpen = true;

                            // 処理モデル定義
                            var model = new HotelUpdate();
                            await Task.Run(() =>
                            {
                                // 処理実行
                                model.Execute();
                            });

                            // 処理成功
                            if (model.Result)
                            {
                                // ダイアログを閉じる
                                IsDialogOpen = false;
                                DialogView = null;

                                // 次のダイアログを表示する
                                Message = "完了しました\r\n";
                                DialogView = new MaterialDialogOk()
                                {
                                    DataContext = this
                                };
                                IsDialogOpen = true;
                            }
                            break;

                        case MaterialDialogOk okDialog:
                            // ダイアログを閉じる
                            IsDialogOpen = false;
                            DialogView = null;
                            break;
                    }
                    break;

                case DialogType.License:
                    // ダイアログを閉じる
                    IsDialogOpen = false;
                    DialogView = null;
                    break;
            }
        }

        public void CancelDialog()
        {
            IsDialogOpen = false;
            DialogView = null;
        }

        /// <summary>
        /// データをXMLから読み込む
        /// </summary>
        /// <returns></returns>
        private Tuple<ObservableCollection<PrefCode>, ObservableCollection<Hotel>, ObservableCollection<RoomType>, ObservableCollection<CheckinTime>> Load()
        {
            var ret1 = new ObservableCollection<PrefCode>();
            try
            {
                // ファイルが存在する
                ret1 = CsvConverter.DeSerialize<PrefCode, PrefCodeMap>(String.Format(@"{0}\PrefCode.csv", CommonPath.CommonDir));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }

            var ret2 = new ObservableCollection<Hotel>();
            try
            {
                // ファイルが存在する
                ret2 = XmlConverter.DeSerializeToCol<Hotel>(String.Format(@"{0}\HotelList.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }

            var ret3 = new ObservableCollection<RoomType>();
            try
            {
                // ファイルが存在する
                ret3 = XmlConverter.DeSerializeToCol<RoomType>(String.Format(@"{0}\RoomType.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }

            var ret4 = new ObservableCollection<CheckinTime>();
            try
            {
                // ファイルが存在する
                ret4 = XmlConverter.DeSerializeToCol<CheckinTime>(String.Format(@"{0}\CheckinTime.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }

            return Tuple.Create(ret1, ret2, ret3, ret4);
        }

        /// <summary>
        /// 設定データをXMLから読み込む
        /// </summary>
        /// <returns></returns>
        private Tuple<ScrConfig, LoginInfo> LoadConfig()
        {
            var ret1 = new ScrConfig();
            try
            {
                // ファイルが存在する
                ret1 = XmlConverter.DeSerialize<ScrConfig>(String.Format(@"{0}\ScrConfig.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }

            var ret2 = new LoginInfo();
            try
            {
                // ファイルが存在する
                ret2 = XmlConverter.DeSerialize<LoginInfo>(String.Format(@"{0}\LoginInfo.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }

            return Tuple.Create(ret1, ret2);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}