using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CT.Helper
{
    public static class Hasher
    {
        public static byte[] Hash(string s)
        {
            var provider = new SHA256CryptoServiceProvider();
            var bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(s));
            return bytes;
        }
    }
}