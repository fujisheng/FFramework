using Framework.Collections;

namespace Framework
{
    class ReferenceNode : MapNode<IReference>
    {
        internal Mark mark;

        public ReferenceNode(IReference reference) : base(reference)
        {
            mark = Mark.Black;
        }
    }
}