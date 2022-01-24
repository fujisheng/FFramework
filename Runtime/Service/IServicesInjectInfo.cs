using FInject;

namespace Framework.Service
{
    public interface IServicesInjectInfo
    {
        Container container { get; }

        void Initialize();
    }
}