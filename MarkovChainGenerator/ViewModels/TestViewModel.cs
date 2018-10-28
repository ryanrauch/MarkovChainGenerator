﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using MarkovChainGenerator.Services.Interfaces;
using LinqToTwitter;
using System.Collections.Generic;
using MarkovChainGenerator.Models;

namespace MarkovChainGenerator.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        private readonly ILoginStore _loginStoreService;
        private readonly ILinqToTwitterAuthorizer _linqToTwitterAuthorizer; //_authSvc;

        public TestViewModel(ILoginStore loginStore,
                            ILinqToTwitterAuthorizer linqToTwitterAuthorizer)
        {
            _loginStoreService = loginStore;
            _linqToTwitterAuthorizer = linqToTwitterAuthorizer;
        }

        private IAuthorizer _auth;
        private UserSecrets _userSecrets;
        private string consumerKey = Secrets.TwitterAPIKey;
        private string consumerSecret = Secrets.TwitterAPISecretKey;

        private List<Tweet> _tweets;
        public List<Tweet> Tweets
        {
            get { return _tweets; }
            set 
            {
                _tweets = value;
                RaisePropertyChanged(() => Tweets);
            }
        }

        public override Task OnAppearingAsync()
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public void InitAuthentication()
        {
            if (_userSecrets != null) return;
            var oauth = new Xamarin.Auth.OAuth1Authenticator(consumerKey, consumerSecret,
                  new Uri("https://api.twitter.com/oauth/request_token"),
                   new Uri("https://api.twitter.com/oauth/authorize"),
                   new Uri("https://api.twitter.com/oauth/access_token"),
                   new Uri("http://127.0.0.1/"));
            oauth.Completed += Oauth_Completed;
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(oauth);
        }

        private async void Oauth_Completed(object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e)
        {
            _auth = _linqToTwitterAuthorizer.GetAuthorizer(consumerKey,
                    consumerSecret,
                    e.Account.Properties["oauth_token"],
                    e.Account.Properties["oauth_token_secret"]);
            await _loginStoreService.SetSecretsAsync(
                 e.Account.Properties["oauth_token"],
                 e.Account.Properties["oauth_token_secret"]
             );
            //RefreshTimeline.ChangeCanExecute();
            //RefreshTimeline.Execute(null);
        }

        private async Task RefreshAsync()
        {
            await _auth.AuthorizeAsync();
            using (var ctx = new TwitterContext(_auth))
            {
                var cs = await ctx.Status.ToListAsync();
                Tweets = new List<Tweet>();
                foreach(var s in cs)
                {
                    Tweet t = new Tweet()
                    {
                        StatusID = s.StatusID,
                        ScreenName = s.User.ScreenName,
                        Text = s.Text
                    };
                    Tweets.Add(t);
                }
                //var srch = await
                //      (from tweet in ctx.Status 
                //       where tweet.Type == StatusType.Home
                //       select new Tweet()
                //       {
                //           StatusID = tweet.StatusID,
                //           ScreenName = tweet.User.ScreenNameResponse,
                //           Text = tweet.Text,
                //           ImageUrl = tweet.RetweetedStatus != null && tweet.RetweetedStatus.User != null ?
                //                      tweet.RetweetedStatus.User.ProfileImageUrl.Replace("http://", "https://") : tweet.User.ProfileImageUrl
                //       }).ToListAsync();
                //Tweets = new List<Tweet>(srch);
            }
        }

        private Command _refreshTimeline;
        public Command RefreshTimeline
        {
            get
            {
                if (_refreshTimeline == null)
                {
                    _refreshTimeline = new Command(async () =>
                    {
                        IsRefreshing = true;
                        try
                        {
                            await RefreshAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                        IsRefreshing = false;
                    }, () => { return _loginStoreService.GetSecrets() != null; });
                }
                return _refreshTimeline;
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            private set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }
    }
}
