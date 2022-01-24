using FInject;
using Framework.Service.Debug;
using Framework.Service.Network;
using Framework.Service.Resource;

namespace Framework.Service
{
    public class ServicesDefaultInjectInfo : IServicesInjectInfo
    {
        public Container container { get; private set; }

        public ServicesDefaultInjectInfo()
        {
            container = new Container();
        }

        public void Initialize()
        {
            var resourceManager = Services.Get<IResourceService>();
            container.Bind<IResourceService>().AsInstance(resourceManager);
            container.Bind<IResourceLoader>().As<ResourceLoader>();
            container.Bind<IDebugger>().As<UnityDebugger>();

            container.Bind<INetworkChannel>().AsInstance(new TcpChannel());
            container.Bind<INetworkPackageHelper>().AsInstance(new DefaultNetworkPackageHelper(new PacketPool()));
            container.Bind<INetworkBCCHelper>().AsInstance(new DefaultNetworkBCCHelper());
            container.Bind<INetworkCompressHelper>().AsInstance(new DefaultNetworkCompressHelper());
        }
    }
}
