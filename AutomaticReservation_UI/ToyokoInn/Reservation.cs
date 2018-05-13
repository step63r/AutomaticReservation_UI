using AutomaticReservation_UI.Common;
using GalaSoft.MvvmLight;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace AutomaticReservation_UI.ToyokoInn
{
    /// <summary>
    /// 予約モデルクラス（東横INN）
    /// </summary>
    public class Reservation : ViewModelBase, IProgressBar
    {
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
        // スクリーンショット設定
        public ScrConfig _scrConfig;
        // ログイン情報
        protected LoginInfo _loginInfo;

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

        // Loggerオブジェクト
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Reservation()
        {
            var configTuple = Load();
            _scrConfig = configTuple.Item1;
            _loginInfo = configTuple.Item2;
        }

        /// <summary>
        /// 予約を実行する（キャンセルチェックが多すぎて頭悪い）
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            log.Debug(String.Format("処理を開始しました"));
            log.Debug(String.Format("> ホテルID　　：{0}", ProcFormat.HotelID.HotelID));
            log.Debug(String.Format("> チェックイン：{0}", ProcFormat.CheckinDate.ToString("yyyy/MM/dd")));
            bool ret = false;

            // スクショのファイル数を管理
            FileManager.RemoveFileObsolete(_scrConfig.ScrPath, "png", _scrConfig.MaxFileCount);
            // ログのファイル数を管理
            FileManager.RemoveFileObsolete(String.Format(@"{0}\AutomaticReservation_UI\log", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "log", _scrConfig.MaxLogCount);

            log.Debug("ドライバ初期化中");
            Message = "ドライバ初期化中";
            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                try
                {
                    // 仮想画面サイズ設定
                    driver.Manage().Window.Size = new System.Drawing.Size(_scrConfig.ScrWidth, _scrConfig.ScrHeight);
                    // 無限に繰り返す
                    log.Debug("==================================================");
                    log.Debug("== ▼ 無限ループ処理                            ==");
                    log.Debug("==================================================");
                    while (true)
                    {
                        #region アクセス～詳細ページ
                        Count += 1;
                        log.Debug("--------------------------------------------------");
                        log.Debug(String.Format("継続回数：{0}", Count));
                        // アクセス
                        Message = "アクセス中";
                        log.Debug(String.Format("アクセス中：{0}", SiteConfig.BASE_URL));
                        driver.Url = SiteConfig.BASE_URL;
                        ScreenShot(driver);

                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }

                        // 「お気に入りリスト」をクリック
                        log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_FAVORITE));
                        driver.FindElement(By.XPath(SiteConfig.XPATH_FAVORITE)).Click();
                        ScreenShot(driver);

                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }

                        log.Debug("ログイン中");
                        Message = "ログイン中";
                        // ログイン処理
                        try
                        {
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_FORM_ADDRESS));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_FORM_ADDRESS)).SendKeys(_loginInfo.LoginAddress);
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_PASS));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_PASS)).SendKeys(AesEncrypt.DecryptFromBase64(_loginInfo.LoginPass, AesKeyConf.key, AesKeyConf.iv));
                            // element.Submit()でもいいかも
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_LOGINBTN));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_LOGINBTN)).Click();
                            ScreenShot(driver);

                            // ログイン失敗
                            if (driver.Url.Equals(String.Format("{0}login", SiteConfig.BASE_URL)))
                            {
                                log.Error("ログインに失敗");
                                Message = "ログインに失敗";
                                break;
                            }
                        }
                        catch (NoSuchElementException)
                        {
                            // ログイン済み
                            log.Debug("ログイン済み");
                        }
                        catch (Exception ex)
                        {
                            log.Error("エラーが発生しました", ex);
                            Message = ex.Message;
                            break;
                        }
                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }

                        // 詳細ページへ
                        log.Debug(String.Format("詳細ページへ移動中：{0}", String.Format("{0}search/detail//{1}", SiteConfig.BASE_URL, ProcFormat.HotelID.HotelID)));
                        Message = "詳細ページへ移動中";
                        driver.Url = String.Format("{0}search/detail//{1}", SiteConfig.BASE_URL, ProcFormat.HotelID.HotelID);
                        ScreenShot(driver);
                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }

                        // 予約ページへ
                        log.Debug(String.Format("予約ページへ移動中：{0}", String.Format("{0}search/reserve/room?chckn_date={1}&room_type={2}", SiteConfig.BASE_URL, ProcFormat.CheckinDate.ToShortDateString(), ProcFormat.Type.RoomTypeID.ToString())));
                        Message = "予約ページへ移動中";
                        driver.Url = String.Format("{0}search/reserve/room?chckn_date={1}&room_type={2}", SiteConfig.BASE_URL, ProcFormat.CheckinDate.ToShortDateString(), ProcFormat.Type.RoomTypeID.ToString());
                        ScreenShot(driver);
                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
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
                                    log.Debug("禁煙　空室あり");
                                    Message = "禁煙　空室あり";
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, "喫煙");
                                    if (ret)
                                    {
                                        log.Debug("喫煙　空室あり");
                                        Message = "喫煙　空室あり";
                                    }
                                    else
                                    {
                                        log.Debug("満室");
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
                                    log.Debug("喫煙　空室あり");
                                    Message = "喫煙　空室あり";
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, "禁煙");
                                    if (ret)
                                    {
                                        log.Debug("禁煙　空室あり");
                                        Message = "禁煙　空室あり";
                                    }
                                    else
                                    {
                                        log.Debug("満室");
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
                                log.Debug("禁煙　空室あり");
                                Message = "禁煙　空室あり";
                            }
                            else
                            {
                                log.Debug("禁煙　満室");
                                Message = "禁煙　満室";
                            }
                        }
                        else
                        {
                            // 喫煙
                            ret = SearchRoom(driver, "喫煙");
                            if (ret)
                            {
                                log.Debug("喫煙　空室あり");
                                Message = "喫煙　空室あり";
                            }
                            else
                            {
                                log.Debug("喫煙　満室");
                                Message = "喫煙　満室";
                            }
                        }
                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }
                        #endregion

                        #region 空室発見～予約確定
                        if (ret)
                        {
                            log.Debug("予約中");
                            Message = "予約中";
                            // 電話番号入力
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_TEL));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_TEL)).SendKeys(_loginInfo.LoginTel);
                            // チェックイン予定時刻
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_CHKINTIME));
                            var chktime_element = driver.FindElement(By.XPath(SiteConfig.XPATH_CHKINTIME));
                            var chktime_select_element = new SelectElement(chktime_element);
                            chktime_select_element.SelectByValue(ProcFormat.CheckinValue.CheckinValue);
                            // 確認ボタン押下
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_CONFIRM));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_CONFIRM)).Click();
                            ScreenShot(driver);
                            // 20180407　規約に同意チェック欄対応
                            // チェックボックスにチェック
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_CHKAGREE));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_CHKAGREE)).Click();
                            // 確定ボタン押下
                            log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_OK));
                            driver.FindElement(By.XPath(SiteConfig.XPATH_OK)).Click();
                            ScreenShot(driver);

                            // 正しく予約できたことを確認
                            try
                            {
                                log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_CHK_VALIDATE));
                                string str_chk = driver.FindElement(By.XPath(SiteConfig.XPATH_CHK_VALIDATE)).Text;

                                if (!(str_chk.Equals(SiteConfig.STR_VALIDATE)))
                                {
                                    // 要素はあるが文字が違う
                                    log.Debug("予約できませんでした（文字列が異なります）");
                                    Message = "予約できませんでした";
                                    ret = false;
                                }
                            }
                            catch (NoSuchElementException)
                            {
                                // 要素がない
                                log.Debug("予約できませんでした（要素が存在しません）");
                                Message = "予約できませんでした";
                                ret = false;
                            }

                            if (ret)
                            {
                                log.Debug("予約完了");
                                Message = "予約完了";
                                break;
                            }
                        }
                        // 指定秒待つ
                        log.Debug(String.Format("スレッド処理を {0} 秒待機します", SiteConfig.TIME_SLEEP / 1000));
                        Message = "スレッド待機中...";
                        // キャンセル可能なスレッド休止にした
                        if (CancelToken.WaitHandle.WaitOne(SiteConfig.TIME_SLEEP))
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }
                        //Thread.Sleep(SiteConfig.TIME_SLEEP);
                        //if (CheckCancel())
                        //{
                            //Message = "キャンセルされました";
                            //break;
                        //}
                        #endregion
                    }
                    log.Debug("==================================================");
                    log.Debug("== ▲ 無限ループ処理                            ==");
                    log.Debug("==================================================");
                }
                catch (Exception ex)
                {
                    log.Error("エラーが発生しました", ex);
                    Message = ex.Message;
                }
                finally
                {
                    // ドライバを閉じる
                    // usingを使用しているので不要なはず
                    //driver.Quit();
                }
            }

            log.Debug(String.Format("処理を終了しました（戻り値：{0}）", ret));
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
                log.Debug("キャンセルされました");
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
        /// データをXMLから読み込む
        /// </summary>
        /// <returns></returns>
        private Tuple<ScrConfig, LoginInfo> Load()
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
