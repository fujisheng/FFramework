using Framework.IoC;

namespace Framework.Module
{
    public interface IModulesInjectInfo
    {
        Container container { get; }

        void Initialize();
    }
}