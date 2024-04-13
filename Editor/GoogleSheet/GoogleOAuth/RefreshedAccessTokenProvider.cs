using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.GoogleSheet.GoogleOAuth
{
    public class RefreshedAccessTokenProvider
    {
        public class Result
        {
            [JsonProperty(property.ACCESS_TOKEN)] public readonly string AccessToken;
            [JsonProperty(property.EXPIRES_IN)] public readonly string ExpiresIn;
            [JsonProperty(property.SCOPE)] public readonly string Scope;
            [JsonProperty(property.TOKEN_TYPE)] public readonly string TokenType;

            public Result(string accessToken, string expiresIn, string scope, string tokenType)
            {
                AccessToken = accessToken;
                ExpiresIn = expiresIn;
                Scope = scope;
                TokenType = tokenType;
            }
        }

        private readonly string _clientId;
        private readonly string _clientSecret;

        public RefreshedAccessTokenProvider(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<ProvideHandle<Result>> ProvideAsync(string refreshToken)
        {
            var form = new WWWForm();
            form.AddField(property.CLIENT_ID, _clientId);
            form.AddField(property.CLIENT_SECRET, _clientSecret);
            form.AddField(property.GRANT_TYPE, property.REFRESH_TOKEN);
            form.AddField(property.REFRESH_TOKEN, refreshToken);

            var taskCompletionSource = new TaskCompletionSource<AsyncOperation>();
            var request = UnityWebRequest.Post(property._url_access_token, form);
            request.SetRequestHeader(property.CONTENT_TYPE, property._content_type);
            request.SendWebRequest().completed += x => taskCompletionSource.SetResult(x);

            await taskCompletionSource.Task;
            var handle = new ProvideHandle<Result>();
            if (request.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
            {
                handle.Fail(new NetworkException(request.error, request.responseCode));
                request.Dispose();
                return handle;
            }

            var response = JsonConvert.DeserializeObject<Result>(request.downloadHandler.text);
            handle.Success(response);
            request.Dispose();
            return handle;
        }
    }
}
