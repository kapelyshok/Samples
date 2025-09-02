using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading;
using AtomicApps.Infrastructure.Services.SaveLoad.Data;
using AtomicApps.Infrastructure.Services.SaveLoad.Storages;
using AtomicApps.Tools;
using Cysharp.Threading.Tasks;
using AtomicApps.Infrastructure.Services.SaveLoad;
using UnityEngine;
using VInspector;

namespace AtomicApps.Infrastructure.Services.SaveLoad
{
    public class SaveService : MonoBehaviour, ISaveService
    {
        [HelpBox(
            @"SETUP INSTRUCTION
1. Create a data class and inherit it from SavableData.
2. Call GetData to register this data class into SaveService.
3. All the registered data will be automatically saved during destruction phase. 
You can save data manually using SaveDataImmediately() or SaveAllDataImmediately.
4. You can use auto save feature:
Check isAllowAutoSave and select desired timeout for system to perform auto save every n seconds.",
            HelpBoxMessageType.Info)]
        
        [SerializeField]
        private SaveDataStorage dataStorage;
        [Space]
        [SerializeField]
        private bool isAllowAutoSave = false;
        [SerializeField, HideIf("isAllowAutoSave", false)]
        private float autoSaveTimeoutInSeconds = 600;
        
        private CancellationTokenSource _autoSaveCTS;
        private SpriteRenderer _spriteRenderer;
        private readonly ConcurrentDictionary<Type, object> _saveDataMap = new();
        private UniTask _autoSaveTask;

        private void Awake()
        {
            _autoSaveCTS = new CancellationTokenSource();
            
            _autoSaveTask = AutoSaveTask(_autoSaveCTS.Token);
        }

        public T GetData<T>(T defaultData = default) where T : ISavable, new()
        {
            if (_saveDataMap.TryGetValue(typeof(T), out var saveData))
            {
                return (T) saveData;
            }
            
            PrepareData(defaultData);
            
            return (T) _saveDataMap[typeof(T)];
        }

        public void SaveDataImmediately<T>(T value) where T : ISavable
        {
            _saveDataMap[typeof(T)] = value;
            
            value.LastTimeSavedUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            
            dataStorage.Save(value);
        }
        
        public void SaveAllDataImmediately()
        {
            foreach (var value in _saveDataMap.Values)
            {
                ((ISavable)value).LastTimeSavedUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            }
            
            dataStorage.Save(_saveDataMap.Values.ToArray());
            
            Debug.Log("Saved all data");
        }
        
        public bool DeleteData<T>() where T : ISavable
        {
            dataStorage.Delete<T>();
            _saveDataMap.TryRemove(typeof(T), out var value);
            ((ISavable)value).SetDefaultValues();
            return _saveDataMap.TryRemove(typeof(T), out _);
        }

        public void DeleteAllData()
        {
            dataStorage.Delete(_saveDataMap.Values.ToArray());
            
            foreach (var value in _saveDataMap.Values)
            {
                ((ISavable)value).SetDefaultValues();
            }
            
            _saveDataMap.Clear();
            
            Debug.Log("Deleted all data");
        }
        
        public void ChangeAutoSaveState(bool state)
        {
            if(isAllowAutoSave == state) return;
            
            isAllowAutoSave = state;
            
            if (_autoSaveCTS != null)
            {
                DisposeAutoSaveCTS();
            }
            
            _autoSaveTask = default;

            if (isAllowAutoSave)
            {
                _autoSaveCTS = new CancellationTokenSource();
                _autoSaveTask = AutoSaveTask(_autoSaveCTS.Token);
            }
        }

        public void ChangeAutoSaveTimeout(int seconds)
        {
            autoSaveTimeoutInSeconds = seconds;
            
            if (_autoSaveCTS != null)
            {
                DisposeAutoSaveCTS();
            }
            
            _autoSaveTask = default;

            if (isAllowAutoSave)
            {
                _autoSaveCTS = new CancellationTokenSource();
                _autoSaveTask = AutoSaveTask(_autoSaveCTS.Token);
            }
        }

        private void PrepareData<T>(T defaultData = default) where T : ISavable, new()
        {
            var loadedData = dataStorage.Load<T>();
            
            if (loadedData != null)
            {
                _saveDataMap[typeof(T)] = loadedData;
            }
            else
            {
                T defaultValue = new T();
                defaultValue.SetDefaultValues(defaultData);
                _saveDataMap[typeof(T)] = defaultValue;
            }
        }

        private async UniTask AutoSaveTask(CancellationToken cancellationToken)
        {
            while (isAllowAutoSave)
            {
                await UniTask.WaitForSeconds(autoSaveTimeoutInSeconds, cancellationToken: cancellationToken);
                SaveAllDataImmediately();
            }
        }

        private void DisposeAutoSaveCTS()
        {
            _autoSaveCTS.Cancel();
            _autoSaveCTS.Dispose();
            _autoSaveCTS = null;
        }
        

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                DisposeAutoSaveCTS();
                SaveAllDataImmediately();
            }
            else
            {
                _autoSaveCTS = new CancellationTokenSource();
                _autoSaveTask = AutoSaveTask(_autoSaveCTS.Token);
            }
        }

        private void OnDestroy()
        {
            DisposeAutoSaveCTS();
            SaveAllDataImmediately();
        }
    }
}
