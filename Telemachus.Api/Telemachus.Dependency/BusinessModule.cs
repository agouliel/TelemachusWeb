using Microsoft.Extensions.DependencyInjection;
using Telemachus.Business.Interfaces;
using Telemachus.Business.Interfaces.Cargo;
using Telemachus.Business.Interfaces.Events;
using Telemachus.Business.Interfaces.Reports;
using Telemachus.Business.Services;
using Telemachus.Business.Services.Cargo;
using Telemachus.Business.Services.Events;
using Telemachus.Business.Services.Reports;
using Telemachus.Business.Services.Voyages;
using AuthenticationService = Telemachus.Business.Services.Authentication.AuthenticationService;
using IAuthenticationService = Telemachus.Business.Interfaces.Authentication.IAuthenticationService;

namespace Telemachus.Dependency
{
    internal static class BusinessModule
    {
        internal static void Load(IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IEventTypeService, EventTypeService>();

            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IStatementService, StatementService>();
            services.AddTransient<IVoyageService, VoyageService>();
            services.AddTransient<ICargoBusinessService, CargoBusinessService>();
        }
    }
}
