using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutomaticReservation_UI.Common
{
    /// <summary>
    /// ローテーションファイル管理クラス
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// 指定ディレクトリのファイルを作成日付の古い順に削除し一定のファイル数を維持する
        /// </summary>
        /// <param name="directory">ディレクトリ（最後の区切りを含まない）</param>
        /// <param name="extension">ファイル拡張子</param>
        /// <param name="saveCount">保持するファイル数</param>
        public static void RemoveFileObsolete(string directory, string extension, int saveCount)
        {
            // ファイル名の正規表現
            var reg = new Regex(String.Format(".{0}$", extension));
            // ファイル一覧を作成日付の新しい順に取得
            var files = Directory.GetFiles(directory).Where(f => reg.IsMatch(f)).OrderByDescending(f => File.GetCreationTime(f));
            int count = 0;
            foreach (string file in files)
            {
                // カウンタ増
                count++;
                if (count > saveCount)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // 使用中のファイルは削除不可
                        // Write log
                    }
                }
            }
            return;
        }
    }
}
