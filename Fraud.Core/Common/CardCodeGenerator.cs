using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.Common
{
    public class CardCodeGenerator
    {
        private const string AllowedChars = "123456789";
        private const int CodeLength = 16;
        private const string prefix = "56427879";


        public static string Generate()
        {

            var chars = new char[CodeLength];
            var bytes = RandomNumberGenerator.GetBytes(CodeLength);

            for (int i = 0; i < CodeLength; i++)
            {
                if (i < 8)
                {
                    chars[i] = prefix[i];
                }
                else
                {
                    chars[i] = AllowedChars[bytes[i] % AllowedChars.Length];
                }
             }


            return new string(chars);
        }

    }
}
