using AutomaticReservation_UI.Common;
using GalaSoft.MvvmLight;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace AutomaticReservation_UI.ToyokoInn
{
    // ホントはBindableBase的な名前のクラスを継承したい。。。
    public class Reservation : ViewModelBase, IProgressBar
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
        /// キャンセルトークン
        /// </summary>
        public CancellationToken CancelToken;
        /// <summary>
        /// スクリーンショット保存先パス
        /// </summary>
        public string ScreenShotPath;

        private int _count;
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
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 予約を実行する（キャンセルチェックが多すぎて頭悪い）
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            bool ret = false;

            Message = "ドライバ初期化中";
            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                try
                {
                    // 仮想画面サイズ設定
                    driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
                    // 無限に繰り返す
                    while (true)
                    {
                        #region アクセス～詳細ページ
                        Count += 1;
                        // アクセス
                        Message = "アクセス中";
                        driver.Url = SiteConfig.BASE_URL;
                        ScreenShot(driver);

                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }

                        // 「お気に入りリスト」をクリック
                        driver.FindElement(By.XPath(SiteConfig.XPATH_FAVORITE)).Click();
                        ScreenShot(driver);

                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }

                        Message = "ログイン中";
                        // ログイン処理
                        try
                        {
                            driver.FindElement(By.XPath(SiteConfig.XPATH_FORM_ADDRESS)).SendKeys(LOGIN_ADDRESS);
                            driver.FindElement(By.XPath(SiteConfig.XPATH_PASS)).SendKeys(LOGIN_PASS);
                            // element.Submit()でもいいかも
                            driver.FindElement(By.XPath(SiteConfig.XPATH_LOGINBTN)).Click();
                            ScreenShot(driver);

                            // ログイン失敗
                            if (driver.Url.Equals(String.Format("{0}login", SiteConfig.BASE_URL)))
                            {
                                Message = "ログインに失敗";
                                break;
                            }
                        }
                        catch (NoSuchElementException)
                        {
                            // ログイン済み
                        }
                        catch (Exception ex)
                        {
                            // [TODO] raise exception
                            Message = ex.ToString();
                            break;
                        }
                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }

                        // 詳細ページへ
                        Message = "詳細ページへ移動中";
                        driver.Url = String.Format("{0}search/detail//{1}", SiteConfig.BASE_URL, ProcFormat.HotelID.HotelID);
                        ScreenShot(driver);
                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }

                        // 予約ページへ
                        Message = "予約ページへ移動中";
                        driver.Url = String.Format("{0}search/reserve/room?chckn_date={1}&room_type={2}", SiteConfig.BASE_URL, ProcFormat.CheckinDate.ToShortDateString(), ProcFormat.Type.RoomTypeID.ToString());
                        ScreenShot(driver);
                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }
                        #endregion

                        #region 禁煙・喫煙による分岐
                        // 20180222　禁煙・喫煙の該当する部屋タイプを全て検索するよう変更
                        if (!ProcFormat.EnableNoSmoking && !ProcFormat.EnableSmoking)
                        {
                            // 禁煙・喫煙両方0だった場合、エラー
                            // UIで阻止済
                        }
                        else if (ProcFormat.EnableNoSmoking && ProcFormat.EnableSmoking)
                        {
                            // 禁煙・喫煙両方1だった場合
                            if (!ProcFormat.SmokingFirst)
                            {
                                // 禁煙を優先
                                ret = SearchRoom(driver, "禁煙");
                                if (ret)
                                {
                                    Message = "禁煙　空室あり";
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, "喫煙");
                                    if (ret)
                                    {
                                        Message = "喫煙　空室あり";
                                    }
                                    else
                                    {
                                        Message = "満室";
                                    }
                                }
                            }
                            else
                            {
                                // 喫煙を優先
                                ret = SearchRoom(driver, "喫煙");
                                if (ret)
                                {
                                    Message = "喫煙　空室あり";
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, "禁煙");
                                    if (ret)
                                    {
                                        Message = "禁煙　空室あり";
                                    }
                                    else
                                    {
                                        Message = "満室";
                                    }
                                }
                            }
                        }
                        else if (ProcFormat.EnableNoSmoking)
                        {
                            // 禁煙
                            ret = SearchRoom(driver, "禁煙");
                            if (ret)
                            {
                                Message = "禁煙　空室あり";
                            }
                            else
                            {
                                Message = "禁煙　満室";
                            }
                        }
                        else
                        {
                            // 喫煙
                            ret = SearchRoom(driver, "喫煙");
                            if (ret)
                            {
                                Message = "喫煙　空室あり";
                            }
                            else
                            {
                                Message = "喫煙　満室";
                            }
                        }
                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }
                        #endregion

                        #region 空室発見～予約確定
                        if (ret)
                        {
                            Message = "予約中";
                            // 電話番号入力
                            driver.FindElement(By.XPath(SiteConfig.XPATH_TEL)).SendKeys(LOGIN_TEL);
                            // チェックイン予定時刻
                            var chktime_element = driver.FindElement(By.XPath(SiteConfig.XPATH_CHKINTIME));
                            var chktime_select_element = new SelectElement(chktime_element);
                            chktime_select_element.SelectByValue(ProcFormat.CheckinValue.CheckinValue);
                            // 確認ボタン押下
                            driver.FindElement(By.XPath(SiteConfig.XPATH_CONFIRM)).Click();
                            ScreenShot(driver);
                            // 確定ボタン押下
                            driver.FindElement(By.XPath(SiteConfig.XPATH_OK)).Click();
                            ScreenShot(driver);

                            // 正しく予約できたことを確認
                            try
                            {
                                string str_chk = driver.FindElement(By.XPath(SiteConfig.XPATH_CHK_VALIDATE)).Text;

                                if (!(str_chk.Equals(SiteConfig.STR_VALIDATE)))
                                {
                                    // 要素はあるが文字が違う
                                    Message = "予約できませんでした";
                                    ret = false;
                                }
                            }
                            catch (NoSuchElementException)
                            {
                                // 要素がない
                                Message = "予約できませんでした";
                                ret = false;
                            }

                            if (ret)
                            {
                                Message = "予約完了";
                                break;
                            }
                        }
                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }

                        // 指定秒待つ
                        Message = "スレッド待機中...";
                        Thread.Sleep(SiteConfig.TIME_SLEEP);
                        if (CheckCancel())
                        {
                            Message = "キャンセルされました";
                            break;
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    // [TODO] raise exception
                    Message = ex.ToString();
                }
                finally
                {
                    // ドライバを閉じる
                    // usingを使用しているので不要なはず
                    //driver.Quit();
                }
            }

            return ret;
        }

        /// <summary>
        /// キャンセルチェック
        /// </summary>
        /// <returns>キャンセルされた場合true</returns>
        private bool CheckCancel()
        {
            bool ret = false;
            if (CancelToken.IsCancellationRequested)
            {
                Message = "キャンセルされました";
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// スクリーンショットを保存する
        /// </summary>
        private void ScreenShot(IWebDriver driver)
        {
            var ss = ((ITakesScreenshot)driver).GetScreenshot();
            ss.SaveAsFile(ScreenShotPath, ScreenshotImageFormat.Png);
        }

        /// <summary>
        /// 空き部屋を検索する
        /// </summary>
        /// <param name="driver">WebDriverオブジェクト</param>
        /// <param name="match">禁煙、もしくは喫煙</param>
        /// <returns></returns>
        private bool SearchRoom(IWebDriver driver, string match)
        {
            bool ret = false;

            int i = 1;

            while (true)
            {
                try
                {
                    // 禁煙・喫煙のラベルを取得
                    string element = driver.FindElement(By.XPath(SiteConfig.XPATH_SMOKELABEL.Replace("INTEGER", i.ToString()))).Text;
                    if (element.Equals(match))
                    {
                        try
                        {
                            driver.FindElement(By.XPath(SiteConfig.XPATH_RESERVEBTN.Replace("INTEGER", i.ToString()))).Click();
                            ScreenShot(driver);
                            // 予約ボタンが押下可能な状態だったらループを抜ける
                            ret = true;
                            break;
                        }
                        catch (NoSuchElementException)
                        {
                            // カウンタを回して続行
                            i++;
                            continue;
                        }
                    }
                    else
                    {
                        // カウンタを回して続行
                        i++;
                        continue;
                    }
                }
                catch (NoSuchElementException)
                {
                    // 要素がもうない
                    break;
                }
            }

            return ret;
        }
    }
}
