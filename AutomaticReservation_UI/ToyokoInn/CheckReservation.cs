using AutomaticReservation_UI.Common;
using OpenQA.Selenium;
using System;

namespace AutomaticReservation_UI.ToyokoInn
{
    /// <summary>
    /// 最新予約取得モデルクラス（東横INN）
    /// </summary>
    public class CheckReservation
    {
        // 予約アイテム
        public ReservationItem _reservationItem;
        // ログイン情報
        protected LoginInfo _loginInfo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CheckReservation()
        {
            _loginInfo = Load();
        }

        /// <summary>
        /// 最新予約取得を実行する
        /// </summary>
        /// <returns></returns>
        public CheckReservationResult Execute()
        {
            var ret = default(CheckReservationResult);

            // ドライバ初期化
            using (var driver = WebDriverFactory.CreateInstance(AppSettings.BrowserName.Chrome))
            {
                // アクセス
                driver.Url = String.Format("{0}mypage/reservation/?lcl_id=ja", SiteConfig.BASE_URL);

                // ログイン試行
                if (!TryLogin(driver))
                {
                    // ログイン失敗
                    ret = CheckReservationResult.LoginFailed;
                    _reservationItem = null;
                }
                else
                {
                    try
                    {
                        // ホテル名
                        string name = driver.FindElement(By.XPath(SiteConfig.XPATH_RESERVATION_NAME)).Text;
                        // チェックイン日時
                        string date = driver.FindElement(By.XPath(SiteConfig.XPATH_RESERVATION_DATE)).Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0];

                        // 予約あり
                        ret = CheckReservationResult.Found;
                        _reservationItem = new ReservationItem()
                        {
                            Name = name,
                            Date = date
                        };
                    }
                    catch (NoSuchElementException)
                    {
                        // 予約なし
                        ret = CheckReservationResult.NotFound;
                        _reservationItem = null;
                    }
                }
            }
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
            try
            {
                if (_loginInfo is null)
                {

                }
                else
                {
                    driver.FindElement(By.XPath(SiteConfig.XPATH_FORM_ADDRESS)).SendKeys(_loginInfo.LoginAddress);
                    driver.FindElement(By.XPath(SiteConfig.XPATH_PASS)).SendKeys(AesEncrypt.DecryptFromBase64(_loginInfo.LoginPass, AesKeyConf.key, AesKeyConf.iv));
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
            }
            catch (NoSuchElementException)
            {
                // ログイン不要
                ret = true;
            }
            catch (FormatException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// データをXMLから読み込む
        /// </summary>
        /// <returns></returns>
        private LoginInfo Load()
        {
            var ret = new LoginInfo();
            try
            {
                // ファイルが存在する
                ret = XmlConverter.DeSerialize<LoginInfo>(String.Format(@"{0}\LoginInfo.xml", SiteConfig.BASE_DIR));
            }
            catch
            {
                // ファイルが存在しない
                // raise exception
            }
            return ret;
        }
    }
}
