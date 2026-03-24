using UnityEngine.Networking;

namespace Framework.Module.WebRequst
{
    internal class WebRequestModule : Module
    {
        public void Put(string url, byte[] data)
        {
            UnityWebRequest.Put(url, data);
        }
    }
}
