using System;
using LinqToTwitter;

namespace MarkovChainGenerator.Services.Interfaces
{
    public interface ILinqToTwitterAuthorizer
    {
        IAuthorizer GetAuthorizer(string consumerKey, string consumerSecret);
        IAuthorizer GetAuthorizer(string consumerKey, string consumerSecret, string oAuthToken, string oAuthTokenSecret);

    }
}
