using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using IdentityModule.Models;
using IdentityModule.Database;
using System.Security.Claims;

namespace IdentityModule.Authorize
{
    public static class PolicyRequirements
    {
        public static DeveloperRequirement Developer { get; } = new DeveloperRequirement();
        public static AdministratorRequirement Administartor { get; } = new AdministratorRequirement();
        public static UserRequirement User { get; } = new UserRequirement();
    }

    #region Base

    public class DeveloperRequirement : AuthorizationHandler<DeveloperRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeveloperRequirement requirement)
        {
            if (context.User.IsInRole(RoleNames.Developer) || context.User.IsInRole(RoleNames.Administrator))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class AdministratorRequirement : AuthorizationHandler<AdministratorRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdministratorRequirement requirement)
        {
            if (context.User.IsInRole(RoleNames.Developer) || context.User.IsInRole(RoleNames.Administrator))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class UserRequirement : AuthorizationHandler<UserRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
        {
            if (context.User.IsInRole(RoleNames.Developer) || context.User.IsInRole(RoleNames.Administrator) || context.User.IsInRole(RoleNames.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    #endregion

    #region FirstNameAuth

    public class FirstNameAuthRequirement : IAuthorizationRequirement
    {
        public string Name { get; set; }
        
        public FirstNameAuthRequirement(string   name)
        {
            Name = name;
        }
    }

    public class FirstNameAuthHandler : AuthorizationHandler<FirstNameAuthRequirement>
    {
        public UserManager<User> _userManager { get; set; }
        public IdentityDataContext _db { get; set; }

        public FirstNameAuthHandler(UserManager<User> userManager, IdentityDataContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FirstNameAuthRequirement requirement)
        {
            var claimIdentifire = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if(claimIdentifire == null)
                return Task.CompletedTask;

            var userid = long.Parse(claimIdentifire.Value);
            var user = _db.Users.FirstOrDefault(u => u.Id == userid);

            if (user != null && user.Name != null && user.Name.ToLower().Contains(requirement.Name.ToLower()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    #endregion

    #region 1000 Days

    public interface INumberOfDaysForAccount
    {
        int Get(long userId);
    }

    public class NumberOfDaysForAccount : INumberOfDaysForAccount
    {
        private readonly IdentityDataContext _db;
        public NumberOfDaysForAccount(IdentityDataContext db)
        {
            _db = db;
        }

        public int Get(long userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if(user != null && user.Created != DateTime.MinValue)
            {
                return (DateTime.Today - user.Created).Days;
            }
            
            return 0;
        }
    }

    public class AdminWithMoreThan1000DaysRequirement : IAuthorizationRequirement
    {
        public int Days { get; set; }
        
        public AdminWithMoreThan1000DaysRequirement(int days)
        {
            Days = days;
        }
    }

    public class AdminWithOver1000DaysHandler : AuthorizationHandler<AdminWithMoreThan1000DaysRequirement>
    {
        private readonly INumberOfDaysForAccount _numberOfDaysForAccount;

        public AdminWithOver1000DaysHandler(INumberOfDaysForAccount numberOfDaysForAccount)
        {
            _numberOfDaysForAccount = numberOfDaysForAccount;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminWithMoreThan1000DaysRequirement requirement)
        {
            if (!context.User.IsInRole(RoleNames.Administrator))
            {
                return Task.CompletedTask;
            }

            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null)
                return Task.CompletedTask;

            var userId = long.Parse(claim.Value);
            
            int numberOfDays = _numberOfDaysForAccount.Get(userId);
            if(numberOfDays >= requirement.Days)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    #endregion
}
