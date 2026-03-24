using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.ObjectPool
{
    public interface IGameObjectPoolModule
    {
        void Push(string gameObjectName, GameObject gameObject);
        ValueTask<GameObject> Pop(string gameObjectName);
        void Release();
        void SetCheckTime(float time);
        void SetSleepTime(float time);
    }
}