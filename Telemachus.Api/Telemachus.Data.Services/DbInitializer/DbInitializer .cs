using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using Telemachus.Data.Models.Authentication;

namespace Telemachus.Data.Services.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public DbInitializer(IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _config = config;
        }
        private void ClearPassword()
        {
            var appSettingsPath = "appsettings.json";
            var json = JObject.Parse(File.ReadAllText(appSettingsPath));
            json["VesselDetails"]["InitialPassword"] = string.Empty;

            File.WriteAllText(appSettingsPath, json.ToString());
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var _context = serviceScope.ServiceProvider.GetService<UserManager<User>>())
                {
                    var vesselDetails = _config.GetSection("VesselDetails").Get<Telemachus.Models.VesselDetails>();

                    if (vesselDetails == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(vesselDetails.RemoteAddress))
                    {
                        throw new CustomException("Remote address is missing. (VesselDetails @ appsettings.json)");
                    }

                    if (vesselDetails.RemotePort == 0)
                    {
                        throw new CustomException("Remote port is missing. (VesselDetails @ appsettings.json)");
                    }

                    if (_context.Users.Any(_ => _.Prefix == vesselDetails.Prefix.ToUpper()))
                    {
                        var user = _context.Users.First(_ => _.Prefix == vesselDetails.Prefix.ToUpper());
                        user.RemoteAddress = vesselDetails.RemoteAddress;
                        user.RemotePort = vesselDetails.RemotePort;
                        if (!string.IsNullOrEmpty(vesselDetails.InitialPassword))
                        {
                            var ph = new PasswordHasher<User>();
                            user.PasswordHash = ph.HashPassword(user, vesselDetails.InitialPassword);
                            ClearPassword();
                        }
                        _context.UpdateAsync(user).Wait();
                    }
                    else
                    {

                        if (string.IsNullOrEmpty(vesselDetails.InitialPassword))
                        {
                            throw new CustomException("Password is missing. (VesselDetails @ appsettings.json)");
                        }
                        if (string.IsNullOrEmpty(vesselDetails.Operator))
                        {
                            throw new CustomException("Operator is missing. (VesselDetails @ appsettings.json)");
                        }
                        if (!vesselDetails.PitchPropeller.HasValue)
                        {
                            throw new CustomException("PitchPropeller is missing. (VesselDetails @ appsettings.json)");
                        }
                        if (!vesselDetails.MainEngineMaxPower.HasValue)
                        {
                            throw new CustomException("MainEngineMaxPower is missing. (VesselDetails @ appsettings.json)");
                        }
                        if (string.IsNullOrEmpty(vesselDetails.Prefix))
                        {
                            throw new CustomException("Prefix is missing. (VesselDetails @ appsettings.json)");
                        }
                        if (string.IsNullOrEmpty(vesselDetails.Name))
                        {
                            throw new CustomException("UserName is missing. (VesselDetails @ appsettings.json)");
                        }

                        var ph = new PasswordHasher<User>();

                        var user = new User
                        {
                            Operator = new Enums.Operator(vesselDetails.Operator).ToString(),
                            PitchPropeller = vesselDetails.PitchPropeller,
                            MainEngineMaxPower = vesselDetails.MainEngineMaxPower,
                            Prefix = vesselDetails.Prefix.ToUpper(),
                            UserName = vesselDetails.Name.ToLower(),
                            RemoteAddress = vesselDetails.RemoteAddress,
                            RemotePort = vesselDetails.RemotePort,
                            NonHafnia = vesselDetails.NonHafnia,
                            NonPool = vesselDetails.NonPool,
                            AvailablePasscodeSlots = 0
                        };
                        user.PasswordHash = ph.HashPassword(user, vesselDetails.InitialPassword);
                        _context.CreateAsync(user).Wait();
                        ClearPassword();

                    }

                }
            }
        }
    }
}
