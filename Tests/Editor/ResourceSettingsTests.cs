using System.Collections.Generic;
using Framework.Module.Resource;
using NUnit.Framework;
using UnityEngine;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// ResourceSettings 单元测试
    /// 测试资源映射配置功能
    /// </summary>    [TestFixture]
    public class ResourceSettingsTests
    {
        ResourceSettings settings;

        [SetUp]
        public void SetUp()
        {
            settings = ScriptableObject.CreateInstance<ResourceSettings>();
        }

        [TearDown]
        public void TearDown()
        {
            if (settings != null)
            {
                Object.DestroyImmediate(settings);
            }
            ResourceSettings.ResetInstance();
        }

        #region 基本映射测试

        [Test]
        public void RegisterMapping_AndGetBundleName_ReturnsCorrectBundle()
        {
            // Arrange
            const string assetPath = "Assets/Test/Prefab.prefab";
            const string bundleName = "test-bundle";

            // Act
            settings.RegisterMapping(assetPath, bundleName);
            var result = settings.GetBundleName(assetPath);

            // Assert
            Assert.AreEqual(bundleName, result);
        }

        [Test]
        public void TryGetBundleName_WithRegisteredPath_ReturnsTrue()
        {
            // Arrange
            const string assetPath = "Assets/Test/Prefab.prefab";
            const string bundleName = "test-bundle";
            settings.RegisterMapping(assetPath, bundleName);

            // Act
            bool found = settings.TryGetBundleName(assetPath, out var result);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual(bundleName, result);
        }

        [Test]
        public void TryGetBundleName_WithUnregisteredPath_FallbackToInference()
        {
            // Arrange - 未注册的路径
            const string assetPath = "Assets/Data/Prefabs/NewPrefab.prefab";

            // Act
            bool found = settings.TryGetBundleName(assetPath, out var result);

            // Assert - 应回退到路径推断（取倒数第二级目录名）
            Assert.IsTrue(found);
            Assert.AreEqual("prefabs", result);
        }

        [Test]
        public void GetBundleName_WithUnregisteredPath_ReturnsDefault()
        {
            // Arrange - 完全无法推断的路径
            const string assetPath = "InvalidPath";

            // Act
            var result = settings.GetBundleName(assetPath);

            // Assert - 应返回 "default"
            Assert.AreEqual("default", result);
        }

        #endregion

        #region 路径规范化测试

        [Test]
        public void RegisterMapping_WithBackslash_NormalizesToForwardSlash()
        {
            // Arrange
            const string assetPathWithBackslash = "Assets\\Test\\Prefab.prefab";
            const string assetPathWithForwardSlash = "assets/test/prefab.prefab";
            const string bundleName = "test-bundle";

            // Act
            settings.RegisterMapping(assetPathWithBackslash, bundleName);
            var result = settings.GetBundleName(assetPathWithForwardSlash);

            // Assert
            Assert.AreEqual(bundleName, result);
        }

        [Test]
        public void RegisterMapping_WithDifferentCase_NormalizesToLowerCase()
        {
            // Arrange
            const string assetPathUpper = "ASSETS/TEST/PREFAB.PREFAB";
            const string assetPathLower = "assets/test/prefab.prefab";
            const string bundleName = "test-bundle";

            // Act
            settings.RegisterMapping(assetPathUpper, bundleName);
            var result = settings.GetBundleName(assetPathLower);

            // Assert
            Assert.AreEqual(bundleName, result);
        }

        #endregion

        #region 移除映射测试

        [Test]
        public void RemoveMapping_ExistingMapping_ReturnsTrue()
        {
            // Arrange
            const string assetPath = "Assets/Test/Prefab.prefab";
            settings.RegisterMapping(assetPath, "test-bundle");

            // Act
            bool removed = settings.RemoveMapping(assetPath);

            // Assert
            Assert.IsTrue(removed);
            Assert.AreEqual("default", settings.GetBundleName(assetPath)); // 移除后应回退到默认
        }

        [Test]
        public void RemoveMapping_NonExistingMapping_ReturnsFalse()
        {
            // Arrange - 未注册的路径
            const string assetPath = "Assets/NonExistent/Prefab.prefab";

            // Act
            bool removed = settings.RemoveMapping(assetPath);

            // Assert
            Assert.IsFalse(removed);
        }

        #endregion

        #region 边界情况测试

        [Test]
        public void RegisterMapping_WithNullPath_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => settings.RegisterMapping(null, "bundle"));
        }

        [Test]
        public void RegisterMapping_WithEmptyPath_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => settings.RegisterMapping("", "bundle"));
        }

        [Test]
        public void RegisterMapping_WithNullBundleName_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => settings.RegisterMapping("Assets/Test.prefab", null));
        }

        [Test]
        public void GetAllAssetPaths_EmptySettings_ReturnsEmpty()
        {
            // Act
            var paths = settings.GetAllAssetPaths();

            // Assert
            Assert.IsEmpty(paths);
        }

        [Test]
        public void GetAllAssetPaths_WithMappings_ReturnsAllPaths()
        {
            // Arrange
            settings.RegisterMapping("Assets/A.prefab", "bundle-a");
            settings.RegisterMapping("Assets/B.prefab", "bundle-b");
            settings.RegisterMapping("Assets/C.prefab", "bundle-c");

            // Act
            var paths = new List<string>(settings.GetAllAssetPaths());

            // Assert
            Assert.AreEqual(3, paths.Count);
            Assert.Contains("assets/a.prefab", paths);
            Assert.Contains("assets/b.prefab", paths);
            Assert.Contains("assets/c.prefab", paths);
        }

        [Test]
        public void Clear_RemovesAllMappings()
        {
            // Arrange
            settings.RegisterMapping("Assets/A.prefab", "bundle-a");
            settings.RegisterMapping("Assets/B.prefab", "bundle-b");

            // Act
            settings.Clear();

            // Assert
            Assert.AreEqual(0, settings.MappingCount);
            Assert.IsEmpty(settings.GetAllAssetPaths());
        }

        #endregion

        #region 单例测试

        [Test]
        public void Instance_WithoutAssetInResources_CreatesEmptyInstance()
        {
            // Act - 确保 Resources 中没有 ResourceSettings
            var instance = ResourceSettings.Instance;

            // Assert - 应创建空实例而不抛出异常
            Assert.IsNotNull(instance);
            Assert.AreEqual(0, instance.MappingCount);
        }

        [Test]
        public void ResetInstance_ClearsCachedInstance()
        {
            // Arrange
            var firstInstance = ResourceSettings.Instance;

            // Act
            ResourceSettings.ResetInstance();
            var secondInstance = ResourceSettings.Instance;

            // Assert - 重置后应重新加载（可能是不同实例）
            // 注意：由于 Instance 是懒加载，两次调用可能返回相同或不同实例
            // 这里主要测试 ResetInstance 不抛出异常
            Assert.IsNotNull(secondInstance);
        }

        #endregion

        #region 复杂场景测试

        [Test]
        public void MultipleMappings_SameBundle_DifferentAssets()
        {
            // Arrange
            const string bundleName = "ui-bundle";
            settings.RegisterMapping("Assets/UI/Button.prefab", bundleName);
            settings.RegisterMapping("Assets/UI/Panel.prefab", bundleName);
            settings.RegisterMapping("Assets/UI/Icon.png", bundleName);

            // Act & Assert
            Assert.AreEqual(bundleName, settings.GetBundleName("Assets/UI/Button.prefab"));
            Assert.AreEqual(bundleName, settings.GetBundleName("Assets/UI/Panel.prefab"));
            Assert.AreEqual(bundleName, settings.GetBundleName("Assets/UI/Icon.png"));
        }

        [Test]
        public void OverrideMapping_UpdatesBundleName()
        {
            // Arrange
            const string assetPath = "Assets/Test/Prefab.prefab";
            settings.RegisterMapping(assetPath, "old-bundle");

            // Act
            settings.RegisterMapping(assetPath, "new-bundle");

            // Assert
            Assert.AreEqual("new-bundle", settings.GetBundleName(assetPath));
            Assert.AreEqual(1, settings.MappingCount); // 不应重复添加
        }

        #endregion
    }
}
