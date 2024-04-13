using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Editor.GoogleSheet
{
    public class ConfidentialLoader
    {
        private readonly string _path;

        public ConfidentialLoader(string path)
        {
            _path = path;
        }

        public string Get(string name)
        {
            var json = ReloadJson();
            return json.FirstOrDefault(x => x.Name == name)?.Key;
        }

        public void Add(string name, string value)
        {
            var json = ReloadJson().ToList();
            json.Remove(json.Find(x => x.Name == name));
            var added = json.Append(new Data(name, value));
            var text = JsonConvert.SerializeObject(added, Formatting.Indented);
            File.WriteAllText(_path, text);
        }

        private IEnumerable<Data> ReloadJson()
        {
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(_path);
            return JsonConvert.DeserializeObject<IEnumerable<Data>>(asset.text);
        }

        private class Data
        {
            [JsonProperty("name")] public readonly string Name;
            [JsonProperty("key")] public readonly string Key;

            public Data(string name, string key)
            {
                Name = name;
                Key = key;
            }
        }
    }
}
