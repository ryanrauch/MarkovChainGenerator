using MarkovChainGenerator.Services.Interfaces;
using MarkovChainGenerator.Views.ContentPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MarkovChainGenerator.Services
{
    public class SinglePageNavigationService : INavigationService
    {
        protected Application CurrentApplication
        {
            get { return Application.Current; }
        }

        public Task NavigatePushAsync<T>(T page) where T : Page
        {
            CurrentApplication.MainPage = page;
            return Task.CompletedTask;
        }

        public Task NavigatePushAsync<T>(T page, object param) where T : Page
        {
            (page.BindingContext as ViewModelBase).Initialize(param);
            return NavigatePushAsync(page);
        }

        public void NavigateToMain()
        {
            CurrentApplication.MainPage = new TwitterSearchUsersView();
        }
    }
}