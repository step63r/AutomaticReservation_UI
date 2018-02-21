using System.Threading.Tasks;
using System.Windows.Input;

namespace AutomaticReservation_UI.Common
{
    /// <summary>
    /// 非同期コマンドインタフェース
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
