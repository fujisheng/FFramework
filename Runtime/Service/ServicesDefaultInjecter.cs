using FInject;
using Framework.Service.Debug;
using Framework.Service.Network;
using Framework.Service.Resource;

namespace Framework.Service
{
    public class ServicesDefaultInjectInfo : IServicesInjectInfo
    {
        public Context Context { get; private set; }

        public ServicesDefaultInjectInfo()
        {
            Context = new Context();
        }

        public void Initialize()
        {
            var resourceManager = Services.Get<IResourceService>();
            Context.Bind<IResourceService>().AsInstance(resourceManager);
            Context.Bind<IResourceLoader>().As<ResourceLoader>();
            Context.Bind<IDebugger>().As<UnityDebugger>();

            Context.Bind<INetworkChannel>().AsInstance(new TcpChannel());
            Context.Bind<INetworkPackageHelper>().AsInstance(new DefaultNetworkPackageHelper(new PacketPool()));
            Context.Bind<INetworkBCCHelper>().AsInstance(new DefaultNetworkBCCHelper());
            Context.Bind<INetworkCompressHelper>().AsInstance(new DefaultNetworkCompressHelper());
        }
    }
}
