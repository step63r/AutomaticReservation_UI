using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;

namespace AutomaticReservation_UI.Common
{
    /// <summary>
    /// 各ブラウザーのウェブドライバーを作成するクラス
    /// </summary>
    internal class WebDriverFactory
    {
        public static IWebDriver CreateInstance(AppSettings.BrowserName browserName)
        {
            switch (browserName)
            {
                case AppSettings.BrowserName.None:
                    throw new ArgumentException(string.Format("Not Definition. BrowserName:{0}", browserName));

                case AppSettings.BrowserName.Chrome:
                    // コマンドプロンプトを非表示にする
                    var chromeDriverService = ChromeDriverService.CreateDefaultService();
                    chromeDriverService.HideCommandPromptWindow = true;
                    // ブラウザを非表示にする
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArguments(new List<string>() { "headless", "lang=ja" });
                    return new ChromeDriver(chromeDriverService, chromeOptions);

                case AppSettings.BrowserName.Firefox:
                    FirefoxDriverService driverService = FirefoxDriverService.CreateDefaultService();
                    driverService.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    driverService.HideCommandPromptWindow = true;
                    driverService.SuppressInitialDiagnosticInformation = true;
                    return new FirefoxDriver(driverService);

                case AppSettings.BrowserName.InternetExplorer:
                    return new InternetExplorerDriver();

                case AppSettings.BrowserName.Edge:
                    return new EdgeDriver();

                case AppSettings.BrowserName.Safari:
                    return new SafariDriver();

                default:
                    throw new ArgumentException(string.Format("Not Definition. BrowserName:{0}", browserName));
            }
        }
    }
}
