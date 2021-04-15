//using ExcelToAssets;
//using Framework.Module.Resource;
//using System.Collections.Concurrent;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Framework.Module
//{
//    public class ConfigManager : ModuleBase
//    {
//        //缓存config
//        ConcurrentDictionary<string, IConfig> loadedConfigs = new ConcurrentDictionary<string, IConfig>();

//        public async Task<IConfig> GetConfig(string configName)
//        {
//            if (!loadedConfigs.ContainsKey(configName))
//            {
//                IConfig config = (await ModuleManager.GetModule<IResourceManager>().LoadAsync<ScriptableObject>(configName)) as IConfig;
//                loadedConfigs.TryAdd(configName, config);
//                return config;
//            }

//            return loadedConfigs[configName];
//        }

//        public async Task<IConfigRow> GetRow(string configName, int id)
//        {
//            IConfig config = await GetConfig(configName);

//            if (config == null)
//            {
//                return null;
//            }

//            foreach (var configRow in config.GetRows())
//            {
//                if (configRow.GetId() == id)
//                {
//                    return configRow;
//                }
//            }

//            Debug.LogWarningFormat("config {0} 中不包含这个id {1} 所属的这一行", configName, id);
//            return null;
//        }

//        public async Task<IConfigRow> GetRow<TConfig>(int id)where TConfig : IConfig
//        {
//            string configName = typeof(TConfig).Name;
//            return await GetRow(configName, id);
//        }

//        public async Task<TConfigRow> GetRow<TConfigRow, TConfig>(int id) where TConfigRow : IConfigRow where TConfig : IConfig
//        {
//            return (TConfigRow) await GetRow<TConfig>(id);
//        }

//        public async Task<TConfigRow> GetRow<TConfigRow>(string configName, int id) where TConfigRow : IConfigRow
//        {
//            return (TConfigRow)await GetRow(configName, id);
//        }
//    }
//}