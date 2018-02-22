using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomaticReservation_UI.Common;
using CsvHelper;

namespace AutomaticReservation_UI.ToyokoInn
{
    public class HotelUpdate
    {
        /// <summary>
        /// 都道府県コード一覧
        /// </summary>
        public ObservableCollection<PrefCode> PrefCodeList { get; set; }
        /// <summary>
        /// ホテル一覧
        /// </summary>
        public ObservableCollection<Hotel> HotelList { get; set; }

        public HotelUpdate()
        {
            PrefCodeList = Load();
        }

        /// <summary>
        /// 実行する
        /// </summary>
        public void Execute()
        {

        }

        /// <summary>
        /// データをCSVから読み込む
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<PrefCode> Load()
        {
            var ret = new ObservableCollection<PrefCode>();
            try
            {
                // ファイルが存在する
                var csv = new CsvReader(new StreamReader(CommonPath.CommonDir));
                csv.Configuration.RegisterClassMap<PrefCodeMap>();
            }
            catch
            {
                // raise exception
            }

            return ret;
        }
    }
}
