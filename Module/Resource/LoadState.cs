using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    public enum LoadState
    {
        Init,
        LoadAssetBundle,
        LoadAsset,
        Loaded,
        Unload,
    }
}