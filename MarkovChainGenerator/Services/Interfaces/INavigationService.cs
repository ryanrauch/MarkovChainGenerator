using System.Threading.Tasks;
using Xamarin.Forms;

namespace MarkovChainGenerator.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateToMain();
        Task NavigatePushAsync<T>(T page) where T : Page;
        Task NavigatePushAsync<T>(T page, object param) where T : Page;
    }
}