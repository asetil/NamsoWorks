//using System;
//using System.IO;
//using System.Security.Cryptography;

//namespace Aware.Util
//{
//    public class CryptHelper : ICryptHelper
//    {
//        public string Encrypt(CryptModel cryptModel)
//        {
//            if (string.IsNullOrEmpty(cryptModel.Text))
//                throw new ArgumentNullException("cryptModel");

//            var aesAlg = NewRijndaelManaged(cryptModel);

//            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
//            var msEncrypt = new MemoryStream();
//            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
//            using (var swEncrypt = new StreamWriter(csEncrypt))
//            {
//                swEncrypt.Write(cryptModel.Text);
//            }

//            return Convert.ToBase64String(msEncrypt.ToArray());
//        }

//        public string Decrypt(CryptModel cryptModel)
//        {
//            if (string.IsNullOrEmpty(cryptModel.Text))
//                throw new ArgumentNullException("cryptModel");

//            if (!IsBase64String(cryptModel.Text))
//                throw new Exception("The cipherText input parameter is not base64 encoded");

//            string text;

//            var aesAlg = NewRijndaelManaged(cryptModel);
//            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
//            var cipher = Convert.FromBase64String(cryptModel.Text);

//            using (var msDecrypt = new MemoryStream(cipher))
//            {
//                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//                {
//                    using (var srDecrypt = new StreamReader(csDecrypt))
//                    {
//                        text = srDecrypt.ReadToEnd();
//                    }
//                }
//            }
//            return text;
//        }

//        private RijndaelManaged NewRijndaelManaged(CryptModel model)
//        {
//            if (model.SaltValue == null) throw new ArgumentNullException("model");
//            var saltBytes = Encoding.ASCII.GetBytes(model.SaltValue);
//            var key = new Rfc2898DeriveBytes(model.SaltValue, saltBytes);

//            var aesAlg = new RijndaelManaged();
//            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
//            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

//            return aesAlg;
//        }

//        private bool IsBase64String(string base64String)
//        {
//            base64String = base64String.Trim();
//            return (base64String.Length % 4 == 0) &&
//                   Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

//        }

//        public string Encrypt(string data)
//        {
//            return Base64Encode(Encrypt(GetCryptModel(data)));
//        }

//        public string Decrypt(string data)
//        {
//            return Decrypt(GetCryptModel(Base64Decode(data)));
//        }

//        private CryptModel GetCryptModel(string text)
//        {
//            const string passPhrase = "dC4i2no9xt";
//            const string saltValue = "D7inaOi6riZ";
//            const string hashAlgorithm = "SHA1";
//            const int passwordIterations = 2;
//            const string initVector = "@1B2c3D4e5F6g7H8";
//            const int keySize = 256;

//            return new CryptModel
//            {
//                HashAlgorithm = hashAlgorithm,
//                InitVector = initVector,
//                KeySize = keySize,
//                PassPhrase = passPhrase,
//                PasswordIterations = passwordIterations,
//                SaltValue = saltValue,
//                Text = text
//            };
//        }

//        private string Base64Encode(string value)
//        {
//            string result = string.Empty;

//            if (!string.IsNullOrEmpty(value))
//            {
//                result = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
//            }

//            return result;
//        }

//        private string Base64Decode(string value)
//        {
//            string result = string.Empty;

//            try
//            {
//                if (!string.IsNullOrEmpty(value))
//                {
//                    result = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
//                }
//            }
//            catch (Exception ex)
//            {
//            }

//            return result;
//        }

//    }
//}

//public class CryptModel
//{
//    public string Text { get; set; }
//    public string PassPhrase { get; set; }
//    public string SaltValue { get; set; }
//    public string HashAlgorithm { get; set; }
//    public int PasswordIterations { get; set; }
//    public string InitVector { get; set; }
//    public int KeySize { get; set; }
//}