using AutomaticReservation_UI.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace AutomaticReservation_UI.ToyokoInn
{
    public class Reservation
    {
        // 後でどこかセーフティな場所に置く
        protected static string LOGIN_ADDRESS = "hoge@yahoo.co.jp";
        protected static string LOGIN_PASS = "password";
        protected static string LOGIN_TEL = "09012345678";

        /// <summary>
        /// 予約データクラス
        /// </summary>
        public ProcessFormat ProcFormat;

        /// <summary>
        /// 予約を実行する
        /// </summary>
        public bool Execute()
        {
            bool ret = true;

            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                try
                {
                    // 無限に繰り返す
                    while (true)
                    {
                        #region アクセス～詳細ページ
                        // アクセス
                        driver.Url = SiteConfig.BASE_URL;

                        // 「お気に入りリスト」をクリック
                        driver.FindElement(By.XPath(SiteConfig.XPATH_FAVORITE)).Click();

                        // ログイン処理
                        try
                        {
                            driver.FindElement(By.XPath(SiteConfig.XPATH_FORM_ADDRESS)).SendKeys(LOGIN_ADDRESS);
                            driver.FindElement(By.XPath(SiteConfig.XPATH_PASS)).SendKeys(LOGIN_PASS);
                            // element.Submit()でもいいかも
                            driver.FindElement(By.XPath(SiteConfig.XPATH_LOGINBTN)).Click();
                        }
                        catch (NoSuchElementException)
                        {
                            // ログイン済み
                        }
                        catch (Exception ex)
                        {
                            // [TODO] raise exception
                        }

                        // 詳細ページへ
                        driver.Url = String.Format("{0}search/reserve/room?chckn_date={1}&room_type={2}", SiteConfig.BASE_URL, ProcFormat.CheckinDate.ToShortDateString(), ProcFormat.Type.RoomTypeID.ToString());
                        #endregion

                        #region 禁煙・喫煙による分岐
                        if (!ProcFormat.EnableNoSmoking && !ProcFormat.EnableSmoking)
                        {
                            // 禁煙・喫煙両方0だった場合、エラー
                            // [TODO] Raise Exception
                            // 実行前に阻止したい
                        }
                        else if (ProcFormat.EnableNoSmoking && ProcFormat.EnableSmoking)
                        {
                            // 禁煙・喫煙両方1だった場合
                            if (!ProcFormat.SmokingFirst)
                            {
                                // 禁煙を優先
                                ret = SearchRoom(driver, SiteConfig.XPATH_NOSMOKING);
                                if (ret)
                                {
                                    // print 空室あり
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, SiteConfig.XPATH_SMOKING);
                                    if (ret)
                                    {
                                        // print 空室あり
                                    }
                                    else
                                    {
                                        // print 満室
                                    }
                                }
                            }
                            else
                            {
                                // 喫煙を優先
                                ret = SearchRoom(driver, SiteConfig.XPATH_SMOKING);
                                if (ret)
                                {
                                    // print 空室あり
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, SiteConfig.XPATH_NOSMOKING);
                                    if (ret)
                                    {
                                        // print 空室あり
                                    }
                                    else
                                    {
                                        // print 満室
                                    }
                                }
                            }
                        }
                        else if (ProcFormat.EnableNoSmoking)
                        {
                            // 禁煙
                            ret = SearchRoom(driver, SiteConfig.XPATH_NOSMOKING);
                            if (ret)
                            {
                                // print 空室あり
                            }
                            else
                            {
                                // print 満室
                            }
                        }
                        else
                        {
                            // 喫煙
                            ret = SearchRoom(driver, SiteConfig.XPATH_SMOKING);
                            if (ret)
                            {
                                // print 空室あり
                            }
                            else
                            {
                                // print 満室
                            }
                        }
                        #endregion

                        #region 空室発見～予約確定
                        if (ret)
                        {
                            // 電話番号入力
                            driver.FindElement(By.XPath(SiteConfig.XPATH_TEL)).SendKeys(LOGIN_TEL);
                            // チェックイン予定時刻
                            var chktime_element = driver.FindElement(By.XPath(SiteConfig.XPATH_CHKINTIME));
                            var chktime_select_element = new SelectElement(chktime_element);
                            chktime_select_element.SelectByValue(ProcFormat.CheckinValue.CheckinValue);
                            // 確認ボタン押下
                            driver.FindElement(By.XPath(SiteConfig.XPATH_CONFIRM)).Click();
                            // 確定ボタン押下
                            driver.FindElement(By.XPath(SiteConfig.XPATH_OK)).Click();

                            // 正しく予約できたことを確認
                            try
                            {
                                string str_chk = driver.FindElement(By.XPath(SiteConfig.XPATH_CHK_VALIDATE)).Text;

                                if (!(str_chk.Equals(SiteConfig.STR_VALIDATE)))
                                {
                                    // 要素はあるが文字が違う
                                    // print 空室を見つけましたが予約できませんでした
                                    ret = false;
                                }
                            }
                            catch (NoSuchElementException)
                            {
                                // 要素がない
                                // print 空室を見つけましたが予約できませんでした
                                ret = false;
                            }

                            if (ret)
                            {
                                // print 予約しました！
                                break;
                            }
                        }
                        #endregion
                    }

                }
                catch
                {
                    // [TODO] raise exception
                }
                finally
                {
                    // ドライバを閉じる
                    driver.Quit();
                }
            }

            return ret;
        }

        /// <summary>
        /// 空き部屋を検索する
        /// </summary>
        /// <param name="driver">WebDriverオブジェクト</param>
        /// <param name="xpath">検索XPath</param>
        /// <returns>空き部屋があった場合true</returns>
        private bool SearchRoom(IWebDriver driver, string xpath)
        {
            bool ret = true;

            try
            {
                driver.FindElement(By.XPath(xpath)).Click();
            }
            catch (NoSuchElementException)
            {
                ret = false;
            }

            return ret;
        }
    }
}
