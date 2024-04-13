using System.Diagnostics.CodeAnalysis;

namespace Editor.GoogleSheet.GoogleOAuth
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class property
    {
        public const string ACCESS_TOKEN = "access_token";
        public const string ACCESS_TYPE = "access_type";
        public const string AUTHORIZATION_CODE = "authorization_code";
        public const string CLIENT_ID = "client_id";
        public const string CLIENT_SECRET = "client_secret";
        public const string CODE = "code";
        public const string CODE_CHALLENGE = "code_challenge";
        public const string CODE_CHALLENGE_METHOD = "code_challenge_method";
        public const string CODE_VERIFIER = "code_verifier";
        public const string CONTENT_TYPE = "Content-Type";
        public const string ERROR = "error";
        public const string EXPIRES_IN = "expires_in";
        public const string GRANT_TYPE = "grant_type";
        public const string ID_TOKEN = "id_token";
        public const string REDIRECT_URI = "redirect_uri";
        public const string REFRESH_TOKEN = "refresh_token";
        public const string RESPONSE_TYPE = "response_type";
        public const string S256 = "S256";
        public const string SCOPE = "scope";
        public const string STATE = "state";
        public const string TOKEN_TYPE = "token_type";

        public const string _access_type = "offline";
        public const string _content_type = "application/x-www-form-urlencoded";
        public const string _scope = "https://www.googleapis.com/auth/drive";
        public const string _url_access_token = "https://www.googleapis.com/oauth2/v4/token";
        public const string _url_auth = "https://accounts.google.com/o/oauth2/v2/auth";
    }
}
