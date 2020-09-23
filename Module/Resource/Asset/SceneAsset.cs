using System.IO;
using UnityEngine.SceneManagement;

namespace Framework.Module.Resource
{
    public class SceneAsset : Asset
    {
        protected readonly LoadSceneMode loadSceneMode;
        protected readonly string sceneName;
        public string assetBundleName;
        protected Bundle bundle;
        protected IBundleLoader bundleLoader;

        public SceneAsset(string path, bool addictive, IBundleLoader bundleLoader)
        {
            Name = path;
            sceneName = Path.GetFileNameWithoutExtension(Name);
            loadSceneMode = addictive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            this.bundleLoader = bundleLoader;
        }

        public override float Progress
        {
            get { return 1; }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = bundleLoader.Load(assetBundleName);
                if (bundle != null)
                    SceneManager.LoadScene(sceneName, loadSceneMode);
            }
            else
            {
                SceneManager.LoadScene(sceneName, loadSceneMode);
            }
        }

        internal override void Unload()
        {
            if (bundle != null)
                bundle.Release();

            if (SceneManager.GetSceneByName(sceneName).isLoaded)
                SceneManager.UnloadSceneAsync(sceneName);

            bundle = null;
        }
    }
}