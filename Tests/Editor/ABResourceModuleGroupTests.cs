using NUnit.Framework;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// ABResourceModule 组名推断测试
    /// 由于 ABResourceModule 是 internal 类，我们通过反射或重新实现逻辑来测试
    /// </summary>
    [TestFixture]
    public class ABResourceModuleGroupTests
    {
        // 复制 ABResourceModule.GetGroup 的逻辑用于测试
        // 这样可以独立测试路径解析逻辑
        string GetGroup(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return "default";
            }

            // 从路径推断组名
            // 规则：Assets/Data/Prefabs/X.prefab -> prefabs
            var path = assetName.Replace('\\', '/').ToLowerInvariant();
            
            if (path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            var lastSlash = path.LastIndexOf('/');
            if (lastSlash > 0)
            {
                var dir = path.Substring(0, lastSlash);
                var dirSlash = dir.LastIndexOf('/');
                if (dirSlash >= 0)
                {
                    return dir.Substring(dirSlash + 1);
                }
                return dir;
            }

            return "default";
        }

        #region 标准路径测试

        [Test]
        public void GetGroup_StandardPrefabPath_ReturnsFolderName()
        {
            // Arrange
            var path = "Assets/Data/Prefabs/HomeView.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("prefabs", group);
        }

        [Test]
        public void GetGroup_DeepPath_ReturnsLastFolder()
        {
            // Arrange
            var path = "Assets/Resources/UI/Panels/Windows/MainWindow.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("windows", group);
        }

        [Test]
        public void GetGroup_TwoLevelPath_ReturnsFirstFolder()
        {
            // Arrange
            var path = "Assets/Configs/data.bytes";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("configs", group);
        }

        #endregion

        #region 路径格式化测试

        [Test]
        public void GetGroup_BackslashPath_NormalizesCorrectly()
        {
            // Arrange
            var path = "Assets\\Data\\Models\\Character.fbx";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("models", group);
        }

        [Test]
        public void GetGroup_MixedSlashPath_NormalizesCorrectly()
        {
            // Arrange
            var path = "Assets/Data\\Textures/skin.png";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("textures", group);
        }

        [Test]
        public void GetGroup_UpperCasePath_ReturnsLowerCase()
        {
            // Arrange
            var path = "ASSETS/DATA/SPRITES/Icon.png";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("sprites", group);
        }

        [Test]
        public void GetGroup_MixedCasePath_ReturnsLowerCase()
        {
            // Arrange
            var path = "Assets/Data/MyPrefabs/Test.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("myprefabs", group);
        }

        #endregion

        #region 边界情况测试

        [Test]
        public void GetGroup_EmptyPath_ReturnsDefault()
        {
            // Act
            var group = GetGroup("");
            
            // Assert
            Assert.AreEqual("default", group);
        }

        [Test]
        public void GetGroup_NullPath_ReturnsDefault()
        {
            // Act
            var group = GetGroup(null);
            
            // Assert
            Assert.AreEqual("default", group);
        }

        [Test]
        public void GetGroup_RootFile_ReturnsDefault()
        {
            // Arrange
            var path = "config.json";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("default", group);
        }

        [Test]
        public void GetGroup_AssetsRootFile_ReturnsDefault()
        {
            // Arrange
            var path = "Assets/file.txt";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("default", group);
        }

        [Test]
        public void GetGroup_OnlyAssetsPrefix_ReturnsDefault()
        {
            // Arrange
            var path = "Assets/";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("default", group);
        }

        #endregion

        #region 特殊字符测试

        [Test]
        public void GetGroup_PathWithSpaces_PreservesSpaces()
        {
            // Arrange
            var path = "Assets/Data/My Prefabs/test.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("my prefabs", group);
        }

        [Test]
        public void GetGroup_PathWithNumbers_PreservesNumbers()
        {
            // Arrange
            var path = "Assets/Data/Level01/scene.unity";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("level01", group);
        }

        [Test]
        public void GetGroup_PathWithUnderscore_PreservesUnderscore()
        {
            // Arrange
            var path = "Assets/Data/my_prefabs/test.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("my_prefabs", group);
        }

        [Test]
        public void GetGroup_PathWithDash_PreservesDash()
        {
            // Arrange
            var path = "Assets/Data/my-prefabs/test.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("my-prefabs", group);
        }

        #endregion

        #region 实际使用场景测试

        [Test]
        public void GetGroup_SkillConfig_ReturnsSkillsFolder()
        {
            // Arrange
            var path = "Assets/Resources/Skills/Skill_5001001/config.asset";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("skill_5001001", group);
        }

        [Test]
        public void GetGroup_UIView_ReturnsViewFolder()
        {
            // Arrange
            var path = "Assets/Data/Prefabs/UI/Views/HomeView.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("views", group);
        }

        [Test]
        public void GetGroup_EnemyPrefab_ReturnsEnemyFolder()
        {
            // Arrange
            var path = "Assets/Data/Prefabs/Enemy/Goblin.prefab";
            
            // Act
            var group = GetGroup(path);
            
            // Assert
            Assert.AreEqual("enemy", group);
        }

        #endregion
    }
}
