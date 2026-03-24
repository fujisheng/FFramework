using System;
using System.Threading.Tasks;

namespace Framework.Module.Archive
{
    public interface IArchiveModule
    {
        ValueTask SaveAsync<T>(string saveKey, T data);
        void Save<T>(string saveKey, T data);
        ValueTask SaveAsync(string saveKey, object data);
        void Save(string saveKey, object data);

        ValueTask<T> LoadAsync<T>(string saveKey);
        T Load<T>(string saveKey);
        ValueTask<object> LoadAsync(string saveKey, Type type);
        object Load(string saveKey, Type type);

        bool Exists(string saveKey);
        void Delete(string saveKey);
    }
}