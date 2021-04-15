using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Service.ObjectPool
{
    public interface IGameObjectPoolService
    {
        void Push(string gameObjectName, GameObject gameObject);
        UniTask<GameObject> Pop(string gameObjectName);
        void Release();
        void SetCheckTime(float time);
        void SetSleepTime(float time);
    }
}