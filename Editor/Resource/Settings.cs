using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Module.Resource.Editor
{
    public class Settings : ScriptableObject
    {
        public bool runtimeMode = true;
        public string assetRootPath = "Assets/Sources";
    }
}