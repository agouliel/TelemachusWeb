using System.Collections.Generic;

namespace Telemachus.Business.Models.Login
{
    public class LoginResponseBusinessModel
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public bool? HasReportsEnabled { get; set; } = null;
        public bool HasRemoteData { get; set; } = false;
        public bool HasVesselConfiguration { get; set; } = false;
        public bool IsInHouse { get; set; } = false;
        public bool IsDevelopment { get; set; } = false;
        public bool? HasInitialData { get; set; } = null;
        public List<UserBusinessModel> Vessels { get; set; } = new List<UserBusinessModel>();
    }
}
