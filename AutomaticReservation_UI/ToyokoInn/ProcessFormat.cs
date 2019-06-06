using System;
using System.Xml.Serialization;
using System.ComponentModel;
using AutomaticReservation_UI.Common;
using System.Security;

namespace AutomaticReservation_UI.ToyokoInn
{    
    /// <summary>
    /// 予約データクラス
    /// </summary>
    [XmlRoot("ProcessFormat")]
    public class ProcessFormat
    {
        // ホテル
        [XmlElement("HotelID")]
        public Hotel HotelID { get; set; }
        // 宿泊日
        [XmlElement("CheckinDate")]
        public DateTime CheckinDate { get; set; }
        // 部屋タイプ
        [XmlElement("Type")]
        public RoomType Type { get; set; }
        // 禁煙ルームを検索する
        [XmlElement("EnableNoSmoking")]
        public bool EnableNoSmoking { get; set; }
        // 喫煙ルームを検索する
        [XmlElement("EnableSmoking")]
        public bool EnableSmoking { get; set; }
        // 喫煙ルームを優先して検索する
        [XmlElement("SmokingFirst")]
        public bool SmokingFirst { get; set; }
        // チェックイン予定時刻
        [XmlElement("CheckinValue")]
        public CheckinTime CheckinValue { get; set; }
        // エラー発生時、自動的にリトライする
        [XmlElement("EnableAutoRetry")]
        public bool EnableAutoRetry { get; set; }
        // 同一日で予約があった場合に上書きする
        [XmlElement("EnableOverwrite")]
        public bool EnableOverwrite { get; set; }
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
        /// パスワード（暗号化する）
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
    /// ログファイル設定クラス
    /// </summary>
    [XmlRoot("LogConfig")]
    public class LogConfig : INotifyPropertyChanged
    {
        // イベントだけ実装しておく。OnPropertyChangedは使わない
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        private int _maxLogCount;
        /// <summary>
        /// 最大ログファイル保持数（超えた場合は古いものから削除される
        /// </summary>
        [XmlElement("MaxLogCount")]
        public int MaxLogCount
        {
            get { return _maxLogCount; }
            set
            {
                if (Equals(_maxLogCount, value))
                {
                    return;
                }
                _maxLogCount = value;
                PropertyChanged.Raise(() => MaxLogCount);
            }
        }
    }
}