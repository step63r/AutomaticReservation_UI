using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AutomaticReservation_UI.Common
{
    /// <summary>
    /// CSVシリアライザ／デシリアライザ
    /// </summary>
    public static class CsvConverter
    {
        /// <summary>
        /// 任意のオブジェクトをcsvにシリアル化する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">オブジェクト</param>
        /// <param name="path">ファイルパス</param>
        /// <returns>成功したらtrue、失敗したらfalse</returns>
        public static bool Serialize<T>(ObservableCollection<T> obj, string path)
        {
            bool ret = true;
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var csvWriter = new CsvWriter(new StreamWriter(fs, Encoding.GetEncoding("shift_jis")));
                    csvWriter.WriteRecords(obj);

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// csvファイルを任意のオブジェクトに逆シリアル化する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <typeparam name="M">クラスマップ型</typeparam>
        /// <param name="path">ファイルパス</param>
        /// <returns>オブジェクト</returns>
        public static ObservableCollection<T> DeSerialize<T, M>(string path) where M : ClassMap<T>
        {
            var ret = default(ObservableCollection<T>);
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var csv = new CsvReader(new StreamReader(fs, Encoding.GetEncoding("shift_jis")));
                    csv.Configuration.RegisterClassMap<M>();
                    ret = new ObservableCollection<T>(csv.GetRecords<T>().ToList());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ret;
        }
    }
}
