using System;
using System.Xml.Serialization;
using System.ComponentModel;
using AutomaticReservation_UI.Common;

namespace AutomaticReservation_UI.ToyokoInn
{
    /// <summary>
    /// 予約データクラス
    /// </summary>
    public class ProcessFormat
    {
        // ホテル
        public Hotel HotelID { get; set; }
        // 宿泊日
        public DateTime CheckinDate { get; set; }
        // 部屋タイプ
        public RoomType Type { get; set; }
        // 禁煙ルームを検索する
        public bool EnableNoSmoking { get; set; }
        // 喫煙ルームを検索する
        public bool EnableSmoking { get; set; }
        // 喫煙ルームを優先して検索する
        public bool SmokingFirst { get; set; }
        // チェックイン予定時刻
        public CheckinTime CheckinValue { get; set; }
    }

    /// <summary>
    /// ホテルクラス
    /// </summary>
    [XmlRoot("Hotel")]
    public class Hotel : INotifyPropertyChanged
    {
        // イベントだけ実装しておく。OnPropertyChangedは使わない
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        private string _hotelID;
        /// <summary>
        /// ホテルID
        /// </summary>
        [XmlElement("HotelID")]
        public string HotelID
        {
            get { return _hotelID; }
            set
            {
                if (Equals(_hotelID, value))
                {
                    return;
                }
                _hotelID = value;
                PropertyChanged.Raise(() => HotelID);
            }
        }

        [XmlIgnore]
        private int _prefCode;
        /// <summary>
        /// 都道府県コード
        /// </summary>
        [XmlElement("PrefCode")]
        public int PrefCode
        {
            get { return _prefCode; }
            set
            {
                if (Equals(_prefCode, value))
                {
                    return;
                }
                _prefCode = value;
                PropertyChanged.Raise(() => PrefCode);
            }
        }

        [XmlIgnore]
        private string _hotelName;
        /// <summary>
        /// ホテル名
        /// </summary>
        [XmlElement("HotelName")]
        public string HotelName
        {
            get { return _hotelName; }
            set
            {
                if (Equals(_hotelName, value))
                {
                    return;
                }
                _hotelName = value;
                PropertyChanged.Raise(() => HotelName);
            }
        }
    }

    /// <summary>
    /// 部屋タイプクラス
    /// </summary>
    [XmlRoot("RoomType")]
    public class RoomType : INotifyPropertyChanged
    {
        // イベントだけ実装しておく。OnPropertyChangedは使わない
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        private int _roomTypeID;
        /// <summary>
        /// 部屋タイプID
        /// </summary>
        [XmlElement("RoomTypeID")]
        public int RoomTypeID
        {
            get { return _roomTypeID; }
            set
            {
                if (Equals(_roomTypeID, value))
                {
                    return;
                }
                _roomTypeID = value;
                PropertyChanged.Raise(() => RoomTypeID);
            }
        }

        [XmlIgnore]
        private string _roomTypeName;
        /// <summary>
        /// 部屋タイプ名
        /// </summary>
        [XmlElement("RoomTypeName")]
        public string RoomTypeName
        {
            get { return _roomTypeName; }
            set
            {
                if (Equals(_roomTypeName, value))
                {
                    return;
                }
                _roomTypeName = value;
                PropertyChanged.Raise(() => RoomTypeName);
            }
        }
    }

    /// <summary>
    /// チェックイン予定時刻クラス
    /// </summary>
    [XmlRoot("CheckinTime")]
    public class CheckinTime : INotifyPropertyChanged
    {
        // イベントだけ実装しておく。OnPropertyChangedは使わない
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        private string _checkinValue;
        /// <summary>
        /// チェックイン予定時刻の実値
        /// </summary>
        [XmlElement("CheckinValue")]
        public string CheckinValue
        {
            get { return _checkinValue; }
            set
            {
                if (Equals(_checkinValue, value))
                {
                    return;
                }
                _checkinValue = value;
                PropertyChanged.Raise(() => CheckinValue);
            }
        }

        [XmlIgnore]
        private string _checkinName;
        /// <summary>
        /// チェックイン予定時刻の表示名
        /// </summary>
        [XmlElement("CheckinName")]
        public string CheckinName
        {
            get { return _checkinName; }
            set
            {
                if (Equals(_checkinName, value))
                {
                    return;
                }
                _checkinName = value;
                PropertyChanged.Raise(() => CheckinName);
            }
        }
    }

    /// <summary>
    /// ログイン情報クラス
    /// </summary>
    [XmlRoot("LoginInfo")]
    public class LoginInfo : INotifyPropertyChanged
    {
        // イベントだけ実装しておく。OnPropertyChangedは使わない
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        private string _loginAddress;
        /// <summary>
        /// メールアドレス
        /// </summary>
        [XmlElement("LoginAddress")]
        public string LoginAddress
        {
            get { return _loginAddress; }
            set
            {
                if (Equals(_loginAddress, value))
                {
                    return;
                }
                _loginAddress = value;
                PropertyChanged.Raise(() => LoginAddress);
            }
        }

        [XmlIgnore]
        private string _loginPass;
        /// <summary>
        /// パスワード
        /// </summary>
        [XmlElement("LoginPass")]
        public string LoginPass
        {
            get { return _loginPass; }
            set
            {
                if (Equals(_loginPass, value))
                {
                    return;
                }
                _loginPass = value;
                PropertyChanged.Raise(() => LoginPass);
            }
        }

        [XmlIgnore]
        private string _loginTel;
        /// <summary>
        /// 電話番号
        /// </summary>
        [XmlElement("LoginTel")]
        public string LoginTel
        {
            get { return _loginTel; }
            set
            {
                if (Equals(_loginTel, value))
                {
                    return;
                }
                _loginTel = value;
                PropertyChanged.Raise(() => LoginTel);
            }
        }
    }

    /// <summary>
    /// スクリーンショット設定クラス
    /// </summary>
    [XmlRoot("ScrConfig")]
    public class ScrConfig : INotifyPropertyChanged
    {
        // イベントだけ実装しておく。OnPropertyChangedは使わない
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        private string _scrPath;
        /// <summary>
        /// 保存ディレクトリ
        /// </summary>
        [XmlElement("ScrPath")]
        public string ScrPath
        {
            get { return _scrPath; }
            set
            {
                if (Equals(_scrPath, value))
                {
                    return;
                }
                _scrPath = value;
                PropertyChanged.Raise(() => ScrPath);
            }
        }

        [XmlIgnore]
        private int _scrWidth;
        /// <summary>
        /// 画面サイズ 横（px）
        /// </summary>
        [XmlElement("ScrWidth")]
        public int ScrWidth
        {
            get { return _scrWidth; }
            set
            {
                if (Equals(_scrWidth, value))
                {
                    return;
                }
                _scrWidth = value;
                PropertyChanged.Raise(() => ScrWidth);
            }
        }

        [XmlIgnore]
        private int _scrHeight;
        /// <summary>
        /// 画面サイズ 縦（px）
        /// </summary>
        [XmlElement("ScrHeight")]
        public int ScrHeight
        {
            get { return _scrHeight; }
            set
            {
                if (Equals(_scrHeight, value))
                {
                    return;
                }
                _scrHeight = value;
                PropertyChanged.Raise(() => ScrHeight);
            }
        }

        [XmlIgnore]
        private int _maxFileCount;
        /// <summary>
        /// 最大ファイル保持数（超えた場合は古いものから削除される）
        /// </summary>
        [XmlElement("MaxFileCount")]
        public int MaxFileCount
        {
            get { return _maxFileCount; }
            set
            {
                if (Equals(_maxFileCount, value))
                {
                    return;
                }
                _maxFileCount = value;
                PropertyChanged.Raise(() => MaxFileCount);
            }
        }
    }
}