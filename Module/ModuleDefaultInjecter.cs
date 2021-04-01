using FInject;
using Framework.Module.Debugger;
using Framework.Module.Resource;

namespace Framework.Module
{
    public class ModuleDefaultInjectInfo : IModuleInjectInfo
    {
        public Context context { get; private set; }

        public ModuleDefaultInjectInfo()
        {
            context = new Context();
        }

        public void Initialize()
        {
            var resourceManager = ModuleManager.Instance.GetModule<IResourceManager>();
            context.Bind<IResourceManager>().AsInstance(resourceManager);
            context.Bind<IResourceLoader>().As<ResourceLoader>();
            context.Bind<IDebugger>().As<UnityDebugger>();
        }
    }
}
