//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEditor;
//using UnityEngine;
//using Object = UnityEngine.Object;

//namespace Framework.Module.Resource
//{
//    public class ResourceManager : ModuleBase, IResourceManager
//    {
//        public override int Priority => -100;
//        IBundleLoader bundleLoader;
//        IAssetLoader assetLoader;
//        readonly string nameMappingPath = "Assets/Resources/GameSetting/AssetsNameMapping.asset";
//        Dictionary<string, string> assetsNamePathMapping;

//        public ResourceManager()
//        {
//            SetBundleLoader(new BundleLoader());
//            SetAssetLoader(new AssetLoader());
//        }

//        public void SetBundleLoader(IBundleLoader bundleLoader)
//        {
//            this.bundleLoader = bundleLoader;
//        }

//        public void SetAssetLoader(IAssetLoader assetLoader)
//        {
//            this.assetLoader = assetLoader;
//        }

//        public override void OnLoad()
//        {
//#if UNITY_EDITOR
//            assetsNamePathMapping = AssetDatabase.LoadAssetAtPath<AssetsNameMapping>(nameMappingPath).mapping;
//            base.OnLoad();
//#else
//            if (bundleLoader == null)
//            {
//                Debug.LogError("请先设置BundleLoader");
//                return;
//            }
//            assetLoader.Initialize(() => 
//            {
//                assetsNamePathMapping = (assetLoader.Load(nameMappingPath, typeof(ScriptableObject)).asset as AssetsNameMapping).mapping;
//                base.OnLoad(); 
//            }, 
//            (error)=> 
//            {
//                Debug.Log($"AssetLoader初始化失败:{error}");
//            }, 
//            bundleLoader);
//#endif
//        }

//        public override void OnUpdate()
//        {
//            this.bundleLoader?.Update();
//            this.assetLoader?.Update();
//        }

//        public IAsset LoadSync<T>(string assetName) where T : Object
//        {
//            if (!assetsNamePathMapping.ContainsKey(assetName))
//            {
//                Debug.LogError($"没有找到这个名字的资源:{assetName}, 请检查是否生成资源名字路径对应表！！！");
//                return null;
//            }

//#if UNITY_EDITOR
//            var asset = new Asset()
//            {
//                asset = AssetDatabase.LoadAssetAtPath<T>(assetsNamePathMapping[assetName])
//            };
//#else
//            var asset = assetLoader.Load(assetsNamePathMapping[assetName], typeof(T));
//#endif
//            return asset;
//        }

//        public async Task<IAsset> LoadAsync<T>(string assetName) where T : Object
//        {
//            if (!assetsNamePathMapping.ContainsKey(assetName))
//            {
//                Debug.LogError($"没有找到这个名字的资源:{assetName}, 请检查是否生成资源名字路径对应表！！！");
//                return null;
//            }

//#if UNITY_EDITOR
//            var asset = new Asset()
//            {
//                asset = AssetDatabase.LoadAssetAtPath<T>(assetsNamePathMapping[assetName])
//            };
//            return asset;
//#else
//            return await Task<T>.Run(() =>
//            {
//                var signal = new ManualResetEvent(false);
//                var asset = assetLoader.LoadAsync(assetsNamePathMapping[assetName], typeof(T));
//                IAsset result = null;
//                asset.Completed += (a) =>
//                {
//                    result = a;
//                };
//                signal.WaitOne();
//                signal.Dispose();
//                return result;
//            });
//#endif  
//        }

//        public void InstantiateGameObjectWithCallback(string assetName, Action<GameObject> onInstantiated)
//        {
//            if (string.IsNullOrEmpty(assetName))
//            {
//                Debug.LogWarning("尝试实例化一个名字为空的gameObject！！！");
//                return;
//            }
//#if UNITY_EDITOR
//            var asset = new Asset()
//            {
//                asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetsNamePathMapping[assetName])
//            };
//            var obj = Object.Instantiate(asset.asset);
//            asset.Require(obj);
//            onInstantiated?.Invoke(obj as GameObject);
//#else

//            var asset = assetLoader.LoadAsync(assetsNamePathMapping[assetName], typeof(GameObject));
//            asset.Completed += (a) =>
//            {
//                var obj = Object.Instantiate(a.asset);
//                a.Require(obj);
//                onInstantiated?.Invoke(obj as GameObject);
//            };
//#endif
//        }
//    }

//}
