using System.Collections.Generic;
using UnityEngine;

namespace Framework.Service.Resource.Editor
{
    public class FrameworkResourceSetting : ScriptableObject
    {
        public string sourcesPath = "Assets/Sources";
        public string defaultGroupName = "Default Local Group";
        public List<string> ignoreExtensions = new List<string>
        {
            ".meta",
            ".DS_Store",
        };
    }
}
