using System;

namespace AutomaticReservation_UI.ToyokoInn
{
    /// <summary>
    /// サイト別設定（東横イン）
    /// </summary>
    public static class SiteConfig
    {
        /// <summary>
        /// 基底ディレクトリ
        /// </summary>
        public static string BASE_DIR = String.Format(@"{0}\AutomaticReservation_UI\ToyokoInn", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        /// <summary>
        /// (XPath) お気に入りリスト
        /// </summary>
        public static string XPATH_FAVORITE = "//*[@id=\"header\"]/div[2]/div[2]/ul[2]/li[3]/a/span";
        /// <summary>
        /// (XPath) メールアドレスフォーム
        /// </summary>
        public static string XPATH_FORM_ADDRESS = "//*[@id=\"mainArea\"]/form/div/div[1]/div/div/ul[1]/li[1]/label/input";
        /// <summary>
        /// (XPath) パスワードフォーム
        /// </summary>
        public static string XPATH_PASS = "//*[@id=\"mainArea\"]/form/div/div[1]/div/div/ul[1]/li[2]/label/input";
        /// <summary>
        /// (XPath) ログインボタン
        /// </summary>
        public static string XPATH_LOGINBTN = "//*[@id=\"linkLogin\"]";
        /// <summary>
        /// (XPath) 予約ボタン（INTEGER -> 任意の数字）
        /// </summary>
        // 2019/02/03  サイト変更対応
        public static string XPATH_RESERVEBTN = "//*[@id=\"mainArea\"]/section[1]/div[INTEGER]/div/div[1]/div[3]/ul/li/a/span";
        //public static string XPATH_RESERVEBTN = "//*[@id=\"mainArea\"]/section[1]/div[INTEGER]/div/div[2]/div[3]/ul/li/a/span";
        /// <summary>
        /// (XPath) 禁煙か喫煙かを判断する要素（INTEGER -> 任意の数字）
        /// </summary>
        public static string XPATH_SMOKELABEL = "//*[@id=\"mainArea\"]/section[1]/h2[INTEGER]/em/span";
        /// <summary>
        /// (XPath) ホテル一覧ページの都道府県名（ITER_REGION, ITER_PREF -> 任意の数字）
        /// </summary>
        public static string XPATH_PREFNAME = "//*[@id=\"mainArea\"]/section[ITER_REGION]/h4[ITER_PREF]/em";
        /// <summary>
        /// (XPath) ホテル一覧ページのホテルID（ITER_REGION, ITER_HOTEL -> 任意の数字）
        /// </summary>
        public static string XPATH_HOTELID = "//*[@id=\"mainArea\"]/section[ITER_REGION]/section[ITER_HOTEL]/div/div[1]/span";
        /// <summary>
        /// (XPath) ホテル一覧ページのホテル名（ITER_REGION, ITER_HOTEL -> 任意の数字）
        /// </summary>
        public static string XPATH_HOTELNAME = "//*[@id=\"mainArea\"]/section[ITER_REGION]/section[ITER_HOTEL]/div/div[2]/a";
        /// <summary>
        /// (XPath) ホテル一覧ページの住所（ITER_REGION, ITER_HOTEL -> 任意の数字）
        /// </summary>
        public static string XPATH_ADDRESS = "//*[@id=\"mainArea\"]/section[ITER_REGION]/section[ITER_HOTEL]/div/div[3]";
        /// <summary>
        /// (XPath) 連絡先電話番号
        /// </summary>
        public static string XPATH_TEL = "//*[@id=\"mainArea\"]/section[2]/div/div[2]/div/div[1]/table/tbody/tr[7]/td/span/input";
        /// <summary>
        /// (XPath) チェックイン予定時刻
        /// </summary>
        public static string XPATH_CHKINTIME = "//*[@id=\"mainArea\"]/section[2]/div/div[2]/div/div[1]/table/tbody/tr[8]/td/div[1]/select";
        /// <summary>
        /// (XPath) 確認ボタン
        /// </summary>
        public static string XPATH_CONFIRM = "//*[@id=\"cnfrm\"]/a/span";
        /// <summary>
        /// 予約済みの場合に表示されるフレームのクラス名
        /// </summary>
        public static string IFRAME_OVERWRITE = ".cboxIframe";
        /// <summary>
        /// (XPath) 予約済みの場合に表示される要素
        /// </summary>
        public static string XPATH_OVERWRITE = "//*[@id=\"mainArea\"]/form/section/p[1]";
        /// <summary>
        /// 予約済みの場合に表示される文字列
        /// </summary>
        public static string STR_OVERWRITE = "同一日にすでにご予約いただいているホテルがあります。";
        /// <summary>
        /// 予約を上書きする場合に押すボタン
        /// </summary>
        public static string XPATH_BTN_OVERWRITE = "//*[@id=\"mainArea\"]/form/section/div/ul/li[2]/span/input";
        /// <summary>
        /// (XPath) 「ご利用規約に同意する」チェック（2018/04/07変更）
        /// </summary>
        public static string XPATH_CHKAGREE = "//*[@id=\"mainArea\"]/div[3]/div/ul[4]/li/label/input";
        /// <summary>
        /// (XPath) 予約確定ボタン
        /// </summary>
        public static string XPATH_OK = "//*[@id=\"entry\"]";
        /// <summary>
        /// (XPath) 予約が正常に終了したことを確認する要素
        /// </summary>
        public static string XPATH_CHK_VALIDATE = "/html/body/main/div/div/div[2]/p";
        /// <summary>
        /// 基底URL
        /// </summary>
        public static string BASE_URL = @"https://www.toyoko-inn.com/";
        /// <summary>
        /// 予約が正常に終了したことを確認する文字列
        /// </summary>
        public static string STR_VALIDATE = "ご予約ありがとうございました。";
        /// <summary>
        /// ループごとのスリープ時間（ミリ秒）
        /// </summary>
        public static int TIME_SLEEP = 30000;
    }
}
