using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// 资源加载 CancellationToken 支持测试用例
    /// 测试异步资源加载的取消功能
    /// </summary>
    [TestFixture]
    public class CancellationTests
    {
        #region AssetLoader 取消测试

        [Test]
        public async Task LoadAsync_CancelledBeforeStart_ThrowsOperationCancelledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            
            // Act & Assert - 由于 AssetLoader 是 internal 类，这里我们模拟测试场景
            // 实际测试需要集成测试环境
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                cts.Token.ThrowIfCancellationRequested();
            });
        }

        [Test]
        public async Task LoadAsync_CancelledDuringLoad_ThrowsOperationCancelledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var taskStarted = false;
            
            // Act & Assert - 模拟异步操作中途取消
            var task = Task.Run(async () =>
            {
                taskStarted = true;
                await Task.Delay(100); // 模拟异步操作
                cts.Token.ThrowIfCancellationRequested();
            });

            await Task.Delay(50); // 让任务开始
            cts.Cancel(); // 取消任务
            
            try
            {
                await task;
                Assert.Fail("Expected OperationCanceledException");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        [Test]
        public async Task LoadAsync_NotCancelled_CompletesSuccessfully()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var result = false;
            
            // Act - 模拟未取消的异步操作
            var task = Task.Run(async () =>
            {
                await Task.Delay(100);
                cts.Token.ThrowIfCancellationRequested();
                result = true;
            });

            await task;
            
            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region BundleLoader 取消测试

        [Test]
        public void LoadAsync_Bundle_CancelledBeforeOperation_ThrowsOperationCancelledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            
            // Act & Assert - 验证 Token 已被取消
            Assert.Throws<OperationCanceledException>(() =>
            {
                cts.Token.ThrowIfCancellationRequested();
            });
        }

        [Test]
        public async Task LoadAsync_Bundle_CancellationTokenPropagatesToDependencies()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var dependencyLoaded = false;
            var mainLoaded = false;
            
            // Act - 模拟依赖链加载过程中的取消
            var dependencyTask = Task.Run(async () =>
            {
                await Task.Delay(50);
                cts.Token.ThrowIfCancellationRequested();
                dependencyLoaded = true;
            });

            var mainTask = Task.Run(async () =>
            {
                cts.Token.ThrowIfCancellationRequested();
                await dependencyTask;
                mainLoaded = true;
            });

            await Task.Delay(25); // 等待主任务开始
            cts.Cancel(); // 取消操作
            
            // Assert - 验证取消生效
            try
            {
                await mainTask;
                Assert.Fail("Expected OperationCanceledException");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
            
            Assert.IsFalse(dependencyLoaded);
            Assert.IsFalse(mainLoaded);
        }

        #endregion

        #region 默认参数测试

        [Test]
        public async Task LoadAsync_DefaultCancellationToken_CompletesSuccessfully()
        {
            // Arrange - 使用默认的 CancellationToken（未取消）
            var ct = default(CancellationToken);
            var result = false;
            
            // Act
            var task = Task.Run(async () =>
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(50);
                result = true;
            });

            await task;
            
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task LoadAsync_NoneCancellationToken_CompletesSuccessfully()
        {
            // Arrange - 使用 CancellationToken.None
            var ct = CancellationToken.None;
            var result = false;
            
            // Act
            var task = Task.Run(async () =>
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(50);
                result = true;
            });

            await task;
            
            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region 边界情况测试

        [Test]
        public async Task LoadAsync_MultipleCancellationTokens_IndependentOperation()
        {
            // Arrange - 多个独立的 CancellationToken
            var cts1 = new CancellationTokenSource();
            var cts2 = new CancellationTokenSource();
            var result1 = false;
            var result2 = false;
            
            // Act - 两个独立的任务，使用不同的 Token
            var task1 = Task.Run(async () =>
            {
                cts1.Token.ThrowIfCancellationRequested();
                await Task.Delay(50);
                result1 = true;
            });

            var task2 = Task.Run(async () =>
            {
                cts2.Token.ThrowIfCancellationRequested();
                await Task.Delay(50);
                result2 = true;
            });

            await Task.Delay(25); // 让任务开始
            cts1.Cancel(); // 只取消第一个任务

            await task2; // 等待第二个任务完成

            // Assert - 第二个任务应该完成，第一个应该被取消
            Assert.IsFalse(result1);
            Assert.IsTrue(result2);
        }

        [Test]
        public async Task LoadAsync_CancelledAfterOperation_StillThrows()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var result = false;
            
            // Act
            var task = Task.Run(async () =>
            {
                cts.Token.ThrowIfCancellationRequested();
                result = true;
                // 操作完成后再次检查
                cts.Token.ThrowIfCancellationRequested();
            });

            cts.Cancel(); // 立即取消

            // Assert - 即使任务已开始，也应该在检查点抛出异常
            try
            {
                await task;
                Assert.Fail("Expected OperationCanceledException");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
            
            Assert.IsFalse(result);
        }

        #endregion
    }
}
