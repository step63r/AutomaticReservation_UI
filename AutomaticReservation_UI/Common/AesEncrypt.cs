using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace AutomaticReservation_UI.Common
{
    public static class AesEncrypt
    {
        /// <summary>
        /// 入力文字列をAES暗号化してBase64形式で返すメソッド
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncryptToBase64(string plainText, byte[] key, byte[] iv)
        {
            // 入力文字列をバイト型配列に変換
            byte[] src = Encoding.Unicode.GetBytes(plainText);
            WriteLine($"平文のバイト型配列の長さ={src.Length}");
            // 出力例：平文のバイト型配列の長さ=60

            // Encryptor（暗号化器）を用意する
            using (var am = new AesManaged())
            using (var encryptor = am.CreateEncryptor(key, iv))
            // ファイルを入力とするなら、ここでファイルを開く
            // using (FileStream inStream = new FileStream(FilePath, ……省略……
            // 出力ストリームを用意する
            using (var outStream = new MemoryStream())
            {
                // 暗号化して書き出す
                using (var cs = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(src, 0, src.Length);
                    // 入力がファイルなら、inStreamから一定量ずつバイトバッファーに読み込んで
                    // cse.Writeで書き込む処理を繰り返す（復号のサンプルコードを参照）
                }
                // 出力がファイルなら、以上で完了

                // Base64文字列に変換して返す
                byte[] result = outStream.ToArray();
                WriteLine($"暗号のバイト型配列の長さ={result.Length}");
                // 出力例：暗号のバイト型配列の長さ=64
                // 出力サイズはBlockSize（既定値16バイト）の倍数になる
                return Convert.ToBase64String(result);
            }
        }

        /// <summary>
        /// 暗号化されたBase64形式の入力文字列をAES復号して平文の文字列を返すメソッド
        /// </summary>
        /// <param name="base64Text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptFromBase64(string base64Text, byte[] key, byte[] iv)
        {
            // Base64文字列をバイト型配列に変換
            byte[] src = Convert.FromBase64String(base64Text);

            // Decryptor（復号器）を用意する
            using (var am = new AesManaged())
            using (var decryptor = am.CreateDecryptor(key, iv))
            // 入力ストリームを開く
            using (var inStream = new MemoryStream(src, false))
            // 出力ストリームを用意する
            using (var outStream = new MemoryStream())
            {
                // 復号して一定量ずつ読み出し、それを出力ストリームに書き出す
                using (var cs = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[4096]; // バッファーサイズはBlockSizeの倍数にする
                    int len = 0;
                    while ((len = cs.Read(buffer, 0, 4096)) > 0)
                        outStream.Write(buffer, 0, len);
                }
                // 出力がファイルなら、以上で完了

                // 文字列に変換して返す
                byte[] result = outStream.ToArray();
                return Encoding.Unicode.GetString(result);
            }
        }
    }
}
