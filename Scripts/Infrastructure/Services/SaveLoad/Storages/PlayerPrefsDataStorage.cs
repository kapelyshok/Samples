using Newtonsoft.Json;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.SaveLoad.Storages
{
    public class PlayerPrefsDataStorage : SaveDataStorage
    {
        public override T Load<T>()
        {
            var key = typeof(T).ToString();
            var json = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log($"Loaded json for {key}: Default");
                return default;
            }

            Debug.Log($"Loaded json for {key}: {json}");
            var parsed = JsonConvert.DeserializeObject<T>(json);
            return parsed;
        }

        public override void Save<T>(T data, bool isPrintLog = false)
        {
            var json = JsonConvert.SerializeObject(data);
            var key = typeof(T).ToString();
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
            if (isPrintLog)
            {
                Debug.Log($"Saved data for {key}: {json}");
            }
        }

        public override void Save(object[] saveData, bool isPrintLog = false)
        {
            foreach (var data in saveData)
            {
                var json = JsonConvert.SerializeObject(data);
                var key = data.GetType().ToString();
                PlayerPrefs.SetString(key, json);
                if (isPrintLog)
                {
                    Debug.Log($"Saved data for {key}: {json}");
                }
            }
            
            PlayerPrefs.Save();
        }

        public override void Delete<T>()
        {
            var key = typeof(T).ToString();
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"Delete data for {key}");
        }

        public override void Delete(object[] saveData)
        {
            foreach (var data in saveData)
            {
                var key = data.GetType().ToString();
                PlayerPrefs.DeleteKey(key);
                
                Debug.Log($"Delete data for {key}");
            }
            
            PlayerPrefs.Save();
        }
    }
}