using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Editor.GoogleSheet.GoogleOAuth;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor.GoogleSheet
{
    public class GoogleSheetClient
    {
        private string _token;
        private const string KeyAuthorization = "Authorization";
        private const int RetryMax = 1;
        private static int _retryCounter;
        private static bool _isUpdatingAuthorization;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private string _refreshToken;

        public GoogleSheetClient(string clientId, string clientSecret, string refreshToken)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _refreshToken = refreshToken;
            UpdateAuthorization().Forget();
        }

        public UniTask<string> GetAsync(string url)
        {
            return RestAsync(Factory);
            UnityWebRequest Factory() => UnityWebRequest.Get(url);
        }

        public UniTask<string> PostAsync(string url, string body)
        {
            return RestAsync(Factory);

            UnityWebRequest Factory()
            {
                var unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                var postData = Encoding.UTF8.GetBytes(body);
                unityWebRequest.uploadHandler = new UploadHandlerRaw(postData);
                unityWebRequest.SetRequestHeader("Content-Type", "application/json");
                return unityWebRequest;
            }
        }

        private async UniTask<string> RestAsync(Func<UnityWebRequest> factory)
        {
            if (string.IsNullOrEmpty(_token)) await UpdateAuthorization();

            var unityWebRequest = factory.Invoke();
            try
            {
                unityWebRequest.SetRequestHeader(KeyAuthorization, $"Bearer {_token}");
                var request = await unityWebRequest.SendWebRequest();
                var result = request.downloadHandler?.text;
                _retryCounter = 0;

                var responseCode = (HttpStatusCode) request.responseCode;
                if (responseCode != HttpStatusCode.OK) throw new HttpRequestException(responseCode.ToString());

                return result;
            }
            catch (UnityWebRequestException e)
            {
                await UpdateAuthorization();
                if (_retryCounter++ < RetryMax) return await RestAsync(factory);
                Debug.LogError(JsonConvert.SerializeObject(e, Formatting.Indented));
                throw;
            }
            finally
            {
                unityWebRequest.Dispose();
            }
        }

        private async UniTask UpdateAuthorization()
        {
            if (_isUpdatingAuthorization)
            {
                await UniTask.WaitWhile(() => _isUpdatingAuthorization);
                return;
            }

            _isUpdatingAuthorization = true;

            if (string.IsNullOrEmpty(_refreshToken))
            {
                var authorizationResult = await AuthorizeAsync();
                var accessTokenResult = await GetAccessTokenAsync(authorizationResult);
                _token = accessTokenResult.AccessToken;
            }
            else
            {
                var result = await RefreshAccessTokenAsync(_refreshToken);
                _token = result.AccessToken;
            }

            _isUpdatingAuthorization = false;
        }

        private async UniTask<AuthorizationCodeProvider.Result> AuthorizeAsync()
        {
            var provider = new AuthorizationCodeProvider(_clientId);
            var handle = await provider.ProvideAsync();
            return handle.Result;
        }

        private async UniTask<AccessTokenProvider.Result> GetAccessTokenAsync(AuthorizationCodeProvider.Result result)
        {
            var provider = new AccessTokenProvider(_clientId, _clientSecret);
            var handle = await provider.ProvideAsync(result.AuthorizationCode, result.CodeVerifier, result.RedirectUri);
            var res = handle.Result;
            _refreshToken = res.RefreshToken;
            return res;
        }

        private async UniTask<RefreshedAccessTokenProvider.Result> RefreshAccessTokenAsync(string refreshToken)
        {
            var provider = new RefreshedAccessTokenProvider(_clientId, _clientSecret);
            var handle = await provider.ProvideAsync(refreshToken);
            return handle.Result;
        }
    }
}
