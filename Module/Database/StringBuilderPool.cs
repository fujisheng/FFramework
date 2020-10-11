using System.Text;
namespace Framework.Module.Database
{
    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        public override int Size => 10;
        protected override StringBuilder New()
        {
            return new StringBuilder();
        }
    }

    internal static class StringBuilders
    {
        static StringBuilderPool pool = new StringBuilderPool();

        public static StringBuilder Pop()
        {
            return pool.Pop();
        }

        public static void Push(StringBuilder builder)
        {
            pool.Push(builder);
        }

        public static void Dispose()
        {
            pool.Dispose();
        }
    }
}