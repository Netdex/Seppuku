using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Seppuku.Module.Utility
{
    public class SeppukuAuth
    {
        public static string GetCurrentToken(string secret)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            return Convert.ToBase64String(
                sha1.ComputeHash(
                    Encoding.ASCII.GetBytes(
                        secret + DateTime.UtcNow.Date)))
                        .Replace('+','-').Replace('/','_').Replace('=','.');
        }
    }
}
