using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.GoogleSheet.GoogleOAuth
{
    public class AccessTokenProvider
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AccessTokenProvider(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<ProvideHandle<Result>> ProvideAsync(string authorizationCode, string codeVerifier,
            string redirectUri)
        {
            var form = new WWWForm();
            form.AddField(property.CLIENT_ID, _clientId);
            form.AddField(property.CLIENT_SECRET, _clientSecret);
            form.AddField(property.CODE, authorizationCode);
            form.AddField(property.CODE_VERIFIER, codeVerifier);
            form.AddField(property.GRANT_TYPE, property.AUTHORIZATION_CODE);
            form.AddField(property.REDIRECT_URI, redirectUri);

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

        public class Result
        {
            [JsonProperty(property.ACCESS_TOKEN)] public readonly string AccessToken;
            [JsonProperty(property.EXPIRES_IN)] public readonly string ExpiresIn;
            [JsonProperty(property.ID_TOKEN)] public readonly string IdToken;
            [JsonProperty(property.REFRESH_TOKEN)] public readonly string RefreshToken;
            [JsonProperty(property.SCOPE)] public readonly string Scope;
            [JsonProperty(property.TOKEN_TYPE)] public readonly string TokenType;

            public Result(string accessToken, string expiresIn, string idToken, string refreshToken, string scope,
                string tokenType)
            {
                AccessToken = accessToken;
                ExpiresIn = expiresIn;
                IdToken = idToken;
                RefreshToken = refreshToken;
                Scope = scope;
                TokenType = tokenType;
            }
        }
    }
}
