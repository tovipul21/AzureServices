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
            _keyVaultName = this._configurationRoot["EventHubKeyVaultName"];
            _secretName = this._configurationRoot["SecretName"];
        }

        public string KeyVaultName 
        {
            get
            {
                //_keyVaultName = this._configurationRoot["EventHubKeyVaultName"];

                return _keyVaultName == null ? "" : _keyVaultName;
            }
        }

        public string SecretName 
        { 
            get
            {
                //_secretName = this._configurationRoot["SecretName"];

                return _secretName == null ? "" : _secretName;
            }
        }

        public async Task<string> RetriveSecretFromVaultAsync()
        {
            KeyVaultSecret kvs;
            var keyVaultURI = $"https://{this.KeyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.GetSecretAsync(this.SecretName);

            return kvs.Value;
        }

        public async Task<string> RetriveSecretFromVaultAsync(string secretName)
        {
            _secretName = secretName;

            return await RetriveSecretFromVaultAsync();
        }

        public async Task<string> SetSecretIntoVaultAsync(string newSecretValue)
        {
            KeyVaultSecret kvs;

            var keyVaultURI = $"https://{this.KeyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.SetSecretAsync(this.SecretName, newSecretValue);

            return kvs.Value;
        }
    }
}
