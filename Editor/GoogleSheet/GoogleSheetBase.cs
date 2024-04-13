using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Editor.GoogleSheet
{
    public class GoogleSheetBase
    {
        private readonly GoogleSheetClient _client;
        private readonly string _spreadsheetKey;
        private readonly string _appId;
        private string _spreadSheetJson;

        public GoogleSheetBase(GoogleSheetClient client, string spreadsheetKey, string appId)
        {
            _client = client;
            _spreadsheetKey = spreadsheetKey;
            _appId = appId;
        }

        public UniTask UpdateSheetAsync(string options, string body = "")
        {
            var url = $"https://script.google.com/macros/s/{_appId}/exec?book={_spreadsheetKey}&{options}";
            return _client.PostAsync(url, body);
        }

        public async UniTask<int> GetSheetIdAsync(string tableName)
        {
            var json = await GetSpreadSheetDataAsync();
            return Convert.ToInt32(JsonConvert.DeserializeXmlNode(json)
                ?.SelectSingleNode($"//sheets/properties[title[text()='{tableName}']]/sheetId")
                ?.InnerText);
        }

        public async UniTask<int> GetFirstSheetIdAsync()
        {
            var json = await GetSpreadSheetDataAsync();
            return Convert.ToInt32(JsonConvert.DeserializeXmlNode(json)
                ?.SelectSingleNode("//sheets/properties/sheetId")
                ?.InnerText);
        }

        public async UniTask<int> GetNthSheetIdAsync(int n)
        {
            var json = await GetSpreadSheetDataAsync();
            return Convert.ToInt32(JsonConvert.DeserializeXmlNode(json)
                ?.SelectNodes("//sheets/properties/sheetId")
                ?[n]
                ?.InnerText);
        }

        private async UniTask<string> GetSpreadSheetDataAsync()
        {
            if (!string.IsNullOrEmpty(_spreadSheetJson)) return _spreadSheetJson;

            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{_spreadsheetKey}";
            _spreadSheetJson = "{\"root\":" + await _client.GetAsync(url) + "}";
            return _spreadSheetJson;
        }

        public UniTask<string> BatchUpdateAsync(string request)
        {
            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{_spreadsheetKey}:batchUpdate";
            return _client.PostAsync(url, request);
        }
    }
}
