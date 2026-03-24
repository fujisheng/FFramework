using System.Collections.Generic;
using NUnit.Framework;

namespace Framework.Tests.Editor.Resource
{
    /// <summary>
    /// 测试用 Mock IReference 实现
    /// 用于 ReferenceTree 单元测试
    /// </summary>
    public class MockReference : IReference
    {
        public string Name { get; }
        public bool IsReleased { get; private set; }
        public int ReleaseCount { get; private set; }

        public MockReference(string name)
        {
            Name = name;
            IsReleased = false;
            ReleaseCount = 0;
        }

        public void Release()
        {
            IsReleased = true;
            ReleaseCount++;
        }

        public override string ToString()
        {
            return $"MockRef({Name})";
        }

        public override bool Equals(object obj)
        {
            if (obj is MockReference other)
            {
                return Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}
