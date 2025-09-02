using AtomicApps.Infrastructure.Services.SaveLoad.Data;
using UnityEngine;

namespace AtomicApps.Infrastructure.Services.SaveLoad.Storages
{
    public abstract class SaveDataStorage : MonoBehaviour, IDataStorage
    {
        public abstract T Load<T>() where T : ISavable, new();
        public abstract void Save<T>(T saveData, bool isPrintLog = false) where T : ISavable;
        public abstract void Save(object[] saveData, bool isPrintLog = false);
        public abstract void Delete<T>() where T : ISavable;
        public abstract void Delete(object[] saveData);
    }
}
