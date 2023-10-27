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
        // ログファイル設定
        public LogConfig _logConfig;
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
            _logConfig = configTuple.Item1;
            _loginInfo = configTuple.Item2;
        }

        /// <summary>
        /// 予約を実行する（キャンセルチェックが多すぎて頭悪い）
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            bool ret = false;
            log.Debug(String.Format("処理を開始しました"));
            log.Debug(String.Format("> ホテルID　　：{0}", ProcFormat.HotelID.HotelID));
            log.Debug(String.Format("> チェックイン：{0}", ProcFormat.CheckinDate.ToString("yyyy/MM/dd")));

            // ログのファイル数を管理
            FileManager.RemoveFileObsolete(String.Format(@"{0}\AutomaticReservation_UI\log", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), "log", _logConfig.MaxLogCount);

            log.Debug("ドライバ初期化中");
            Message = "ドライバ初期化中";
            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                // 仮想画面サイズ設定
                driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

                #region 初期処理
                // アクセス
                Message = "アクセス中";
                log.Debug(String.Format("アクセス中：{0}", SiteConfig.BASE_URL));
                driver.Url = SiteConfig.BASE_URL;

                if (CheckCancel())
                {
                    log.Debug("キャンセルされました");
                    Message = "キャンセルされました";
                    return ret;
                }

                // 詳細ページへ
                log.Debug(String.Format("詳細ページへ移動中：{0}", String.Format("{0}search/detail//{1}", SiteConfig.BASE_URL, ProcFormat.HotelID.HotelID)));
                Message = "詳細ページへ移動中";
                driver.Url = String.Format("{0}search/detail//{1}", SiteConfig.BASE_URL, ProcFormat.HotelID.HotelID);
                if (CheckCancel())
                {
                    log.Debug("キャンセルされました");
                    Message = "キャンセルされました";
                    return ret;
                }
                #endregion

                #region 無限ループ
                while (true)
                {
                    try
                    {
                        Count += 1;
                        log.Debug("--------------------------------------------------");
                        log.Debug(String.Format("継続回数：{0}", Count));

                        // 予約ページへ
                        log.Debug(String.Format("予約ページへ移動中：{0}", String.Format("{0}search/reserve/room?chckn_date={1}&room_type={2}", SiteConfig.BASE_URL, ProcFormat.CheckinDate.ToShortDateString(), ProcFormat.Type.RoomTypeID.ToString())));
                        Message = "予約ページへ移動中";
                        driver.Url = String.Format("{0}search/reserve/room?chckn_date={1}&room_type={2}", SiteConfig.BASE_URL, ProcFormat.CheckinDate.ToShortDateString(), ProcFormat.Type.RoomTypeID.ToString());
                        if (CheckCancel())
                        {
                            log.Debug("キャンセルされました");
                            Message = "キャンセルされました";
                            break;
                        }

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
                                ret = SearchRoom(driver, "禁煙", ProcFormat.StrictRoomType, ProcFormat.Type.RoomTypeName);
                                if (ret)
                                {
                                    log.Debug("禁煙　空室あり");
                                    Message = "禁煙　空室あり";
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, "喫煙", ProcFormat.StrictRoomType, ProcFormat.Type.RoomTypeName);
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
                                ret = SearchRoom(driver, "喫煙", ProcFormat.StrictRoomType, ProcFormat.Type.RoomTypeName);
                                if (ret)
                                {
                                    log.Debug("喫煙　空室あり");
                                    Message = "喫煙　空室あり";
                                }
                                else
                                {
                                    // print 満室
                                    ret = SearchRoom(driver, "禁煙", ProcFormat.StrictRoomType, ProcFormat.Type.RoomTypeName);
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
                            ret = SearchRoom(driver, "禁煙", ProcFormat.StrictRoomType, ProcFormat.Type.RoomTypeName);
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
                            ret = SearchRoom(driver, "喫煙", ProcFormat.StrictRoomType, ProcFormat.Type.RoomTypeName);
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

                        // ログイン試行
                        if (!TryLogin(driver))
                        {
                            log.Fatal("ログインに失敗");
                            log.Info("このエラーは自動リトライできません");
                            Message = "ログインに失敗";
                            return ret;
                        }

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

                            // 同一日で予約があった場合
                            try
                            {
                                // Waitオブジェクトを定義
                                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 3));

                                // iframeに操作をスイッチ
                                wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.CssSelector(SiteConfig.IFRAME_OVERWRITE)));
                                log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_OVERWRITE));
                                if (SiteConfig.STR_OVERWRITE.Equals(driver.FindElement(By.XPath(SiteConfig.XPATH_OVERWRITE)).Text))
                                {
                                    if (ProcFormat.EnableOverwrite)
                                    {
                                        driver.FindElement(By.XPath(SiteConfig.XPATH_BTN_OVERWRITE)).Click();
                                    }
                                }
                                else
                                {
                                    // raise
                                }
                            }
                            catch (NoSuchElementException)
                            {
                                // 何もしない
                            }
                            catch (WebDriverTimeoutException)
                            {
                                // 何もしない
                            }

                            // 予約＆正常終了確認
                            try
                            {
                                

                                // 20180407　規約に同意チェック欄対応
                                // チェックボックスにチェック
                                log.Debug(string.Format("処理中：{0}", SiteConfig.XPATH_CHKAGREE));
                                driver.FindElement(By.XPath(SiteConfig.XPATH_CHKAGREE)).Click();

                                // 確定ボタン押下
                                log.Debug(string.Format("処理中：{0}", SiteConfig.XPATH_OK));
                                driver.FindElement(By.XPath(SiteConfig.XPATH_OK)).Click();

                                log.Debug(string.Format("処理中：{0}", SiteConfig.XPATH_CHK_VALIDATE));
                                string str_chk = driver.FindElement(By.XPath(SiteConfig.XPATH_CHK_VALIDATE)).Text;

                                if (!(str_chk.Equals(SiteConfig.STR_VALIDATE)))
                                {
                                    // 要素はあるが文字が違う
                                    log.Debug("予約できませんでした（文字列が異なります）");
                                    Message = "予約できませんでした";
                                    ret = false;
                                    if (ProcFormat.EnableAutoRetry)
                                    {
                                        log.Info("処理を続行します");
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (NoSuchElementException)
                            {
                                // 要素がない
                                log.Debug("予約できませんでした（要素が存在しません）");
                                Message = "予約できませんでした";
                                ret = false;
                                if (ProcFormat.EnableAutoRetry)
                                {
                                    log.Info("処理を続行します");
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (ret)
                            {
                                log.Debug("予約完了");
                                Message = "予約完了";
                                break;
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.ToString());
                        if (ProcFormat.EnableAutoRetry)
                        {                            
                            log.Info("処理を続行します");
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 適当な秒数待つ
                    int threadSleep = new Random().Next(SiteConfig.MIN_THREAD_SLEEP_MILLISEC, SiteConfig.MAX_THREAD_SLEEP_MILLISEC);
                    log.Debug(String.Format("スレッド処理を {0} 秒待機します", threadSleep / 1000));
                    Message = "スレッド待機中...";
                    // キャンセル可能なスレッド休止にした
                    if (CancelToken.WaitHandle.WaitOne(threadSleep))
                    {
                        log.Debug("キャンセルされました");
                        Message = "キャンセルされました";
                        break;
                    }
                }
                #endregion
            }
            log.Debug(String.Format("処理を終了しました（戻り値：{0}）", ret));
            return ret;
        }

        /// <summary>
        /// ログインを試みる
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        private bool TryLogin(IWebDriver driver)
        {
            bool ret = false;
            log.Debug("ログイン中");
            Message = "ログイン中";
            try
            {
                log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_FORM_ADDRESS));
                driver.FindElement(By.XPath(SiteConfig.XPATH_FORM_ADDRESS)).SendKeys(_loginInfo.LoginAddress);
                log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_PASS));
                driver.FindElement(By.XPath(SiteConfig.XPATH_PASS)).SendKeys(AesEncrypt.DecryptFromBase64(_loginInfo.LoginPass, AesKeyConf.key, AesKeyConf.iv));
                log.Debug(String.Format("処理中：{0}", SiteConfig.XPATH_LOGINBTN));
                driver.FindElement(By.XPath(SiteConfig.XPATH_LOGINBTN)).Click();

                // ログイン失敗
                if (driver.Url.Equals(String.Format("{0}login", SiteConfig.BASE_URL)))
                {
                    // ret = false;
                }
                else
                {
                    ret = true;
                }
            }
            catch (NoSuchElementException)
            {
                // ログイン不要
                ret = true;
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
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// データをXMLから読み込む
        /// </summary>
        /// <returns></returns>
        private Tuple<LogConfig, LoginInfo> Load()
        {
            var ret1 = new LogConfig();
            try
            {
                // ファイルが存在する
                ret1 = XmlConverter.DeSerialize<LogConfig>(String.Format(@"{0}\LogConfig.xml", SiteConfig.BASE_DIR));
                ret1 = ret1 ?? new LogConfig() { MaxLogCount = 100 };
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
        /// <param name="strictRoomType">厳密な部屋タイプのみに限定する</param>
        /// <param name="roomTypeName">ホテル名</param>
        /// <returns></returns>
        private bool SearchRoom(IWebDriver driver, string match, bool strictRoomType, string roomTypeName)
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
                            if (strictRoomType)
                            {
                                string roomName = driver.FindElement(By.XPath(SiteConfig.XPATH_ROOM_NAME.Replace("INTEGER", i.ToString()))).Text.Replace($"{match} ", "");
                                // 厳密な部屋タイプ名称と不一致であればカウンタを回して続行
                                if (!roomName.Equals(roomTypeName))
                                {
                                    i++;
                                    continue;
                                }
                            }

                            driver.FindElement(By.XPath(SiteConfig.XPATH_RESERVEBTN.Replace("INTEGER", i.ToString()))).Click();
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
