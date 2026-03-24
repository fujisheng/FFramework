using Framework.IoC;

using System;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;

namespace Framework.Module.Archive
{
    internal sealed class ArchiveModule : Module, IArchiveModule
    {
        ISerializer serializer;
        IEncryptionProvider encryptor;
        ICompressionProvider compressor;
        string saveFolder;

        public ArchiveModule()
        {
            this.saveFolder = "Saves";
        }

        [Inject]
        public void SetSerializer(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        [Inject]
        public void SetEncryptor(IEncryptionProvider encryptor)
        {
            this.encryptor = encryptor;
        }

        [Inject]
        public void SetCompressor(ICompressionProvider compressor)
        {
            this.compressor = compressor;
        }

        public void SetSaveFolder(string folder)
        {
            this.saveFolder = folder;
        }

        public async ValueTask SaveAsync<T>(string saveKey, T data)
        {
            await SaveInternalAsync(saveKey, data);
        }

        public void Save<T>(string saveKey, T data)
        {
            SaveInternal(saveKey, data);
        }

        public async ValueTask SaveAsync(string saveKey, object data)
        {
            await SaveInternalAsync(saveKey, data);
        }

        public void Save(string saveKey, object data)
        {
            SaveInternal(saveKey, data);
        }

        public async ValueTask<T> LoadAsync<T>(string saveKey)
        {
            return await LoadInternalAsync<T>(saveKey);
        }

        public T Load<T>(string saveKey)
        {
            return LoadInternal<T>(saveKey);
        }

        public async ValueTask<object> LoadAsync(string saveKey, Type type)
        {
            return await LoadInternalAsync(saveKey, type);
        }

        public object Load(string saveKey, Type type)
        {
            return LoadInternal(saveKey, type);
        }

        async ValueTask SaveInternalAsync(string saveKey, object data)
        {
            var filePath = GetFilePath(saveKey);
            EnsureDirectoryExists(filePath);

            var processedData = await ProcessSave(data);
            await File.WriteAllBytesAsync(filePath, processedData);
        }

        void SaveInternal(string saveKey, object data)
        {
            var filePath = GetFilePath(saveKey);
            EnsureDirectoryExists(filePath);

            var processedData = ProcessSave(data).Result;
            File.WriteAllBytes(filePath, processedData);
        }

        async ValueTask<T> LoadInternalAsync<T>(string saveKey)
        {
            var filePath = GetFilePath(saveKey);
            if (!File.Exists(filePath))
            {
                return default;
            }

            var fileData = await File.ReadAllBytesAsync(filePath);
            return ProcessLoad<T>(fileData);
        }

        T LoadInternal<T>(string saveKey)
        {
            var filePath = GetFilePath(saveKey);
            if (!File.Exists(filePath))
            {
                return default;
            }

            var fileData = File.ReadAllBytes(filePath);
            return ProcessLoad<T>(fileData);
        }

        async ValueTask<object> LoadInternalAsync(string saveKey, Type type)
        {
            var filePath = GetFilePath(saveKey);
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileData = await File.ReadAllBytesAsync(filePath);
            return ProcessLoad(fileData, type);
        }

        object LoadInternal(string saveKey, Type type)
        {
            var filePath = GetFilePath(saveKey);
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileData = File.ReadAllBytes(filePath);
            return ProcessLoad(fileData, type);
        }

        public bool Exists(string saveKey)
        {
            var filePath = GetFilePath(saveKey);
            return File.Exists(filePath);
        }

        public void Delete(string saveKey)
        {
            var filePath = GetFilePath(saveKey);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        async ValueTask<byte[]> ProcessSave<T>(T data)
        {
            return await Task.Run(() =>
            {
                var serialized = serializer.Serialize(data);
                var compressed = compressor.Compress(serialized);
                return encryptor.Encrypt(compressed);
            });
        }

        T ProcessLoad<T>(byte[] data)
        {
            var decrypted = encryptor.Decrypt(data);
            var decompressed = compressor.Decompress(decrypted);
            return serializer.Deserialize<T>(decompressed);
        }

        object ProcessLoad(byte[] data, Type type)
        {
            var decrypted = encryptor.Decrypt(data);
            var decompressed = compressor.Decompress(decrypted);
            return serializer.Deserialize(decompressed, type);
        }

        string GetFilePath(string saveKey)
        {
            return Path.Combine(GetPlatformSavePath(), $"{saveKey}.save");
        }

        string GetPlatformSavePath()
        {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "..", saveFolder);
#elif UNITY_ANDROID || UNITY_IOS
            return Path.Combine(Application.persistentDataPath, saveFolder);
#else
            return Path.Combine(Application.dataPath, saveFolder);
#endif
        }

        void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}