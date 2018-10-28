using System;
using System.Threading.Tasks;
using MarkovChainGenerator.Services.Interfaces;

namespace MarkovChainGenerator.ViewModels
{
    public class InitialViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public InitialViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override Task OnAppearingAsync()
        {
            throw new NotImplementedException();
        }
    }
}
