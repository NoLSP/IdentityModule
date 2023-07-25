﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityModule.Database
{
    public static class ClaimNames
    {
        public const string Create = "Create";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
    }

    public static class ClaimStore
    {
        public static List<Claim> ClaimsList = new List<Claim>()
        {
            new Claim("Create","Create"),
            new Claim("Edit","Edit"),
            new Claim("Delete","Delete")
        };
    }
}
