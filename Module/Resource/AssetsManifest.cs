using System;
using UnityEngine;

namespace Framework.Module.Resource
{
    [Serializable]
    public class AssetData
    {
        public int variant;
        public int bundle;
        public int dir;
        public string name;
    }

    public class AssetsManifest : ScriptableObject
    {
        public string downloadURL = "";
        public string[] activeVariants = new string[0];
        [HideInInspector]public string[] bundles = new string[0];
        [HideInInspector]public string[] dirs = new string[0];
        [HideInInspector]public AssetData[] assets = new AssetData[0];
    }
}
