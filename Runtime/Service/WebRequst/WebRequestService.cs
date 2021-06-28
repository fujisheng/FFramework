using UnityEngine.Networking;

namespace Framework.Service.WebRequst
{
    internal class WebRequestService : Service
    {
        public void Put(string url, byte[] data)
        {
            UnityWebRequest.Put(url, data);
        }
    }
}
