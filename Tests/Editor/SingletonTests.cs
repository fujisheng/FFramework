using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// Singleton 单例模式测试用例
    /// 测试线程安全性和单例保证
    /// </summary>
    [TestFixture]
    public class SingletonTests
    {
        // 测试用单例类
        class TestSingleton : Singleton<TestSingleton>
        {
            public int Value { get; set; }
            
            // 私有构造函数
            private TestSingleton() { }
        }

        // 另一个测试用单例类
        class AnotherSingleton : Singleton<AnotherSingleton>
        {
            public string Name { get; set; }
            
            private AnotherSingleton() { }
        }

        #region 基本单例测试

        [Test]
        public void Instance_ReturnsNonNull()
        {
            // Act
            var instance = TestSingleton.Instance;
            
            // Assert
            Assert.IsNotNull(instance);
        }

        [Test]
        public void Instance_ReturnsSameInstance()
        {
            // Act
            var instance1 = TestSingleton.Instance;
            var instance2 = TestSingleton.Instance;
            
            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void Instance_DifferentTypes_DifferentInstances()
        {
            // Act
            var test = TestSingleton.Instance;
            var another = AnotherSingleton.Instance;
            
            // Assert
            Assert.IsNotNull(test);
            Assert.IsNotNull(another);
            Assert.AreNotSame(test, another);
        }

        [Test]
        public void Instance_PreservesState()
        {
            // Arrange
            TestSingleton.Instance.Value = 42;
            
            // Act
            var value = TestSingleton.Instance.Value;
            
            // Assert
            Assert.AreEqual(42, value);
        }

        #endregion

        #region 线程安全测试

        [Test]
        public void Instance_MultipleThreads_ReturnsSameInstance()
        {
            // Arrange
            const int threadCount = 10;
            var instances = new TestSingleton[threadCount];
            var barrier = new Barrier(threadCount);
            var tasks = new Task[threadCount];
            
            // Act - 同时从多个线程获取实例
            for (int i = 0; i < threadCount; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() =>
                {
                    barrier.SignalAndWait(); // 等待所有线程就绪后同时开始
                    instances[index] = TestSingleton.Instance;
                });
            }
            
            Task.WaitAll(tasks);
            
            // Assert - 所有线程获取的应该是同一个实例
            var first = instances[0];
            for (int i = 1; i < threadCount; i++)
            {
                Assert.AreSame(first, instances[i], $"Thread {i} got different instance");
            }
        }

        [Test]
        public void Instance_ConcurrentAccess_NoExceptions()
        {
            // Arrange
            const int iterationCount = 1000;
            var exceptions = new System.Collections.Concurrent.ConcurrentBag<System.Exception>();
            
            // Act
            Parallel.For(0, iterationCount, i =>
            {
                try
                {
                    var instance = TestSingleton.Instance;
                    instance.Value = i;
                    var _ = instance.Value; // 读取
                }
                catch (System.Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
            
            // Assert
            Assert.IsEmpty(exceptions);
        }

        #endregion

        #region 边界测试

        [Test]
        public void Instance_MultipleAccesses_NeverNull()
        {
            // Arrange
            const int accessCount = 100;
            
            // Act & Assert
            for (int i = 0; i < accessCount; i++)
            {
                Assert.IsNotNull(TestSingleton.Instance);
            }
        }

        #endregion
    }
}
