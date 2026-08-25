using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Common
{
    public static class UserCodeGenerator
    {
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        private const int CodeLength = 5;

        public static string Generate()
        {
            var chars = new char[CodeLength];
            var bytes = RandomNumberGenerator.GetBytes(CodeLength);

            for (int i = 0; i < CodeLength; i++)
            {
                chars[i] = AllowedChars[bytes[i] % AllowedChars.Length];
            }

            return new string(chars);
        }
    }
}
