using IdentityModule.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityModule.Authorize
{
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
            if(user!=null && user.Created != DateTime.MinValue)
            {
                return (DateTime.Today - user.Created).Days;
            }
            return 0;
        }
    }
}
