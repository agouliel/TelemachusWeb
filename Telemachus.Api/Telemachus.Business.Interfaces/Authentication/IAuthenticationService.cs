using System.Collections.Generic;
using System.Threading.Tasks;
using Telemachus.Business.Models.Login;

namespace Telemachus.Business.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<LoginResponseBusinessModel> AuthenticateAsync(string login, string password, string passcode = "", string comment = "");
        Task<List<UserBusinessModel>> GetVesselsAsync(string targetVessel);
        Task<UserBusinessModel> GetVesselAsync(string userId);
        Task<string> CreateVesselUserAsync(string userName, string password);
        Task UpdateVesselUserPassword(string userName, string password);
        Task<LoginResponseBusinessModel> GetUserDetails(string userId);
        Task<LoginResponseBusinessModel> SwitchAsync(string id);
        Task<object> GetBootstrapData();
        Task ResetPasscode(string passcodeVal);
    }
}
