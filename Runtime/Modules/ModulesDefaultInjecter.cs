using Framework.IoC;
using Framework.Module.Log;
using Framework.Module.Network;
using Framework.Module.Resource;

namespace Framework.Module
{
    public class ModulesDefaultInjectInfo : IModulesInjectInfo
    {
        public Container container { get; private set; }

        public ModulesDefaultInjectInfo()
        {
            container = new Container();
        }

        public void Initialize()
        {
            var resourceManager = ModuleManager.Get<IResourceModule>();
            container.Bind<IResourceModule>().FromInstance(resourceManager);
            container.Bind<IResourceLoader, ResourceLoader>();
            container.Bind<ILogger, UnityLogger>();

            container.Bind<INetworkChannel>().FromInstance(new TcpChannel());
            container.Bind<INetworkPackageHelper>().FromInstance(new DefaultNetworkPackageHelper(new PacketPool()));
            container.Bind<INetworkBCCHelper>().FromInstance(new DefaultNetworkBCCHelper());
            container.Bind<INetworkCompressHelper>().FromInstance(new DefaultNetworkCompressHelper());

            container.Bind<Archive.ISerializer, Archive.UnityJsonSerializer>();
            container.Bind<Archive.IEncryptionProvider, Archive.EmptyEncryptor>();
                //key: new byte[16] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 },
                //iv: new byte[16] { 0x0F, 0x1E, 0x2D, 0x3C, 0x4B, 0x5A, 0x69, 0x78, 0x87, 0x96, 0xA5, 0xB4, 0xC3, 0xD2, 0xE1, 0xF0 }
            //));
            container.Bind<Archive.ICompressionProvider, Archive.EmptyCompressionProvider>();
        }
    }
}
