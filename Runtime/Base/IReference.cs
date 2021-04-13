namespace Framework
{
    public interface IReference
    {
        bool IsUnused { get; }
        int RefCount { get; }
        void Retain();
        void Release();
    }
}