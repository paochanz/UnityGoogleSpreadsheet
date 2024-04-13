using System.IO;
using Cysharp.Threading.Tasks;
using Editor.GoogleSheet;
using UnityEditor;
using UnityEngine;

namespace UnityGoogleSpreadsheet._Sample.Editor.GoogleSheet
{
    public class ExportGssData : EditorWindow
    {
        private const string DataPath = "Packages/UnityGoogleSpreadsheet/~Sample/Resources";
        private const string KeyPath = "Packages/UnityGoogleSpreadsheet/~Sample/Editor/key.json";
        private static GoogleSheetClient _googleSheetClient;
        private static ConfidentialLoader _confidentialLoader;
        private static string _googleAppID;
        private static string _gssAppURL;

        [MenuItem("Window/ExportGSSData")]
        private static void Create()
        {
            GetWindow<ExportGssData>(nameof(ExportGssData));
        }

        private void OnEnable()
        {
            _confidentialLoader = new ConfidentialLoader(KeyPath);
            var clientId = _confidentialLoader.Get("GoogleClientID");
            var clientSecret = _confidentialLoader.Get("GoogleClientSecret");
            var refreshToken = _confidentialLoader.Get("GoogleRefreshToken");
            _googleSheetClient = new GoogleSheetClient(clientId, clientSecret, refreshToken);
            _gssAppURL = _confidentialLoader.Get("GSSAppURL");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Export All Data", GUILayout.Width(150))) ExportAllData();
        }

        private static void ExportAllData()
        {
            var directoryInfo = new DirectoryInfo(DataPath);
            UniTask.WhenAll(directoryInfo
                .GetFiles("*.json")
                .Select(file => ExecuteDataAsync(file.FullName)));
        }

        private static async UniTask ExecuteDataAsync(string path)
        {
            var type = Path.GetFileNameWithoutExtension(path);
            var url = $"{_gssAppURL}?sheet={type}";
            var data = await _googleSheetClient.GetAsync(url);
            await File.WriteAllTextAsync(path, data);
            Debug.Log($"DONE: {path}");
        }
    }
}
