using FInject;

namespace Framework.Module
{
    public interface IModuleInjectInfo
    {
        Context Context { get; }

        void Initialize();
    }
}