using AtomicApps.Infrastructure.Services.SaveLoad.Data;

namespace AtomicApps.Infrastructure.Services.SaveLoad.Storages
{
    public interface IDataStorage
    {
        T Load<T>() where T : ISavable, new();
        void Save<T>(T saveData, bool isPrintLog = false) where T : ISavable;
        void Save(object[] saveData, bool isPrintLog = false);
        void Delete<T>() where T : ISavable;
        void Delete(object[] saveData);
    }
}