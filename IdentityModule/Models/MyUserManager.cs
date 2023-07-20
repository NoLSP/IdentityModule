using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace IdentityModule.Models
{
    public class MyUserManager : UserManager<User>
    {
        public MyUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {
            user.CreationDateTime = DateTime.UtcNow;

            return base.CreateAsync(user, password);
        }

        public override Task<IdentityResult> CreateAsync(User user)
        {
            user.CreationDateTime = DateTime.UtcNow;

            return base.CreateAsync(user);
        }
    }
}
