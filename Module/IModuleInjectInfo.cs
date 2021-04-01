using FInject;

namespace Framework.Module
{
    public interface IModuleInjectInfo
    {
        Context context { get; }

        void Initialize();
    }
}