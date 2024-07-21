using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace HelperServices
{
    public class KeyVaultService
    {
        public string KeyVaultName 
        {
            get
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);

                var configuration = builder.Build();

                var keyVaultName = configuration["EventHubKeyVaultName"];

                return keyVaultName == null ? "" : keyVaultName;
            }
        }

        public async Task<string> RetriveSecretFromVaultAsync(string keyVaultName, string secretName)
        {
            KeyVaultSecret kvs;
            var keyVaultURI = $"https://{keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.GetSecretAsync(secretName);

            return kvs.Value;
        }
    }
}
