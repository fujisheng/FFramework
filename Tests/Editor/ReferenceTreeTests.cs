using System.Collections.Generic;
using NUnit.Framework;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// ReferenceTree 三色标记法 GC 测试用例
    /// 测试引用树的分配、释放、标记和清扫逻辑
    /// </summary>
    [TestFixture]
    public class ReferenceTreeTests
    {
        ReferenceTree tree;
        List<MockReference> allRefs;

        [SetUp]
        public void SetUp()
        {
            tree = new ReferenceTree();
            allRefs = new List<MockReference>();
        }

        [TearDown]
        public void TearDown()
        {
            tree = null;
            allRefs.Clear();
            allRefs = null;
        }

        MockReference CreateRef(string name)
        {
            var r = new MockReference(name);
            allRefs.Add(r);
            return r;
        }

        #region Alloc 分配测试

        [Test]
        public void Alloc_SingleRoot_AddsToRoots()
        {
            // Arrange
            var root = CreateRef("Root");
            
            // Act
            tree.Alloc(root);
            
            // 触发 GC
            tree.Collect();
            tree.Delete();
            
            // Assert - root 应该存活（不被释放）
            Assert.IsFalse(root.IsReleased);
        }

        [Test]
        public void Alloc_WithParentAndChild_BuildsRelationship()
        {
            // Arrange
            var parent = CreateRef("Parent");
            var child = CreateRef("Child");
            
            // Act
            tree.Alloc(parent);          // parent 作为根
            tree.Alloc(parent, child);   // child 作为 parent 的子节点
            
            // 触发 GC
            tree.Collect();
            tree.Delete();
            
            // Assert - 都应该存活
            Assert.IsFalse(parent.IsReleased);
            Assert.IsFalse(child.IsReleased);
        }

        [Test]
        public void Alloc_MultiLevel_AllSurvive()
        {
            // Arrange: Root -> A -> B -> C
            var root = CreateRef("Root");
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            
            // Act
            tree.Alloc(root);
            tree.Alloc(root, a);
            tree.Alloc(a, b);
            tree.Alloc(b, c);
            
            tree.Collect();
            tree.Delete();
            
            // Assert
            Assert.IsFalse(root.IsReleased);
            Assert.IsFalse(a.IsReleased);
            Assert.IsFalse(b.IsReleased);
            Assert.IsFalse(c.IsReleased);
        }

        #endregion

        #region Collect + Delete 垃圾回收测试

        [Test]
        public void CollectDelete_UnreferencedNode_GetsReleased()
        {
            // Arrange
            var root = CreateRef("Root");
            var orphan = CreateRef("Orphan");
            
            tree.Alloc(root);
            tree.Alloc(orphan);  // orphan 也是根
            
            // Act - 第一次 GC，两者都存活
            tree.Collect();
            tree.Delete();
            
            Assert.IsFalse(root.IsReleased);
            Assert.IsFalse(orphan.IsReleased);
            
            // 手动释放 orphan（模拟用户释放）
            tree.Release(orphan);
            
            // 第二次 GC
            tree.Collect();
            tree.Delete();
            
            // Assert - orphan 应该被释放
            Assert.IsTrue(orphan.IsReleased);
            Assert.IsFalse(root.IsReleased);
        }

        [Test]
        public void CollectDelete_DiamondDependency_AllSurvive()
        {
            // Arrange: 菱形依赖
            //     Root
            //    /    \
            //   A      B
            //    \    /
            //      C
            var root = CreateRef("Root");
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            
            tree.Alloc(root);
            tree.Alloc(root, a);
            tree.Alloc(root, b);
            tree.Alloc(a, c);
            tree.Alloc(b, c);  // C 被 A 和 B 同时引用
            
            // Act
            tree.Collect();
            tree.Delete();
            
            // Assert - 全部存活
            Assert.IsFalse(root.IsReleased);
            Assert.IsFalse(a.IsReleased);
            Assert.IsFalse(b.IsReleased);
            Assert.IsFalse(c.IsReleased);
        }

        [Test]
        public void CollectDelete_CascadeRelease_ReleasesAllDescendants()
        {
            // Arrange: Root -> A -> B -> C
            var root = CreateRef("Root");
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            
            tree.Alloc(root);
            tree.Alloc(root, a);
            tree.Alloc(a, b);
            tree.Alloc(b, c);
            
            // Act - 强制释放 A，应该级联释放 B 和 C
            tree.Release(a, force: true);
            tree.Collect();
            tree.Delete();
            
            // Assert
            Assert.IsFalse(root.IsReleased);
            Assert.IsTrue(a.IsReleased);
            Assert.IsTrue(b.IsReleased);
            Assert.IsTrue(c.IsReleased);
        }

        #endregion

        #region Release 释放测试

        [Test]
        public void Release_NullReference_DoesNotThrow()
        {
            // Act & Assert - 不应抛出异常
            Assert.DoesNotThrow(() => tree.Release(null));
        }

        [Test]
        public void Release_NonExistentReference_DoesNotThrow()
        {
            // Arrange
            var nonExistent = CreateRef("NonExistent");
            
            // Act & Assert - 不应抛出异常
            Assert.DoesNotThrow(() => tree.Release(nonExistent));
        }

        [Test]
        public void Release_WithChildren_NoForce_DoesNotRelease()
        {
            // Arrange
            var parent = CreateRef("Parent");
            var child = CreateRef("Child");
            
            tree.Alloc(parent);
            tree.Alloc(parent, child);
            
            // Act - 尝试释放有子节点的 parent（不强制）
            tree.Release(parent, force: false);
            tree.Collect();
            tree.Delete();
            
            // Assert - parent 不应被释放（因为有子节点）
            Assert.IsFalse(parent.IsReleased);
            Assert.IsFalse(child.IsReleased);
        }

        [Test]
        public void Release_WithChildren_Force_ReleasesAll()
        {
            // Arrange
            var parent = CreateRef("Parent");
            var child = CreateRef("Child");
            
            tree.Alloc(parent);
            tree.Alloc(parent, child);
            
            // Act - 强制释放
            tree.Release(parent, force: true);
            tree.Collect();
            tree.Delete();
            
            // Assert - 都应该被释放
            Assert.IsTrue(parent.IsReleased);
            Assert.IsTrue(child.IsReleased);
        }

        #endregion

        #region 多根节点测试

        [Test]
        public void MultipleRoots_AllSurvive()
        {
            // Arrange
            var root1 = CreateRef("Root1");
            var root2 = CreateRef("Root2");
            var root3 = CreateRef("Root3");
            
            tree.Alloc(root1);
            tree.Alloc(root2);
            tree.Alloc(root3);
            
            // Act
            tree.Collect();
            tree.Delete();
            
            // Assert
            Assert.IsFalse(root1.IsReleased);
            Assert.IsFalse(root2.IsReleased);
            Assert.IsFalse(root3.IsReleased);
        }

        [Test]
        public void MultipleRoots_SharedChild_ChildSurvives()
        {
            // Arrange
            var root1 = CreateRef("Root1");
            var root2 = CreateRef("Root2");
            var shared = CreateRef("Shared");
            
            tree.Alloc(root1);
            tree.Alloc(root2);
            tree.Alloc(root1, shared);
            tree.Alloc(root2, shared);  // shared 被两个根引用
            
            // Act - 释放 root1
            tree.Release(root1, force: true);
            tree.Collect();
            tree.Delete();
            
            // Assert - shared 仍被 root2 引用，不应释放
            Assert.IsTrue(root1.IsReleased);
            Assert.IsFalse(root2.IsReleased);
            Assert.IsFalse(shared.IsReleased);  // 仍被 root2 引用
        }

        #endregion

        #region 边界情况测试

        [Test]
        public void EmptyTree_CollectDelete_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                tree.Collect();
                tree.Delete();
            });
        }

        [Test]
        public void MultipleCollectDelete_NoDoubleRelease()
        {
            // Arrange
            var root = CreateRef("Root");
            var child = CreateRef("Child");
            
            tree.Alloc(root);
            tree.Alloc(root, child);
            
            // Act - 释放后多次 GC
            tree.Release(child, force: true);
            tree.Collect();
            tree.Delete();
            
            tree.Collect();
            tree.Delete();
            
            tree.Collect();
            tree.Delete();
            
            // Assert - 只释放一次
            Assert.AreEqual(1, child.ReleaseCount);
        }

        [Test]
        public void SelfReference_HandledCorrectly()
        {
            // Arrange - 节点引用自己（虽然逻辑上不应该，但测试健壮性）
            var node = CreateRef("SelfRef");
            
            tree.Alloc(node);
            tree.Alloc(node, node);  // 自引用
            
            // Act & Assert - 不应无限循环
            Assert.DoesNotThrow(() =>
            {
                tree.Collect();
                tree.Delete();
            });
        }

        #endregion

        #region 循环引用检测测试

#if UNITY_EDITOR
        [Test]
        public void Alloc_WouldCreateCycle_LogsWarningAndSkips()
        {
            // Arrange: A -> B -> C
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            
            tree.Alloc(a);
            tree.Alloc(a, b);
            tree.Alloc(b, c);
            
            // Act: 尝试 C -> A (会形成循环)
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning, new System.Text.RegularExpressions.Regex("Cycle detected"));
            tree.Alloc(c, a);
            
            // 触发 GC
            tree.Collect();
            tree.Delete();
            
            // Assert - 所有节点都应该存活（因为都可达）
            Assert.IsFalse(a.IsReleased);
            Assert.IsFalse(b.IsReleased);
            Assert.IsFalse(c.IsReleased);
        }

        [Test]
        public void Alloc_ComplexCycle_DetectsCorrectly()
        {
            // Arrange: 复杂图 Root -> A -> B -> C
            var root = CreateRef("Root");
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            
            tree.Alloc(root);
            tree.Alloc(root, a);
            tree.Alloc(a, b);
            tree.Alloc(b, c);
            
            // Act: 尝试 B -> A (会形成循环)
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning, new System.Text.RegularExpressions.Regex("Cycle detected"));
            tree.Alloc(b, a);
            
            // 触发 GC
            tree.Collect();
            tree.Delete();
            
            // Assert - 所有节点都应该存活
            Assert.IsFalse(root.IsReleased);
            Assert.IsFalse(a.IsReleased);
            Assert.IsFalse(b.IsReleased);
            Assert.IsFalse(c.IsReleased);
        }

        [Test]
        public void Alloc_NoCycle_AllowsNormalAllocation()
        {
            // Arrange: Root -> A -> B
            var root = CreateRef("Root");
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            
            tree.Alloc(root);
            tree.Alloc(root, a);
            tree.Alloc(a, b);
            
            // Act: 添加 Root -> C (不会形成循环)
            tree.Alloc(root, c);
            
            // 触发 GC
            tree.Collect();
            tree.Delete();
            
            // Assert - 所有节点都应该存活
            Assert.IsFalse(root.IsReleased);
            Assert.IsFalse(a.IsReleased);
            Assert.IsFalse(b.IsReleased);
            Assert.IsFalse(c.IsReleased);
        }
#endif

        #endregion

        #region 复杂场景测试

        [Test]
        public void ComplexGraph_OnlyOrphansReleased()
        {
            // Arrange: 复杂图
            //   Root1 -> A -> C
            //   Root2 -> B -> D
            //   Orphan1 (无父节点)
            //   Orphan2 -> OrphanChild
            var root1 = CreateRef("Root1");
            var root2 = CreateRef("Root2");
            var a = CreateRef("A");
            var b = CreateRef("B");
            var c = CreateRef("C");
            var d = CreateRef("D");
            var orphan1 = CreateRef("Orphan1");
            var orphan2 = CreateRef("Orphan2");
            var orphanChild = CreateRef("OrphanChild");
            
            tree.Alloc(root1);
            tree.Alloc(root2);
            tree.Alloc(root1, a);
            tree.Alloc(root2, b);
            tree.Alloc(a, c);
            tree.Alloc(b, d);
            tree.Alloc(orphan1);
            tree.Alloc(orphan2);
            tree.Alloc(orphan2, orphanChild);
            
            // Act - 释放 orphan1 和 orphan2
            tree.Release(orphan1);
            tree.Release(orphan2, force: true);
            tree.Collect();
            tree.Delete();
            
            // Assert
            Assert.IsFalse(root1.IsReleased);
            Assert.IsFalse(root2.IsReleased);
            Assert.IsFalse(a.IsReleased);
            Assert.IsFalse(b.IsReleased);
            Assert.IsFalse(c.IsReleased);
            Assert.IsFalse(d.IsReleased);
            Assert.IsTrue(orphan1.IsReleased);
            Assert.IsTrue(orphan2.IsReleased);
            Assert.IsTrue(orphanChild.IsReleased);
        }

        #endregion
    }
}
