using LinqToTwitter;
using MarkovChainGenerator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MarkovChainGenerator.ViewModels
{
    public class TwitterSearchUsersViewModel : ViewModelBase
    {
        private readonly ILoginStore _loginStoreService;
        private readonly ILinqToTwitterAuthorizer _linqToTwitterAuthorizer;
        private IAuthorizer _auth;

        public TwitterSearchUsersViewModel(
            ILoginStore loginStore,
            ILinqToTwitterAuthorizer linqToTwitterAuthorizer)
        {
            _loginStoreService = loginStore;
            _linqToTwitterAuthorizer = linqToTwitterAuthorizer;
        }

        public override Task OnAppearingAsync()
        {
            return Task.CompletedTask;
        }

        public async Task SearchUsers(String partial)
        {
            _auth = _linqToTwitterAuthorizer.GetAuthorizer(Secrets.TwitterAPIKey, Secrets.TwitterAPISecretKey);
            await _auth.AuthorizeAsync();

            using (var ctx = new TwitterContext(_auth))
            {
                var foundUsers =
                await
                (from user in ctx.User
                 where user.Type == UserType.Search &&
                       user.Query == partial
                 select user)
                .ToListAsync();

                if (foundUsers != null)
                    foundUsers.ForEach(user =>
                        Console.WriteLine("User: " + user.ScreenNameResponse));
            }
        }
    }
}
