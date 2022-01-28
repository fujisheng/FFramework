using Framework.Service.Resource;
using UnityEngine;

namespace Framework.Test
{
    public class AssetLoaderTest : UnityEngine.MonoBehaviour
    {
        AssetLoader homeViewLoader = new AssetLoader();
        AssetLoader tipsViewLoader = new AssetLoader();

        float lastCollectTime;

        private void Awake()
        {
            lastCollectTime = Time.realtimeSinceStartup;
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 30), "OpenHomeView"))
            {
                homeViewLoader.Instantiate("Assets/Data/Prefabs/HomeView.prefab");
            }
            if (GUI.Button(new Rect(100, 0, 100, 30), "OpenTipsView"))
            {
                tipsViewLoader.Instantiate("Assets/Data/Prefabs/TipsView.prefab");
            }

            if (GUI.Button(new Rect(0, 30, 100, 30), "ReleaseHomeView"))
            {
                homeViewLoader.Release();
            }

            if (GUI.Button(new Rect(100, 30, 100, 30), "ReleaseTipsView"))
            {
                tipsViewLoader.Release();
            }
        }

        private void LateUpdate()
        {
            if (Time.realtimeSinceStartup - lastCollectTime > 10f)
            {
                AssetsReferenceTree.Instance.Collect();
                AssetsReferenceTree.Instance.Delete();
                lastCollectTime = Time.realtimeSinceStartup;
            }
        }
    }
}
