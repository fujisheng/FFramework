using System.Collections.Generic;
using NUnit.Framework;
using Framework.Collections;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// MapNode 图节点测试用例
    /// 测试父子关系建立、移除等操作
    /// </summary>
    [TestFixture]
    public class MapNodeTests
    {
        [Test]
        public void Constructor_WithValue_StoresValue()
        {
            // Arrange & Act
            var node = new MapNode<string>("TestValue");
            
            // Assert
            Assert.AreEqual("TestValue", node.Value);
        }

        [Test]
        public void Constructor_InitializesEmptyLists()
        {
            // Arrange & Act
            var node = new MapNode<string>("Test");
            
            // Assert
            Assert.IsNotNull(node.Children);
            Assert.IsNotNull(node.Previous);
            Assert.AreEqual(0, node.Children.Count);
            Assert.AreEqual(0, node.Previous.Count);
        }

        [Test]
        public void AddChild_SingleChild_AddsToChildren()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            
            // Act
            parent.AddChild(child);
            
            // Assert
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(child, parent.Children[0]);
        }

        [Test]
        public void AddChild_SetsParentReference()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            
            // Act
            parent.AddChild(child);
            
            // Assert
            Assert.AreEqual(1, child.Previous.Count);
            Assert.AreSame(parent, child.Previous[0]);
        }

        [Test]
        public void AddChild_DuplicateChild_NotAddedTwice()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            
            // Act
            parent.AddChild(child);
            parent.AddChild(child);
            
            // Assert
            Assert.AreEqual(1, parent.Children.Count);
        }

        [Test]
        public void AddChild_MultipleChildren_AllAdded()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child1 = new MapNode<string>("Child1");
            var child2 = new MapNode<string>("Child2");
            var child3 = new MapNode<string>("Child3");
            
            // Act
            parent.AddChild(child1);
            parent.AddChild(child2);
            parent.AddChild(child3);
            
            // Assert
            Assert.AreEqual(3, parent.Children.Count);
        }

        [Test]
        public void RemoveChild_ExistingChild_RemovesFromChildren()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            parent.AddChild(child);
            
            // Act
            parent.RemoveChild("Child");
            
            // Assert
            Assert.AreEqual(0, parent.Children.Count);
        }

        [Test]
        public void RemoveChild_ExistingChild_RemovesParentReference()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            parent.AddChild(child);
            
            // Act
            parent.RemoveChild("Child");
            
            // Assert
            Assert.AreEqual(0, child.Previous.Count);
        }

        [Test]
        public void RemoveChild_NonExistingChild_DoesNotThrow()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            
            // Act & Assert
            Assert.DoesNotThrow(() => parent.RemoveChild("NonExistent"));
        }

        [Test]
        public void AddPrevious_SingleParent_AddsToPrevious()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            
            // Act
            child.AddPrevious(parent);
            
            // Assert
            Assert.AreEqual(1, child.Previous.Count);
            Assert.AreSame(parent, child.Previous[0]);
        }

        [Test]
        public void AddPrevious_DuplicateParent_NotAddedTwice()
        {
            // Arrange
            var parent = new MapNode<string>("Parent");
            var child = new MapNode<string>("Child");
            
            // Act
            child.AddPrevious(parent);
            child.AddPrevious(parent);
            
            // Assert
            Assert.AreEqual(1, child.Previous.Count);
        }

        [Test]
        public void MultipleParents_AllTracked()
        {
            // Arrange
            var parent1 = new MapNode<string>("Parent1");
            var parent2 = new MapNode<string>("Parent2");
            var child = new MapNode<string>("Child");
            
            // Act
            parent1.AddChild(child);
            parent2.AddChild(child);
            
            // Assert
            Assert.AreEqual(2, child.Previous.Count);
        }

        [Test]
        public void DiamondStructure_CorrectlyBuilt()
        {
            // Arrange: Diamond pattern
            //     Root
            //    /    \
            //   A      B
            //    \    /
            //      C
            var root = new MapNode<string>("Root");
            var a = new MapNode<string>("A");
            var b = new MapNode<string>("B");
            var c = new MapNode<string>("C");
            
            // Act
            root.AddChild(a);
            root.AddChild(b);
            a.AddChild(c);
            b.AddChild(c);
            
            // Assert
            Assert.AreEqual(2, root.Children.Count);
            Assert.AreEqual(1, a.Children.Count);
            Assert.AreEqual(1, b.Children.Count);
            Assert.AreEqual(2, c.Previous.Count);  // C has two parents
            Assert.AreEqual(0, c.Children.Count);
        }

        [Test]
        public void RemoveChild_FromDiamond_OnlyRemovesOneEdge()
        {
            // Arrange
            var a = new MapNode<string>("A");
            var b = new MapNode<string>("B");
            var c = new MapNode<string>("C");
            
            a.AddChild(c);
            b.AddChild(c);
            
            // Act - remove C from A only
            a.RemoveChild("C");
            
            // Assert
            Assert.AreEqual(0, a.Children.Count);
            Assert.AreEqual(1, b.Children.Count);
            Assert.AreEqual(1, c.Previous.Count);  // Only B is parent now
            Assert.AreSame(b, c.Previous[0]);
        }

        #region 类型测试

        [Test]
        public void MapNode_WithIntValue_Works()
        {
            // Arrange
            var node = new MapNode<int>(42);
            
            // Assert
            Assert.AreEqual(42, node.Value);
        }

        [Test]
        public void MapNode_WithObjectValue_Works()
        {
            // Arrange
            var obj = new System.Object();
            var node = new MapNode<object>(obj);
            
            // Assert
            Assert.AreSame(obj, node.Value);
        }

        #endregion
    }
}
