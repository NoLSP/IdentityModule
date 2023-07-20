using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using IdentityModule.Database;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;

namespace IdentityModule.Models
{
    public class MyUserStore : UserStore<User, Role, IdentityDataContext, long>
    {
        public MyUserStore(IdentityDataContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
        {
        }
    }
}
