using Shadowsocks.Common.Model;
using Shadowsocks.Model;

using SimpleInjector;

namespace Shadowsocks
{
    public static class ShadowsocksClient
    {
        static ShadowsocksClient()
        {
            var container = IoCManager.Container;

            container.Register<Configuration>(Lifestyle.Singleton);
            container.Collection.Register<IService>(new[] { (typeof(ShadowsocksClient)).Assembly });

            container.Verify();
        }

        public static void Startup()
        {
            foreach (var service in IoCManager.Container.GetAllInstances<IService>())
                service.Startup();

        }
    }
}
