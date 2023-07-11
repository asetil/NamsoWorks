using System;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Extensions.Configuration;
using Aware.Util;

namespace Aware.Manager
{
    public class EncryptManager : IEncryptManager
    {
        private string _Pwd = "Aw@73e_NyZh*+!34?€_";
        private readonly byte[] _Salt = new byte[] { 0x45, 0xF1, 0x61, 0x6e, 0x20, 0x00, 0x65, 0x64, 0x76, 0x65, 0x64, 0x03, 0x76 };

        public EncryptManager(IConfiguration configuration)
        {
            _Pwd += configuration.GetValue("ClientSecret");
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                if (!string.IsNullOrEmpty(cipherText))
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_Pwd, _Salt);
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
            }
            catch (Exception)
            {

            }
            return cipherText;
        }

        private byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            CryptoStream cs = null;

            try
            {
                var alg = Rijndael.Create();
                alg.Key = key;
                alg.IV = iv;

                var ms = new MemoryStream();
                cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(cipherData, 0, cipherData.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                cs.Close();
            }
        }

        public string Encrypt(string clearText)
        {
            try
            {
                if (!string.IsNullOrEmpty(clearText))
                {
                    byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_Pwd, _Salt);
                    byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (Exception)
            {

            }
            return clearText;
        }


        private byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
        {

            CryptoStream cs = null;
            try
            {
                var alg = Rijndael.Create();
                alg.Key = key;
                alg.IV = iv;

                var ms = new MemoryStream();
                cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearData, 0, clearData.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                cs.Close();
            }
        }
    }
}