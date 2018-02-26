using AutomaticReservation_UI.Common;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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
        private ObservableCollection<PrefCode> PrefCodeList;
        /// <summary>
        /// ホテル一覧
        /// </summary>
        private ObservableCollection<Hotel> HotelList;
        /// <summary>
        ///  処理結果
        /// </summary>
        public bool Result = false;

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
            HotelList = new ObservableCollection<Hotel>();

            // ドライバ初期化
            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                // 都道府県コード
                int pref_code = 0;
                // その地域の都道府県リスト
                var pref_names = new List<string>();
                // アクセス
                driver.Url = String.Format("{0}hotel_list/?lcl_id=ja", SiteConfig.BASE_URL);

                // 地域で繰り返す（7地域）
                for (int iter_region = 1; iter_region <= 7; iter_region++)
                {
                    // ----- ----- ----- ----- -----
                    // 地域ごとに変数を初期化
                    // ----- ----- ----- ----- -----
                    pref_names = new List<string>();
                    int iter_pref = 1;
                    int iter_hotel = 1;

                    // その地域の都道府県一覧取得
                    while (true)
                    {
                        try
                        {
                            // 都道府県名
                            string pref_name = driver.FindElement(By.XPath(SiteConfig.XPATH_PREFNAME.Replace("ITER_REGION", iter_region.ToString()).Replace("ITER_PREF", iter_pref.ToString()))).Text;
                            // リストに追加
                            pref_names.Add(pref_name);
                            // 都道府県イテレータを回す
                            iter_pref++;
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
                    }

                    // その地域のホテル取得
                    while (true)
                    {
                        try
                        {
                            // ホテルID
                            string hotel_id = driver.FindElement(By.XPath(SiteConfig.XPATH_HOTELID.Replace("ITER_REGION", iter_region.ToString()).Replace("ITER_HOTEL", iter_hotel.ToString()))).Text;
                            // 0埋めして5桁にする
                            hotel_id = String.Concat("00", hotel_id);
                            
                            // ホテル名
                            string hotel_name = driver.FindElement(By.XPath(SiteConfig.XPATH_HOTELNAME.Replace("ITER_REGION", iter_region.ToString()).Replace("ITER_HOTEL", iter_hotel.ToString()))).Text;
                            // 「東横INN」があると長くなるので消す
                            hotel_name = hotel_name.Replace("東横INN", "");

                            // 住所
                            string hotel_address = driver.FindElement(By.XPath(SiteConfig.XPATH_ADDRESS.Replace("ITER_REGION", iter_region.ToString()).Replace("ITER_HOTEL", iter_hotel.ToString()))).Text;
                            bool is_oversea = false;
                            // 住所の先頭文字列を読んで都道府県を特定する
                            foreach(string one_pref in pref_names)
                            {
                                if (hotel_address.StartsWith(one_pref))
                                {
                                    try
                                    {
                                        // 都道府県コード設定
                                        pref_code = PrefCodeList.Where(item => item.PrefName.Equals(one_pref)).First().ID;
                                        break;
                                    }
                                    catch
                                    {
                                        // 海外フラグをオンにしてbreak
                                        is_oversea = true;
                                        break;
                                    }
                                }
                            }

                            // 海外の時はcontinue
                            if (is_oversea)
                            {
                                iter_hotel++;
                                continue;
                            }

                            // 要素を追加
                            HotelList.Add(new Hotel()
                            {
                                HotelID = hotel_id,
                                HotelName = hotel_name,
                                PrefCode = pref_code
                            });
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
                }

                // csvファイルに出力
                XmlConverter.SerializeFromCol(HotelList, String.Format(@"{0}\HotelList.xml", SiteConfig.BASE_DIR));

                // 処理成功
                Result = true;
            }
        }
    }
}
