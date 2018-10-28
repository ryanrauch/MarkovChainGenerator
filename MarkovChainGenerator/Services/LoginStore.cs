using System;
using System.Threading.Tasks;
using MarkovChainGenerator.Services.Interfaces;

namespace MarkovChainGenerator.Services
{
    class LoginStore : ILoginStore
    {
        private enum OAuth
        {
            OAuthToken, OAuthSecret
        }
        public UserSecrets GetSecrets()
        {
            string token = null;
            string secret = null;
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey(OAuth.OAuthToken.ToString()))
            {
                token = Xamarin.Forms.Application.Current.Properties[OAuth.OAuthToken.ToString()].ToString();
            }
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey(OAuth.OAuthSecret.ToString()))
            {
                secret = Xamarin.Forms.Application.Current.Properties[OAuth.OAuthSecret.ToString()].ToString();
            }
            if (string.IsNullOrEmpty(secret))
            {
                return null;
            }
            return new UserSecrets() { OAuthSecret = secret, OAuthToken = token };
        }
        public async Task SetSecretsAsync(UserSecrets secrets)
        {
            await SetSecretsAsync(secrets.OAuthToken, secrets.OAuthSecret);
        }
        public async Task SetSecretsAsync(string oauthToken, string oauthSecret)
        {
            Xamarin.Forms.Application.Current.Properties[OAuth.OAuthSecret.ToString()] = oauthSecret;
            Xamarin.Forms.Application.Current.Properties[OAuth.OAuthToken.ToString()] = oauthToken;
            await Xamarin.Forms.Application.Current.SavePropertiesAsync();
        }
    }
}
