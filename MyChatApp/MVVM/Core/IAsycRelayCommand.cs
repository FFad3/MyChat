using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyChatApp.MVVM.Core
{
    public interface IAsycRelayCommand : ICommand
    {
        Task ExecuteAsync(CancellationToken token);
    }
}