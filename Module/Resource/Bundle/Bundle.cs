using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Module.Resource
{
	public class Bundle : Asset
	{
		public readonly List<Bundle> dependencies = new List<Bundle>();

		public AssetBundle assetBundle
		{
			get { return asset as AssetBundle; }
			internal set { asset = value; }
		}

        internal override void Load()
		{
			asset = AssetBundle.LoadFromFile(Name);
			if (assetBundle == null)
				Error = Name + " LoadFromFile failed.";
		}

        internal override void Unload()
		{
			if (assetBundle == null)
				return;
			assetBundle.Unload(true);
			assetBundle = null;
		}
	}
}