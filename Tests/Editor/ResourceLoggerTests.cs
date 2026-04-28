using NUnit.Framework;
using Framework.Module.Resource;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// ResourceLogger 测试用例
    /// 注意：ResourceLogger 使用 [Conditional("DEBUG_RESOURCE")] 属性，
    /// 只有在定义了 DEBUG_RESOURCE 符号时才会实际输出日志
    /// </summary>
    [TestFixture]
    public class ResourceLoggerTests
    {
        #region 基本调用测试（确保不抛异常）

        [Test]
        public void Log_WithMessage_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.Log("Test message"));
        }

        [Test]
        public void LogLoad_WithAssetName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogLoad("Assets/Test.prefab"));
        }

        [Test]
        public void LogLoadAsync_WithAssetName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogLoadAsync("Assets/Test.prefab"));
        }

        [Test]
        public void LogRelease_WithAssetName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogRelease("Assets/Test.prefab"));
        }

        [Test]
        public void LogCollect_WithNodeCount_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogCollect(100));
        }

        [Test]
        public void LogDelete_WithWhiteCount_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogDelete(50));
        }

        [Test]
        public void LogInstantiate_WithAssetName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogInstantiate("Assets/Prefabs/Player.prefab"));
        }

        [Test]
        public void LogDestroy_WithAssetName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogDestroy("Player(Clone)"));
        }

        [Test]
        public void LogBundleLoad_WithBundleName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogBundleLoad("prefabs"));
        }

        [Test]
        public void LogAlloc_WithOwnerAndTarget_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogAlloc("BundleLoader", "prefabs"));
        }

        [Test]
        public void LogWarning_WithMessage_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogWarning("Warning message"));
        }

        [Test]
        public void LogError_WithMessage_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogError("Error message"));
        }

        #endregion

        #region 边界情况测试

        [Test]
        public void Log_WithNullMessage_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.Log(null));
        }

        [Test]
        public void Log_WithEmptyMessage_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.Log(""));
        }

        [Test]
        public void LogLoad_WithNullAssetName_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogLoad(null));
        }

        [Test]
        public void LogCollect_WithZeroCount_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogCollect(0));
        }

        [Test]
        public void LogCollect_WithNegativeCount_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogCollect(-1));
        }

        [Test]
        public void LogAlloc_WithNullParams_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.LogAlloc(null, null));
        }

        #endregion

        #region 特殊字符测试

        [Test]
        public void Log_WithSpecialCharacters_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.Log("Test with special chars: 中文 日本語 한국어 !@#$%"));
        }

        [Test]
        public void Log_WithNewlines_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ResourceLogger.Log("Line1\nLine2\rLine3"));
        }

        [Test]
        public void Log_WithLongMessage_DoesNotThrow()
        {
            var longMessage = new string('x', 10000);
            Assert.DoesNotThrow(() => ResourceLogger.Log(longMessage));
        }

        #endregion
    }
}
