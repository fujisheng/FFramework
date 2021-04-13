using FInject;
using Framework.Module.Debugger;
using Framework.Module.Resource;

namespace Framework.Module
{
    public class ModuleDefaultInjectInfo : IModuleInjectInfo
    {
        public Context Context { get; private set; }

        public ModuleDefaultInjectInfo()
        {
            Context = new Context();
        }

        public void Initialize()
        {
            var resourceManager = ModuleManager.GetModule<IResourceManager>();
            Context.Bind<IResourceManager>().AsInstance(resourceManager);
            Context.Bind<IResourceLoader>().As<ResourceLoader>();
            Context.Bind<IDebugger>().As<UnityDebugger>();
        }
    }
}
