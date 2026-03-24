using System.Collections.Generic;

namespace Framework.Module.Resource
{
    /// <summary>
    /// 资源映射配置接口
    /// 定义资源路径到Bundle名称的映射
    /// </summary>
    public interface IResourceMapping
    {
        /// <summary>
        /// 获取资源对应的Bundle名称
        /// </summary>        /// <param name="assetPath">资源路径</param>
        /// <returns>Bundle名称，如果未找到返回null</returns>
        string GetBundleName(string assetPath);

        /// <summary>
        /// 尝试获取资源对应的Bundle名称
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="bundleName">输出的Bundle名称</param>
        /// <returns>是否找到映射</returns>
        bool TryGetBundleName(string assetPath, out string bundleName);

        /// <summary>
        /// 注册资源映射
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="bundleName">Bundle名称</param>
        void RegisterMapping(string assetPath, string bundleName);

        /// <summary>
        /// 移除资源映射
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>是否成功移除</returns>
        bool RemoveMapping(string assetPath);

        /// <summary>
        /// 获取所有已注册的资源路径
        /// </summary>
        /// <returns>资源路径枚举</returns>
        IEnumerable<string> GetAllAssetPaths();

        /// <summary>
        /// 清除所有映射
        /// </summary>
        void Clear();
    }
}
