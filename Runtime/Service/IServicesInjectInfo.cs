using FInject;

namespace Framework.Service
{
    public interface IServicesInjectInfo
    {
        Context Context { get; }

        void Initialize();
    }
}