﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MApp.DA
{
    /// <summary>
    /// used for encrypting string
    /// http://kristianguevara.net/creating-your-asp-net-mvc-5-application-from-scratch-for-beginners-using-entity-framework-6-and-identity-with-crud-functionalities/
    /// </summary>
    public class CustomEnrypt
    {
        /// <summary>
        /// encryptes string
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns>encrypted string</returns>
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "OJ1KBFZI56PO8NQ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}
