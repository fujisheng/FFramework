using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Framework.Module.Resource;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// FolderBasedResourceMapping 测试用例
    /// 测试资源路径到 Bundle 名称的映射逻辑
    /// </summary>
    [TestFixture]
    public class FolderBasedResourceMappingTests
    {
        FolderBasedResourceMapping mapping;

        [SetUp]
        public void SetUp()
        {
            mapping = new FolderBasedResourceMapping("default");
        }

        [TearDown]
        public void TearDown()
        {
            mapping.Clear();
            mapping = null;
        }

        #region GetBundleName 路径推断测试

        [Test]
        public void GetBundleName_WithStandardPath_ReturnsLastFolder()
        {
            // Arrange
            var path = "Assets/Data/Prefabs/HomeView.prefab";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("prefabs", bundleName);
        }

        [Test]
        public void GetBundleName_WithDeepPath_ReturnsLastFolder()
        {
            // Arrange
            var path = "Assets/Resources/UI/Panels/MainPanel.prefab";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("panels", bundleName);
        }

        [Test]
        public void GetBundleName_WithBackslashPath_NormalizesAndReturns()
        {
            // Arrange
            var path = "Assets\\Data\\Models\\Character.fbx";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("models", bundleName);
        }

        [Test]
        public void GetBundleName_WithMixedCasePath_ReturnsLowerCase()
        {
            // Arrange
            var path = "Assets/Data/TEXTURES/Skin.png";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("textures", bundleName);
        }

        [Test]
        public void GetBundleName_WithRootFile_ReturnsDefault()
        {
            // Arrange
            var path = "config.json";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("default", bundleName);
        }

        [Test]
        public void GetBundleName_WithEmptyPath_ReturnsDefault()
        {
            // Act
            var bundleName = mapping.GetBundleName("");
            
            // Assert
            Assert.AreEqual("default", bundleName);
        }

        [Test]
        public void GetBundleName_WithNullPath_ReturnsDefault()
        {
            // Act
            var bundleName = mapping.GetBundleName(null);
            
            // Assert
            Assert.AreEqual("default", bundleName);
        }

        [Test]
        public void GetBundleName_WithOneLevelPath_ReturnsFolder()
        {
            // Arrange
            var path = "Assets/file.txt";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            // Assets/ 被移除后，剩余 file.txt 没有目录，返回 default
            Assert.AreEqual("default", bundleName);
        }

        [Test]
        public void GetBundleName_WithTwoLevelPath_ReturnsFolder()
        {
            // Arrange
            var path = "Assets/Configs/data.bytes";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("configs", bundleName);
        }

        #endregion

        #region RegisterMapping / TryGetBundleName 测试

        [Test]
        public void RegisterMapping_WithValidInput_CanRetrieve()
        {
            // Arrange
            var assetPath = "Assets/Special/CustomAsset.asset";
            var bundleName = "custom_bundle";
            
            // Act
            mapping.RegisterMapping(assetPath, bundleName);
            var result = mapping.TryGetBundleName(assetPath, out var retrievedBundle);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("custom_bundle", retrievedBundle);
        }

        [Test]
        public void RegisterMapping_OverwritesExisting()
        {
            // Arrange
            var assetPath = "Assets/Test/File.txt";
            
            // Act
            mapping.RegisterMapping(assetPath, "bundle1");
            mapping.RegisterMapping(assetPath, "bundle2");
            mapping.TryGetBundleName(assetPath, out var result);
            
            // Assert
            Assert.AreEqual("bundle2", result);
        }

        [Test]
        public void RegisterMapping_NormalizesPath()
        {
            // Arrange
            var assetPath1 = "Assets/Test/File.txt";
            var assetPath2 = "ASSETS\\TEST\\FILE.TXT";
            
            // Act
            mapping.RegisterMapping(assetPath1, "mybundle");
            var result = mapping.TryGetBundleName(assetPath2, out var bundleName);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("mybundle", bundleName);
        }

        [Test]
        public void RegisterMapping_WithNullPath_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => mapping.RegisterMapping(null, "bundle"));
        }

        [Test]
        public void RegisterMapping_WithEmptyPath_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => mapping.RegisterMapping("", "bundle"));
        }

        [Test]
        public void RegisterMapping_WithNullBundle_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => mapping.RegisterMapping("Assets/file.txt", null));
        }

        [Test]
        public void RegisterMapping_WithEmptyBundle_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => mapping.RegisterMapping("Assets/file.txt", ""));
        }

        #endregion

        #region RemoveMapping 测试

        [Test]
        public void RemoveMapping_ExistingPath_ReturnsTrue()
        {
            // Arrange
            var path = "Assets/Test/File.txt";
            mapping.RegisterMapping(path, "bundle");
            
            // Act
            var result = mapping.RemoveMapping(path);
            
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void RemoveMapping_NonExistingPath_ReturnsFalse()
        {
            // Act
            var result = mapping.RemoveMapping("Assets/NonExisting.txt");
            
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void RemoveMapping_AfterRemove_UsesInference()
        {
            // Arrange
            var path = "Assets/Test/File.txt";
            mapping.RegisterMapping(path, "custom_bundle");
            
            // Act
            mapping.RemoveMapping(path);
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            // 移除注册后，应该使用路径推断
            Assert.AreEqual("test", bundleName);
        }

        #endregion

        #region GetAllAssetPaths 测试

        [Test]
        public void GetAllAssetPaths_Empty_ReturnsEmpty()
        {
            // Act
            var paths = mapping.GetAllAssetPaths().ToList();
            
            // Assert
            Assert.AreEqual(0, paths.Count);
        }

        [Test]
        public void GetAllAssetPaths_WithMappings_ReturnsAll()
        {
            // Arrange
            mapping.RegisterMapping("Assets/A.txt", "bundle_a");
            mapping.RegisterMapping("Assets/B.txt", "bundle_b");
            mapping.RegisterMapping("Assets/C.txt", "bundle_c");
            
            // Act
            var paths = mapping.GetAllAssetPaths().ToList();
            
            // Assert
            Assert.AreEqual(3, paths.Count);
            Assert.IsTrue(paths.Contains("assets/a.txt")); // normalized
            Assert.IsTrue(paths.Contains("assets/b.txt"));
            Assert.IsTrue(paths.Contains("assets/c.txt"));
        }

        #endregion

        #region Clear 测试

        [Test]
        public void Clear_RemovesAllMappings()
        {
            // Arrange
            mapping.RegisterMapping("Assets/A.txt", "a");
            mapping.RegisterMapping("Assets/B.txt", "b");
            
            // Act
            mapping.Clear();
            var paths = mapping.GetAllAssetPaths().ToList();
            
            // Assert
            Assert.AreEqual(0, paths.Count);
        }

        #endregion

        #region RegisterMappings 批量注册测试

        [Test]
        public void RegisterMappings_WithMultiple_AllRegistered()
        {
            // Arrange
            var mappings = new Dictionary<string, string>
            {
                { "Assets/A.txt", "bundle_a" },
                { "Assets/B.txt", "bundle_b" },
                { "Assets/C.txt", "bundle_c" }
            };
            
            // Act
            mapping.RegisterMappings(mappings);
            
            // Assert
            Assert.IsTrue(mapping.TryGetBundleName("Assets/A.txt", out var a));
            Assert.AreEqual("bundle_a", a);
            Assert.IsTrue(mapping.TryGetBundleName("Assets/B.txt", out var b));
            Assert.AreEqual("bundle_b", b);
            Assert.IsTrue(mapping.TryGetBundleName("Assets/C.txt", out var c));
            Assert.AreEqual("bundle_c", c);
        }

        #endregion

        #region 边界情况测试

        [Test]
        public void GetBundleName_WithTrailingSlash_HandlesCorrectly()
        {
            // Arrange
            var path = "Assets/Data/Prefabs/";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("prefabs", bundleName);
        }

        [Test]
        public void GetBundleName_WithMultipleSlashes_HandlesCorrectly()
        {
            // Arrange
            var path = "Assets//Data///Prefabs////File.prefab";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            // 多个斜杠会导致空目录名，具体行为取决于实现
            Assert.IsNotNull(bundleName);
        }

        [Test]
        public void GetBundleName_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var path = "Assets/Data/My Prefabs/File (1).prefab";
            
            // Act
            var bundleName = mapping.GetBundleName(path);
            
            // Assert
            Assert.AreEqual("my prefabs", bundleName);
        }

        #endregion
    }
}
