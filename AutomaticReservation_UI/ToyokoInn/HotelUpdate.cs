using AutomaticReservation_UI.Common;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomaticReservation_UI.ToyokoInn
{
    /// <summary>
    /// ホテル一覧取得モデルクラス（東横INN）
    /// </summary>
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
            try
            {
                // 都道府県コード一覧の読込
                PrefCodeList = CsvConverter.DeSerialize<PrefCode, PrefCodeMap>(String.Format(@"{0}\PrefCode.csv", CommonPath.CommonDir));
            }
            catch
            {
                // raise exception
            }
        }

        /// <summary>
        /// 実行する
        /// </summary>
        public void Execute()
        {
            // ドライバ初期化
            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                // 都道府県コード
                int pref_code;
                // アクセス
                driver.Url = String.Format("{0}hotel_list/?lcl_id=ja", SiteConfig.BASE_URL);

                // 都道府県イテレータ
                int iter_pref = 1;
                // 都道府県の繰り返し
                while (true)
                {
                    // 都道府県コード取得
                    try
                    {
                        // 都道府県名
                        string pref_name = driver.FindElement(By.XPath(SiteConfig.XPATH_PREFNAME.Replace("ITER_PREF", iter_pref.ToString()))).Text;

                        // 都道府県コード
                        pref_code = PrefCodeList.Where(item => item.PrefName.Equals(pref_name)).First().ID;
                    }
                    catch (NoSuchElementException)
                    {
                        // 要素がもうない
                        break;
                    }
                    catch
                    {
                        // 都道府県コードCSVにない場合は（海外）スキップ
                        continue;
                    }

                    // ホテルイテレータ
                    int iter_hotel = 1;
                    while (true)
                    {
                        // ホテル取得
                        try
                        {
                            // ホテルID
                            string hotel_id = driver.FindElement(By.XPath(SiteConfig.XPATH_HOTELID.Replace("ITER_PREF", iter_pref.ToString()).Replace("ITER_HOTEL", iter_hotel.ToString()))).Text;
                            // 0埋めして5桁にする
                            hotel_id = String.Concat("00", hotel_id);

                            // ホテル名
                            string hotel_name = driver.FindElement(By.XPath(SiteConfig.XPATH_HOTELNAME.Replace("ITER_PREF", iter_pref.ToString()).Replace("ITER_HOTEL", iter_hotel.ToString()))).Text;
                            // 「東横INN」があると長くなるので消す
                            hotel_name = hotel_name.Replace("東横INN", "");

                            var target = new Hotel()
                            {
                                HotelID = hotel_id,
                                HotelName = hotel_name,
                                PrefCode = pref_code
                            };

                            // 要素を追加
                            HotelList.Add(target);
                        }
                        catch (NoSuchElementException)
                        {
                            // 要素がもうない
                            break;
                        }
                        catch
                        {
                            // raise exception
                        }
                        iter_hotel++;
                    }
                    iter_pref++;
                }

                // csvファイルに出力
                CsvConverter.Serialize(HotelList, String.Format(@"{0}\HotelList.csv", SiteConfig.BASE_DIR));
            }
        }
    }
}
