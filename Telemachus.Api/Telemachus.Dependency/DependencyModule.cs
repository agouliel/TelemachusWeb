using Microsoft.Extensions.DependencyInjection;

namespace Telemachus.Dependency
{
    public static class DependencyModule
    {
        public static void Load(IServiceCollection services)
        {
            BusinessModule.Load(services);
            DataModule.Load(services);
        }
    }
}
