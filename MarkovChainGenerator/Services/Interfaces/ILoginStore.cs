using System.Threading.Tasks;

namespace MarkovChainGenerator.Services.Interfaces
{
    public interface ILoginStore
    {
        UserSecrets GetSecrets();
        Task SetSecretsAsync(UserSecrets secrets);
        Task SetSecretsAsync(string oauthToken, string oauthSecret);
    }
}

