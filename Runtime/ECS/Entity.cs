namespace Framework.ECS
{
    public struct Entity
    {
        internal int Index { get; private set; }
        internal Entity(int index)
        {
            this.Index = index;
        }

        public static bool operator==(Entity l, Entity r)
        {
            return l.Index == r.Index;
        }

        public static bool operator!=(Entity l, Entity r)
        {
            return l.Index != r.Index;
        }

        public override bool Equals(object obj)
        {
            if(obj is Entity entity)
            {
                return entity.Index == this.Index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Index;
        }

        public override string ToString()
        {
            return $"entity:{Index}";
        }
    }
}