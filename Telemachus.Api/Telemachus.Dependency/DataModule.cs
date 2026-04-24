using Microsoft.Extensions.DependencyInjection;
using Telemachus.Business.Interfaces.Reports.Design;
using Telemachus.Business.Services.Reports.Design;
using Telemachus.Data.Interfaces.Services;
using Telemachus.Data.Services.Interfaces;
using Telemachus.Data.Services.Repositories;
using Telemachus.Data.Services.Services;

namespace Telemachus.Dependency
{
    internal static class DataModule
    {
        internal static void Load(IServiceCollection services)
        {
            services.AddTransient<IFileDataService, FileDataService>();
            services.AddTransient<IEventTypeRepository, EventTypeRepository>();
            services.AddTransient<IEventTypeDataService, EventTypeDataService>();

            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IEventDataService, EventDataService>();

            services.AddTransient<IVoyageDataService, VoyageDataService>();
            services.AddTransient<IVoyageRepository, VoyageRepository>();

            services.AddTransient<IReportDataService, ReportDataService>();
            services.AddTransient<IReportRepository, ReportRepository>();

            services.AddTransient<IReportDesignRepository, ReportDesignRepository>();
            services.AddTransient<IReportDesignService, ReportDesignService>();

            services.AddTransient<ICargoDataService, CargoDataService>();

        }
    }
}
