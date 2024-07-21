using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace HelperServices
{
    public class KeyVaultService
    {
        IConfigurationBuilder _builder;
        IConfigurationRoot _configurationRoot;
        private string _keyVaultName = string.Empty;
        private string _secretName = string.Empty;

        public KeyVaultService()
        {
            _builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);
            _configurationRoot = _builder.Build();
        }

        public string KeyVaultName 
        {
            get
            {
                _keyVaultName = _configurationRoot["EventHubKeyVaultName"];

                return _keyVaultName == null ? "" : _keyVaultName;
            }
        }

        public string SecretName 
        { 
            get
            {
                _secretName = _configurationRoot["SecretName"];

                return _secretName == null ? "" : _secretName;
            }
        }

        public async Task<string> RetriveSecretFromVaultAsync()
        {
            KeyVaultSecret kvs;
            var keyVaultURI = $"https://{_keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.GetSecretAsync(_secretName);

            return kvs.Value;
        }

        public async Task<string> SetSecretIntoVaultAsync(string newSecretValue)
        {
            KeyVaultSecret kvs;

            var keyVaultURI = $"https://{_keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.SetSecretAsync(_secretName, newSecretValue);

            return kvs.Value;
        }
    }
}
