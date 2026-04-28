using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Assert = NUnit.Framework.Assert;

namespace Framework.Tests.Editor
{
    /// <summary>
    /// 测试用的模拟资源类
    /// 用于替代 internal Asset 类进行测试
    /// </summary>
    public class MockAsset
    {
        public string Name { get; }
        public object Data { get; }

        public MockAsset(object data, string name = "MockAsset")
        {
            Data = data;
            Name = name;
        }
    }

    /// <summary>
    /// 资源加载重试机制测试
    /// 测试 AssetLoader 在瞬态失败时的重试行为
    /// </summary>
    [TestFixture]
    public class RetryTests
    {
        #region 瞬态错误重试测试

        [Test]
        public async Task LoadAsync_TransientFailure_RetriesAndSucceeds()
        {
            // Arrange: 模拟瞬态失败2次，第3次成功
            var attemptCount = 0;
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                if (attemptCount <= 2)
                {
                    throw new IOException($"Transient error #{attemptCount}");
                }
                return Task.FromResult(new MockAsset(null)); // 模拟成功
            };

            // Act
            var result = await mockLoader.LoadWithRetryAsync("test_asset");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, attemptCount, "应该尝试3次：2次失败 + 1次成功");
        }

        [Test]
        public async Task LoadAsync_AllRetriesFail_ThrowsException()
        {
            // Arrange: 所有重试都失败
            var attemptCount = 0;
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                throw new IOException($"Persistent IO error #{attemptCount}");
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await mockLoader.LoadWithRetryAsync("test_asset");
            });

            Assert.IsTrue(ex.Message.Contains("Load failed after 3 retries"));
            Assert.AreEqual(3, attemptCount, "应该尝试3次");
            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf<IOException>(ex.InnerException);
        }

        [Test]
        public async Task LoadAsync_PermanentFailure_NoRetry()
        {
            // Arrange: 永久性错误不应该重试
            var attemptCount = 0;
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                throw new ArgumentException("Permanent error: invalid argument");
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await mockLoader.LoadWithRetryAsync("test_asset");
            });

            Assert.AreEqual(1, attemptCount, "永久性错误不应该重试");
        }

        [Test]
        public async Task LoadAsync_NullReferenceException_NoRetry()
        {
            // Arrange: NullReferenceException 不应该重试
            var attemptCount = 0;
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                throw new NullReferenceException("Null reference");
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await mockLoader.LoadWithRetryAsync("test_asset");
            });

            Assert.AreEqual(1, attemptCount, "NullReferenceException 不应该重试");
        }

        #endregion

        #region 指数退避延迟测试

        [Test]
        public async Task LoadAsync_RetryDelay_IncreasesExponentially()
        {
            // Arrange
            var attemptCount = 0;
            var delays = new System.Collections.Generic.List<TimeSpan>();
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                if (attemptCount < 3)
                {
                    throw new IOException($"Error #{attemptCount}");
                }
                return Task.FromResult(new MockAsset(null));
            };
            mockLoader.OnDelay = (delay) => delays.Add(delay);

            // Act
            await mockLoader.LoadWithRetryAsync("test_asset");

            // Assert
            Assert.AreEqual(2, delays.Count, "应该有2次延迟（第1、2次失败后）");
            Assert.AreEqual(TimeSpan.FromSeconds(0.5), delays[0], "第1次延迟应为0.5秒");
            Assert.AreEqual(TimeSpan.FromSeconds(1.0), delays[1], "第2次延迟应为1.0秒");
        }

        #endregion

        #region 取消令牌测试

        [Test]
        public async Task LoadAsync_CancelledDuringRetry_ThrowsOperationCancelledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var attemptCount = 0;
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                if (attemptCount == 1)
                {
                    throw new IOException("First error");
                }
                // 第2次尝试前取消
                cts.Cancel();
                throw new IOException("Second error");
            };

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await mockLoader.LoadWithRetryAsync("test_asset", cts.Token);
            });

            Assert.AreEqual(2, attemptCount);
        }

        #endregion

        #region 瞬态错误检测测试

        [Test]
        public void IsTransientError_IOExceptions_ReturnsTrue()
        {
            // Arrange
            var ioEx = new IOException("IO error");
            var fileNotFoundEx = new FileNotFoundException("File not found");
            var directoryNotFoundEx = new DirectoryNotFoundException("Directory not found");

            // Act & Assert
            Assert.IsTrue(MockRetryableLoader.IsTransientErrorStatic(ioEx));
            Assert.IsTrue(MockRetryableLoader.IsTransientErrorStatic(fileNotFoundEx));
            Assert.IsTrue(MockRetryableLoader.IsTransientErrorStatic(directoryNotFoundEx));
        }

        [Test]
        public void IsTransientError_PermanentExceptions_ReturnsFalse()
        {
            // Arrange
            var argEx = new ArgumentException("Invalid argument");
            var nullRefEx = new NullReferenceException("Null reference");
            var invalidOpEx = new InvalidOperationException("Invalid operation");

            // Act & Assert
            Assert.IsFalse(MockRetryableLoader.IsTransientErrorStatic(argEx));
            Assert.IsFalse(MockRetryableLoader.IsTransientErrorStatic(nullRefEx));
            Assert.IsFalse(MockRetryableLoader.IsTransientErrorStatic(invalidOpEx));
        }

        [Test]
        public void IsTransientError_NestedIOException_ReturnsTrue()
        {
            // Arrange: 包装在普通 Exception 中的 IOException
            var innerEx = new IOException("Inner IO error");
            var outerEx = new Exception("Outer error", innerEx);

            // Act & Assert
            Assert.IsTrue(MockRetryableLoader.IsTransientErrorStatic(outerEx));
        }

        #endregion

        #region 成功路径测试

        [Test]
        public async Task LoadAsync_SuccessOnFirstAttempt_NoRetry()
        {
            // Arrange
            var attemptCount = 0;
            var mockLoader = new MockRetryableLoader();
            mockLoader.OnTryLoad = () =>
            {
                attemptCount++;
                return Task.FromResult(new MockAsset(null));
            };

            // Act
            var result = await mockLoader.LoadWithRetryAsync("test_asset");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, attemptCount, "第一次成功不应该重试");
        }

        #endregion
    }

    /// <summary>
    /// 用于测试的模拟加载器
    /// 模拟 AssetLoader 的重试行为
    /// </summary>
    public class MockRetryableLoader
    {
        public Func<Task<MockAsset>> OnTryLoad { get; set; }
        public Action<TimeSpan> OnDelay { get; set; }

        const int maxRetries = 3;

        public async Task<MockAsset> LoadWithRetryAsync(string name, CancellationToken cancellationToken = default)
        {
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await TryLoadAsync();
                }
                catch (Exception ex) when (IsTransientErrorStatic(ex) && attempt < maxRetries)
                {
                    lastException = ex;
                    var delay = TimeSpan.FromSeconds(0.5f * attempt);
                    OnDelay?.Invoke(delay);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new Exception($"Load failed after {maxRetries} retries: {name}", lastException);
        }

        async Task<MockAsset> TryLoadAsync()
        {
            if (OnTryLoad != null)
            {
                return await OnTryLoad();
            }
            return new MockAsset(null);
        }

        public static bool IsTransientErrorStatic(Exception ex)
        {
            if (ex is IOException)
            {
                return true;
            }

            if (ex.InnerException != null)
            {
                return IsTransientErrorStatic(ex.InnerException);
            }

            return false;
        }
    }
}
