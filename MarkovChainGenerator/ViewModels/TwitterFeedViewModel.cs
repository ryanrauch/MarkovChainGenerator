using LinqToTwitter;
using System.Linq;
using Markov;
using MarkovChainGenerator.Models;
using MarkovChainGenerator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MarkovChainGenerator.ViewModels
{
    public class TwitterFeedViewModel : ViewModelBase
    {
        private readonly ILoginStore _loginStoreService;
        private readonly ILinqToTwitterAuthorizer _linqToTwitterAuthorizer; 

        public TwitterFeedViewModel(
            ILoginStore loginStore,
            ILinqToTwitterAuthorizer linqToTwitterAuthorizer)
        {
            _loginStoreService = loginStore;
            _linqToTwitterAuthorizer = linqToTwitterAuthorizer;
            _userName = "realDonaldTrump";
            _maxTweets = 3200;
            _generateNumber = 10;
            _chain = new MarkovChain<string>(1);
        }

        private IAuthorizer _auth;

        private MarkovChain<string> _chain;

        private Int32 _generateNumber;
        public Int32 GenerateNumber
        {
            get { return _generateNumber; }
            set
            {
                _generateNumber = value;
                RaisePropertyChanged(() => GenerateNumber);
            }
        }

        private Int32 _maxTweets;
        public Int32 MaxTweets
        {
            get { return _maxTweets; }
            set
            {
                _maxTweets = Math.Min(value, 3200); // 3,200-Max
                RaisePropertyChanged(() => MaxTweets);
            }
        }

        private String _userName;
        public String UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        private List<String> _generatedTweets;
        public List<String> GeneratedTweets
        {
            get { return _generatedTweets; }
            set
            {
                _generatedTweets = value;
                RaisePropertyChanged(() => GeneratedTweets);
            }
        }

        public ICommand LoadDataCommand => new Command(async () => { await LoadTwitterData(); });

        public Int32 TweetsLoaded
        {
            get { return Tweets.Count; }
        }

        public String UserNameImageUrl
        {
            get
            {
                if(TweetsLoaded > 0)
                {
                    return Tweets[0].ImageUrl;
                }
                return String.Empty;
            }
        }

        private List<Tweet> _tweets = new List<Tweet>();
        public List<Tweet> Tweets
        {
            get { return _tweets; }
            set
            {
                _tweets = value;
                RaisePropertyChanged(() => Tweets);
                RaisePropertyChanged(() => TweetsLoaded);
                RaisePropertyChanged(() => UserNameImageUrl);
            }
        }

        public override async Task OnAppearingAsync()
        {
            //await RefreshData();
            await LoadTwitterData();
        }

        public override void Initialize(object param)
        {
            base.Initialize(param);
            if(param is String s)
            {
                UserName = s;
            }
        }

        public void InitializeChain()
        {
            _chain = new MarkovChain<string>(1);
            foreach (var t in Tweets)
            {
                _chain.Add(t.Text.Split(' '), 1);
            }
        }

        public void GenerateTweets(Int32 number)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            GeneratedTweets = new List<String>();
            for (int i = 0; i < number; i++)
            {
                var sentence = string.Join(" ", _chain.Chain(rand));
                GeneratedTweets.Add(sentence);
            }
        }

        public async Task RefreshData()
        {
            _auth = _linqToTwitterAuthorizer.GetAuthorizer(Secrets.TwitterAPIKey, Secrets.TwitterAPISecretKey);
            await _auth.AuthorizeAsync();

            //ht--tps://github.com/JoeMayo/LinqToTwitter/wiki/Querying-the-User-Timeline
            using (var ctx = new TwitterContext(_auth))
            {
                List<Status> tweets =
                await
                (from tweet in ctx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == UserName &&
                       tweet.Count == 200 &&
                       tweet.SinceID == 1/*sinceID*/ &&
                       tweet.TweetMode == TweetMode.Extended
                 select tweet)
                .ToListAsync();

                Tweets =
                   (from tweet in tweets
                    select new Tweet
                    {
                        StatusID = tweet.StatusID,
                        ScreenName = tweet.User.ScreenNameResponse,
                        Text = tweet.FullText,
                        ImageUrl = tweet.User.ProfileImageUrl
                    })
                   .ToList();
            }
            InitializeChain();
            GenerateTweets(GenerateNumber);
        }

        public async Task LoadTwitterData()
        {
            _auth = _linqToTwitterAuthorizer.GetAuthorizer(Secrets.TwitterAPIKey, Secrets.TwitterAPISecretKey);
            await _auth.AuthorizeAsync();

            using (var ctx = new TwitterContext(_auth))
            {
                await RunUserTimelineQueryAsync(ctx);
            }
            InitializeChain();
            GenerateTweets(GenerateNumber);
        }

        public async Task RunUserTimelineQueryAsync(TwitterContext twitterCtx)
        {
            //List<Status> tweets =
            //    await
            //    (from tweet in twitterCtx.Status
            //     where tweet.Type == StatusType.User &&
            //           tweet.ScreenName == "JoeMayo"
            //     select tweet)
            //    .ToListAsync();

            const int MaxTweetsToReturn = 200;
            int MaxTotalResults = MaxTweets;

            // oldest id you already have for this search term
            ulong sinceID = 1;

            // used after the first query to track current session
            ulong maxID;

            var combinedSearchResults = new List<Status>();

            List<Status> tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == UserName &&
                       tweet.Count == MaxTweetsToReturn &&
                       tweet.SinceID == sinceID &&
                       tweet.TweetMode == TweetMode.Extended
                 select tweet)
                .ToListAsync();

            if (tweets != null)
            {
                combinedSearchResults.AddRange(tweets);
                ulong previousMaxID = ulong.MaxValue;
                do
                {
                    // one less than the newest id you've just queried
                    //maxID = tweets.Max(status => status.StatusID) - 1;
                    //sinceID = (ulong)combinedSearchResults.Count;
                    //maxID = sinceID + MaxTweetsToReturn;
                    //Debug.Assert(maxID < previousMaxID);
                    //previousMaxID = maxID;
                    // one less than the newest id you've just queried
                    maxID = tweets.Min(status => status.StatusID) - 1;

                    //Debug.Assert(maxID < previousMaxID);
                    previousMaxID = maxID;

                    tweets =
                        await
                        (from tweet in twitterCtx.Status
                         where tweet.Type == StatusType.User &&
                               tweet.ScreenName == UserName &&
                               tweet.Count == MaxTweetsToReturn &&
                               tweet.MaxID == maxID &&
                               tweet.SinceID == sinceID &&
                               tweet.TweetMode == TweetMode.Extended
                         select tweet)
                        .ToListAsync();

                    combinedSearchResults.AddRange(tweets);

                } while (tweets.Any() && combinedSearchResults.Count < MaxTotalResults);

                //PrintTweetsResults(tweets);
                Tweets =
                   (from tweet in combinedSearchResults
                    select new Tweet
                    {
                        StatusID = tweet.StatusID,
                        ScreenName = tweet.User.ScreenNameResponse,
                        Text = tweet.FullText,
                        ImageUrl = tweet.User.ProfileImageUrl
                    })
                   .ToList();
            }
            else
            {
                //Console.WriteLine("No entries found.");
                Tweets = new List<Tweet>();
                Tweets.Add(new Tweet() { Text = "No entries found." });
            }
        }
    }
}
