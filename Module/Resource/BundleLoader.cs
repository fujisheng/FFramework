using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Framework.Module.Resource
{
	public delegate string OverrideDataPathDelegate (string bundleName);

	public class BundleLoader : IBundleLoader
	{
		readonly int MAX_LOAD_SIZE_PERFREME = 3;
		readonly List<Bundle> _bundles = new List<Bundle> ();
		readonly List<Bundle> _unusedBundles = new List<Bundle> ();
		readonly List<Bundle> _ready2Load = new List<Bundle> ();
		readonly List<Bundle> _loading = new List<Bundle> ();
		public string[] ActiveVariants { get; set; }
		string dataPath { get; set; }
		AssetBundleManifest manifest { get; set; }
		public OverrideDataPathDelegate OverrideBaseDownloadingUrl { get; set; }

        /// <summary>
        /// ��ȡbundle����������
        /// </summary>
        /// <param name="bundle">bundle��</param>
        /// <returns>����������bundle��</returns>
		public string[] GetAllDependencies (string bundle)
		{
			return manifest == null ? null : manifest.GetAllDependencies (bundle);
		}

        /// <summary>
        /// ��ʼ��loader
        /// </summary>
        /// <param name="path">���bundle��·��</param>
        /// <param name="platform">ƽ̨��</param>
        /// <param name="onSuccess">��ʼ���ɹ��Ļص�</param>
        /// <param name="onError">��ʼ��ʧ�ܵĻص�</param>
		public void Initialize (string path, string platform, Action onSuccess, Action<string> onError)
		{
			dataPath = path;
			var request = Load (platform, true, true);
			request.Completed += delegate 
            {
				if (request.Error != null)
                {
                    if (onError != null)
                    {
                        onError(request.Error);
                        return;
                    }
                }
				
				manifest = request.assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
				request.assetBundle.Unload (false);
				request.assetBundle = null;
				request.Release ();
				request = null;
                onSuccess?.Invoke();
            };
		}

        /// <summary>
        /// ͬ������һ��bundle
        /// </summary>
        /// <param name="assetBundleName">Ҫ���ص�bundle��</param>
        /// <returns></returns>
		public Bundle Load (string assetBundleName)
		{
			return Load (assetBundleName, false, false);
		}

        /// <summary>
        /// �첽����һ��bundle
        /// </summary>
        /// <param name="assetBundleName">Ҫ���ص�bundle��</param>
        /// <returns></returns>
		public Bundle LoadAsync (string assetBundleName)
		{
			return Load (assetBundleName, false, true);
		}

        /// <summary>
        /// ����bundle����������bundle��
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="assetBundleName"></param>
        /// <param name="asyncRequest"></param>
        void LoadDependencies(Bundle bundle, string assetBundleName, bool asyncRequest)
        {
            var dependencies = manifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length <= 0)
            {
                return;
            }

            for (var i = 0; i < dependencies.Length; i++)
            {
                var item = dependencies[i];
                bundle.dependencies.Add(Load(item, false, asyncRequest));
            }
        }

        /// <summary>
        /// ����һ��bundle
        /// </summary>
        /// <param name="assetBundleName">bundle��</param>
        /// <param name="isLoadingAssetBundleManifest">�Ƿ��Ѿ�������manifest</param>
        /// <param name="asyncMode">�Ƿ����첽����</param>
        /// <returns></returns>
        Bundle Load(string assetBundleName, bool isLoadingAssetBundleManifest, bool asyncMode)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                Debug.LogError("assetBundleName == null");
                return null;
            }

            if (!isLoadingAssetBundleManifest)
            {
                if (manifest == null)
                {
                    Debug.LogError("Please initialize AssetBundleManifest by calling Bundles.Initialize()");
                    return null;
                }

                assetBundleName = RemapVariantName(assetBundleName);
            }

            var url = GetDataPath(assetBundleName) + assetBundleName;
            for (int i = 0, max = _bundles.Count; i < max; i++)
            {
                var item = _bundles[i];
                if (!item.Name.Equals(url))
                {
                    continue;
                }
                    
                item.Retain();
                return item;
            }

            Bundle bundle;
            if (url.StartsWith("http://", StringComparison.Ordinal) || url.StartsWith("https://", StringComparison.Ordinal) || url.StartsWith("file://", StringComparison.Ordinal) || url.StartsWith("ftp://", StringComparison.Ordinal))
            {
                bundle = new WebBundle
                {
                    hash = manifest != null ? manifest.GetAssetBundleHash(assetBundleName) : new Hash128(),
                    cache = !isLoadingAssetBundleManifest
                };
            } 
            else
            {
                bundle = asyncMode ? new BundleAsync() : new Bundle();
            }
                
            bundle.Name = url;
            _bundles.Add(bundle);
            if (MAX_LOAD_SIZE_PERFREME > 0 && (bundle is BundleAsync || bundle is WebBundle))
            {
                _ready2Load.Add(bundle);
            }
            else
            {
                bundle.Load();
            }

            if (!isLoadingAssetBundleManifest)
            {
                LoadDependencies(bundle, assetBundleName, asyncMode);
            }
                
            bundle.Retain();
            Log("Load->" + url);
            return bundle;
        }

        string RemapVariantName(string assetBundleName)
        {
            var bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();
            var baseName = assetBundleName.Split('.')[0];
            var bestFit = int.MaxValue;
            var bestFitIndex = -1;

            for (var i = 0; i < bundlesWithVariant.Length; i++)
            {
                var curSplit = bundlesWithVariant[i].Split('.');
                var curBaseName = curSplit[0];
                var curVariant = curSplit[1];

                if (curBaseName != baseName)
                {
                    continue;
                }  

                var found = Array.IndexOf(ActiveVariants, curVariant);

                if (found == -1)
                {
                    found = int.MaxValue - 1;
                }   

                if (found >= bestFit)
                {
                    continue;
                }
                    
                bestFit = found;
                bestFitIndex = i;
            }

            if (bestFit == int.MaxValue - 1)
            {
                Debug.LogWarning(
                    "Ambiguous asset bundle variant chosen because there was no matching active variant: " +
                    bundlesWithVariant[bestFitIndex]);
            }
                
            return bestFitIndex != -1 ? bundlesWithVariant[bestFitIndex] : assetBundleName;
        }

        string GetDataPath(string bundleName)
        {
            if (OverrideBaseDownloadingUrl == null)
            {
                return dataPath;
            }
                
            foreach (var @delegate in OverrideBaseDownloadingUrl.GetInvocationList())
            {
                var method = (OverrideDataPathDelegate)@delegate;
                var res = method(bundleName);
                if (res != null)
                {
                    return res;
                }  
            }

            return dataPath;
        }

        /// <summary>
        /// ж��һ��bundle
        /// </summary>
        /// <param name="bundle">Ҫж�ص�bundle</param>
        public void Unload (Bundle bundle)
		{
			bundle.Release ();
			for (var i = 0; i < _unusedBundles.Count; i++)
            {
				var item = _unusedBundles [i];
				if (!item.Name.Equals(bundle.Name))
                {
                    continue;
                }
				item.Unload ();
				_unusedBundles.RemoveAt (i);
				return;
			}
		}

        /// <summary>
        /// ж��һ��bundle
        /// </summary>
        /// <param name="assetBundleName">Ҫж�ص�bundle��</param>
		public void Unload (string assetBundleName)
		{
			for (int i = 0, max = _bundles.Count; i < max; i++)
            {
				var item = _bundles [i];
				if (!item.Name.Equals(assetBundleName))
                {
                    continue;
                }
				Unload (item);
				break;
			}
		}

        /// <summary>
        /// ж��������bundle
        /// </summary>
        /// <param name="bundle">Ҫж��������bundle</param>
		void UnloadDependencies (Bundle bundle)
		{
			for (var i = 0; i < bundle.dependencies.Count; i++)
            {
				var item = bundle.dependencies [i];
				item.Release ();
			}

			bundle.dependencies.Clear ();
		}

		public void Update ()
		{
			if (MAX_LOAD_SIZE_PERFREME > 0)
            {
				if (_ready2Load.Count > 0 && _loading.Count < MAX_LOAD_SIZE_PERFREME)
                {
					for (int i = 0; i < Math.Min(MAX_LOAD_SIZE_PERFREME - _loading.Count, _ready2Load.Count); i++)
                    {
						var item = _ready2Load [i];
						if (item.LoadState == LoadState.Init)
                        {
							item.Load();
							_loading.Add(item);
							_ready2Load.RemoveAt (i);
							i--;
						}
					}
				}

				for (int i = 0; i < _loading.Count; i++)
                {
					var item = _loading [i];
					if (item.LoadState == LoadState.Loaded || item.LoadState == LoadState.Unload)
                    {
						_loading.RemoveAt (i);
						i--;
					}
				}
			}

			for (var i = 0; i < _bundles.Count; i++)
            {
				var item = _bundles [i];
				if (item.Update () || !item.IsUnused)
                {
                    continue;
                }
				_unusedBundles.Add (item);
				UnloadDependencies (item);
				_bundles.RemoveAt (i);
				i--;
			}

			for (var i = 0; i < _unusedBundles.Count; i++)
            {
				var item = _unusedBundles [i];
				item.Unload ();
				Log ("Unload->" + item.Name);
			}

			_unusedBundles.Clear ();
		}

        [Conditional("LOG_ENABLE")]
        void Log(string s)
        {
            Debug.Log("[Bundles]" + s);
        }
    }
}