using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tutorial7.Handlers
{
    public class PasswordHandler
    {
        public  string hashing(string value, string salt)
        {
            var valuebytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valuebytes);
        }

        public  string createsalt()
        {
            byte[] randombytes = new Byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randombytes);
                return Convert.ToBase64String(randombytes);

            }
        }


        public  bool validate(string value, string salt, string hash)
            => hashing(value, salt) == hash;
    }

}
