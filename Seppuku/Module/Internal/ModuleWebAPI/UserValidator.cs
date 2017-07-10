using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Security;
using Seppuku.Config;
using Seppuku.Module.Utility;

namespace Seppuku.Module.Internal.ModuleWebAPI
{
    public class UserValidator : IUserValidator
    {
        public IUserIdentity Validate(string username, string password)
        {
            if (username == "seppuku" && password == SeppukuAuth.GetCurrentToken(Conf.Get("Secret", "")))
            {
                return new UserIdentity()
                {
                    UserName = username
                };
            }
            return null;
        }
    }
}
