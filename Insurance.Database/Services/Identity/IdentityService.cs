﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Database.Roles;
using Insurance.Database.Users;
using Insurance.ApiProviders.Identity;
using Insurance.ApiProviders.IClients;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Insurance.Database.Services.Identity
{
    public class IdentityService : IIdentityService, IDisposable
    {
        private readonly IClientService _clientService;
        private readonly IInsuranceRoleManager _roleManager;
        private readonly IInsuranceUserManager<IdentityUser, string> _userManager;

        public IdentityService(IClientService clientService,
            IInsuranceUserManager<IdentityUser, string> userManager,
            IInsuranceRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _clientService = clientService;
        }

        /// <summary>
        ///     Authenticates the user with the database
        /// </summary>
        public async Task<bool> Authenticate(string username, string password)
        {
            var user = _userManager.FindByNameAsync(username).Result;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> Register(string userName, string password)
        {
            var user = new IdentityUser {UserName = userName, Email = ""};

            if (!_roleManager.RoleExists("admin"))
            {
                var role = new IdentityRole();
                role.Name = "admin";

                await _roleManager.CreateAsync(role);
            }

            if (!_roleManager.RoleExists("guest"))
            {
                var role = new IdentityRole();
                role.Name = "guest";

                await _roleManager.CreateAsync(role);
            }

            var result = await _userManager.CreateAsync(user, password);

            // If user registering is from external add its role, else add admin role
            var externalUser = await _clientService.GetByUserAsync(user.UserName).ConfigureAwait(false);
            if (externalUser != null)
                await _userManager.AddToRoleAsync(user.Id, externalUser.Role);
            else
                await _userManager.AddToRoleAsync(user.Id, "guest");

            return result;
        }

        public async Task<List<string>> GetAllUsers()
        {
            return _userManager.Users.Select(u => u.UserName).ToList();
        }

        public void Dispose()
        {
        }

        /// <summary>
        ///     Get roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetRoles(string userId)
        {
            return _userManager.GetRoles(userId).ToList();
        }
    }
}