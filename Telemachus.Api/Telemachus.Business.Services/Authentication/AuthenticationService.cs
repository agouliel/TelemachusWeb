using Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces.Authentication;
using Telemachus.Business.Models.Login;
using Telemachus.Business.Services.Mappers;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Models.Events;
using Telemachus.Data.Services.Context;
using Telemachus.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Telemachus.Business.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly TelemachusContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private bool _hasVesselGateway { get; set; } = false;
        private VesselDetails _vesselDetails { get; set; }
        private bool _isInHouse { get; set; } = false;

        public AuthenticationService(IHttpContextAccessor httpContext, UserManager<User> userManager, IConfiguration config, TelemachusContext context, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
            _env = env;
            var gatewayIp = _config["Gateway"];
            if (!string.IsNullOrEmpty(gatewayIp))
            {
                var remoteIp = httpContext.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                if (remoteIp == gatewayIp)
                {
                    _hasVesselGateway = true;
                }
            }
            _vesselDetails = _config.GetSection("VesselDetails").Get<VesselDetails>();
            _isInHouse = string.IsNullOrEmpty(_vesselDetails?.Prefix) && !_hasVesselGateway;
        }

        public async Task<LoginResponseBusinessModel> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var vessels = await GetVesselsAsync(_vesselDetails?.Prefix);

            return new LoginResponseBusinessModel()
            {
                Role = roles.FirstOrDefault(),
                UserName = user.UserName,
                UserId = user.Id,
                Vessels = vessels,
                IsDevelopment = _env.IsDevelopment(),
                HasVesselConfiguration = !string.IsNullOrEmpty(_vesselDetails?.Prefix),
                HasRemoteData = HasRemoteData(user),
                IsInHouse = _isInHouse
            };
        }

        public async Task ResetPasscode(string passcodeVal)
        {
            var passcode = await _context.UserPasscodes.Where(_ => _.Passcode == passcodeVal).FirstOrDefaultAsync();

            if (passcode == null)
            {
                throw new CustomException("Passcode not found.");
            }

            _context.UserPasscodes.Remove(passcode);
            await _context.SaveChangesAsync();
        }

        private async Task<UserPasscode> GetPasscode(string userId, string passCode, string comment)
        {
            var userPasscodes = await _context.UserPasscodes.Where(_ => _.UserId == userId).ToListAsync();

            var userPasscode = userPasscodes.FirstOrDefault(_ => _.Passcode == passCode);

            if (userPasscode == null && !string.IsNullOrEmpty(passCode) && !string.IsNullOrEmpty(comment))
            {
                var availableSlots = await _context.Users.Where(_ => _.Id == userId).Select(_ => _.AvailablePasscodeSlots).FirstAsync();
                if (userPasscodes.Count < availableSlots)
                {
                    userPasscode = new UserPasscode()
                    {
                        UserId = userId,
                        Passcode = passCode,
                        Comment = comment,
                        DateCreated = DateTime.UtcNow
                    };
                    _context.UserPasscodes.Add(userPasscode);
                    await _context.SaveChangesAsync();
                }
            }

            return userPasscode;
        }

        public async Task<LoginResponseBusinessModel> AuthenticateAsync(string login, string password, string passcode = "", string comment = "")
        {
            var user = await _userManager.FindByNameAsync(login);
            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var claim = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                    };
                    foreach (var role in roles)
                    {
                        claim.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
                    var token = new JwtSecurityToken(
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                        claims: claim);
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    var userPasscode = await GetPasscode(user.Id, passcode, comment);

                    var vessels = await GetVesselsAsync(_vesselDetails?.Prefix);

                    var hasRemoteData = HasRemoteData(user);

                    bool? hasInitialData = null;

                    if (!_isInHouse && hasRemoteData)
                    {
                        hasInitialData = await _context.Voyages.AnyAsync(v => v.UserId == user.Id);
                    }

                    return new LoginResponseBusinessModel()
                    {
                        Token = tokenString,
                        Role = roles.FirstOrDefault(),
                        UserName = login,
                        UserId = user.Id,
                        HasReportsEnabled = _isInHouse || userPasscode != null,
                        Vessels = vessels,
                        IsDevelopment = _env.IsDevelopment(),
                        HasVesselConfiguration = !string.IsNullOrEmpty(_vesselDetails?.Prefix),
                        HasRemoteData = HasRemoteData(user),
                        HasInitialData = hasInitialData,
                        IsInHouse = _isInHouse,
                    };
                }
            }

            return new LoginResponseBusinessModel();
        }

        private bool HasRemoteData(User user)
        {
            var isRemoteConnectionValid = false;

            if (!string.IsNullOrEmpty(_vesselDetails?.Prefix))
            {
                isRemoteConnectionValid = !string.IsNullOrEmpty(user.RemoteAddress) && !string.IsNullOrEmpty(_config.GetValue<string>("SyncCredentials:Passcode"));
            }
            var hasRemoteData = _isInHouse ? !string.IsNullOrEmpty(user.RemoteAddress) : isRemoteConnectionValid;
            return hasRemoteData;
        }

        public async Task<object> GetBootstrapData()
        {
            var vessels = await GetVesselsAsync(_vesselDetails?.Prefix);

            return new
            {
                Vessels = vessels,
                HasVesselConfiguration = !string.IsNullOrEmpty(_vesselDetails?.Prefix),
                IsInHouse = _isInHouse,
                IsDevelopment = _env.IsDevelopment()
            };
        }

        public async Task<LoginResponseBusinessModel> SwitchAsync(string id)
        {
            if (!_isInHouse)
            {
                return new LoginResponseBusinessModel();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new LoginResponseBusinessModel();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var claim = new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                    };
            foreach (var role in roles)
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var token = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                claims: claim);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var vessels = await GetVesselsAsync(_vesselDetails?.Prefix);

            return new LoginResponseBusinessModel()
            {
                Token = tokenString,
                Role = roles.FirstOrDefault(),
                UserName = user.UserName,
                UserId = user.Id,
                HasReportsEnabled = null,
                Vessels = vessels,
                IsDevelopment = _env.IsDevelopment(),
                HasVesselConfiguration = !string.IsNullOrEmpty(_vesselDetails?.Prefix),
                HasRemoteData = HasRemoteData(user),
                IsInHouse = _isInHouse,
            };
        }

        public async Task<List<UserBusinessModel>> GetVesselsAsync(string targetVessel)
        {
            var query = _context
                .Users
                .Where(u => !Vessel.HiddenVessels.Contains(u.Id))
                .AsQueryable();
            if (!string.IsNullOrEmpty(targetVessel))
            {
                query = query.Where(u => u.Prefix.ToLower() == targetVessel.ToLower());
            };

            var users = await query
                .OrderBy(_ => _.UserName)
                .Select(_ => new User()
                {
                    Id = _.Id,
                    UserName = _.UserName,
                    Prefix = _.Prefix,
                })
                .ToListAsync();
            return users?.Select(a => a.ToBusinessModel()).ToList();
        }

        public async Task<UserBusinessModel> GetVesselAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user.ToBusinessModel();
        }

        public async Task UpdateVesselUserPassword(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            foreach (var validator in _userManager.PasswordValidators)
            {
                var passwordResult = await validator.ValidateAsync(_userManager, null, password);

                if (!passwordResult.Succeeded)
                {
                    throw new Exception(string.Join(", ", passwordResult.Errors.Select(_ => _.Description)));
                }
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(_ => _.Description)));
            }
        }

        public async Task<string> CreateVesselUserAsync(string userName, string password)
        {
            var errors = string.Empty;
            var user = new User()
            {
                UserName = userName
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                user = await _userManager.FindByNameAsync(userName);
                result = await _userManager.AddToRoleAsync(user, "BASIC");
                if (result.Succeeded)
                {
                    var voyage = new VoyageDataModel()
                    {
                        UserId = user.Id,
                        CurrentConditionId = 6,
                        IsFinished = false,
                        StartDate = DateTimeOffset.UtcNow,
                        CurrentVoyageConditionKey = Guid.NewGuid()
                    };
                    _context.Voyages.Add(voyage);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    errors = string.Join(",", result.Errors.Select(a => a.Code).ToList());
                }
            }
            else
            {
                errors = string.Join(",", result.Errors.Select(a => a.Code).ToList());
            }

            return errors;
        }

    }
}
