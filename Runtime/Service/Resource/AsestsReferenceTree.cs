using System;

namespace Framework.Service.Resource
{
    class AssetsReferenceTree : ReferenceTree
    {
        class ReferenceRoot : IReference
        {
            public void Release()
            {
                throw new Exception("AssetsReferenceRoot can not delete");
            }
        }

        static AssetsReferenceTree instance;
        static IReference root;
        internal static AssetsReferenceTree Instance
        {
            get
            {
                return instance ?? (instance = new AssetsReferenceTree());
            }
        }

        internal static IReference Root
        {
            get
            {
                return root ?? (root = new ReferenceRoot());
            }
        }
    }
}