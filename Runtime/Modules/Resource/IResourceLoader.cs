using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    public interface IResourceLoader
    {
        /// <summary>
        /// 异步获取一个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="asset">资源标识</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        ValueTask<T> GetAsync<T>(string asset, CancellationToken cancellationToken = default) where T : Object;

        /// <summary>
        /// 异步获取所有资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="group">资源组名</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        ValueTask<IList<T>> GetAllAsync<T>(string group, CancellationToken cancellationToken = default) where T : Object;

        /// <summary>
        /// 异步预载一个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源标识</param>
        /// <returns></returns>
        ValueTask Perload<T>(string asset) where T : Object;

        /// <summary>
        /// 异步预载所有资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="group">资源组</param>
        /// <returns></returns>
        ValueTask PerloadAll<T>(string group) where T : Object;

        /// <summary>
        /// 同步获取一个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="asset">资源标识</param>
        /// <returns></returns>
        T Get<T>(string asset) where T : Object;

        /// <summary>
        /// 异步实例化一个GameObject
        /// </summary>
        /// <param name="asset">资源</param>
        /// <param name="position">实例化的坐标</param>
        /// <param name="rotation">实例化的旋转</param>
        /// <param name="parent">实例化的父节点</param>
        /// <param name="trackHandle"></param>
        /// <returns></returns>
        ValueTask<GameObject> InstantiateAsync(string asset, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true);

        /// <summary>
        /// 同步实例化一个GameObject
        /// </summary>
        /// <param name="asset">资源</param>
        /// <param name="position">实例化的坐标</param>
        /// <param name="rotation">实例化的旋转</param>
        /// <param name="parent">实例化的父节点</param>
        /// <param name="trackHandle"></param>
        /// <returns></returns>
        GameObject Instantiate(string asset, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool trackHandle = true);

        /// <summary>
        /// 释放一个实例化的GameObject
        /// </summary>
        /// <param name="gameObject">GameObject实例</param>
        void ReleaseInstance(GameObject gameObject);

        /// <summary>
        /// 释放这个Loader 请在释放后将其置空
        /// </summary>
        void Release();
    }
}