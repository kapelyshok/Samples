using AtomicApps.Infrastructure.Services.SaveLoad.Data;

namespace AtomicApps.Infrastructure.Services.SaveLoad
{
    public interface ISaveService
    {
        T GetData<T>(T defaultData = default) where T : ISavable, new();
        void SaveDataImmediately<T>(T value) where T : ISavable;
        void SaveAllDataImmediately();
        bool DeleteData<T>() where T : ISavable;
        void DeleteAllData();
        void ChangeAutoSaveState(bool state);
        void ChangeAutoSaveTimeout(int seconds);
    }
}