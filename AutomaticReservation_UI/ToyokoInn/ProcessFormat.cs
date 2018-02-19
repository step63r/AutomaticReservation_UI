using System;
using System.Xml.Serialization;
using System.ComponentModel;
using AutomaticReservation_UI.Common;

namespace AutomaticReservation_UI.ToyokoInn
{
    /// <summary>
    /// Pythonモジュールに引き渡すデータクラス
    /// </summary>
    public class ProcessFormat
    {
        // ホテルID
        public string HotelID { get; set; }
        // 宿泊日
        public DateTime CheckinDate { get; set; }
        // 部屋タイプ
        public int RoomType { get; set; }
        // 禁煙ルームを検索する
        public bool EnableNoSmoking { get; set; }
        // 喫煙ルームを検索する
        public bool EnableSmoking { get; set; }
        // 喫煙ルームを優先して検索する
        public bool SmokingFirst { get; set; }
        // チェックイン予定時刻
        public string CheckinValue { get; set; }
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
    public  class CheckinTime : INotifyPropertyChanged
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
}
