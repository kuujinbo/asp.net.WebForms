/* ###########################################################################
 * simple symmetric encryption
 * ###########################################################################
 */
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace kuujinbo.asp.net.WebForms
{
    public class Crypto
    {
        // ===========================================================================
        // symmetric encryption key
        private string _base64Key;
        // ----------------------------------------------------------------------------
        public Crypto()
        {
            _base64Key = GenerateRijndaelManagedKey();
        }
        public Crypto(string base64Key)
        {
            _base64Key = base64Key;
        }
        /*
         * ###########################################################################
         * Base64 encoded
         * ###########################################################################
        */
        // @param data => clear text
        // RETURN => **Base64 encoded** encrypted string
        public string EncryptToBase64(string data)
        {
            byte[] clear = Encoding.UTF8.GetBytes(data);
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Convert.FromBase64String(_base64Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    // append IV
                    rm.GenerateIV();
                    ms.Write(rm.IV, 0, rm.IV.Length);

                    using (CryptoStream cs = new CryptoStream(
                      ms, rm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clear, 0, clear.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        // ---------------------------------------------------------------------------    
        // @param base64String => **Base64 encoded** string
        // RETURN => plain-text string
        public string DecryptBase64(string base64String)
        {
            byte[] encrypted = Convert.FromBase64String(base64String);
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Convert.FromBase64String(_base64Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    int readpos = 0;
                    byte[] iv = new byte[rm.IV.Length];
                    Array.Copy(encrypted, iv, iv.Length);
                    rm.IV = iv;
                    readpos += rm.IV.Length;
                    using (CryptoStream cs = new CryptoStream(
                      ms, rm.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        // don't read appended IV from encryption
                        cs.Write(encrypted, readpos, encrypted.Length - readpos);
                        cs.FlushFinalBlock();
                        using (StreamReader r = new StreamReader(ms, Encoding.UTF8))
                        {
                            ms.Position = 0;
                            return r.ReadToEnd();
                        }
                    }
                }
            }
        }

        /*
         * ###########################################################################
         * byte array
         * ###########################################################################
        */
        // encrypt byte array
        public byte[] Encrypt(byte[] clear)
        {
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Convert.FromBase64String(_base64Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    // append IV
                    rm.GenerateIV();
                    ms.Write(rm.IV, 0, rm.IV.Length);

                    using (CryptoStream cs = new CryptoStream(
                      ms, rm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clear, 0, clear.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }
        // ----------------------------------------------------------------------------
        // decrypt byte array
        public byte[] Decrypt(byte[] encrypted)
        {
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = Convert.FromBase64String(_base64Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    int readpos = 0;
                    byte[] iv = new byte[rm.IV.Length];
                    Array.Copy(encrypted, iv, iv.Length);
                    rm.IV = iv;
                    readpos += rm.IV.Length;
                    using (CryptoStream cs = new CryptoStream(
                      ms, rm.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        // don't read appended IV from encryption
                        cs.Write(encrypted, readpos, encrypted.Length - readpos);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }
        /*
         * ###########################################################################
         * helper methods
         * ###########################################################################
        */
        // generate Base64 symmetric encryption key
        public static string GenerateRijndaelManagedKey()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[32];
            rng.GetBytes(salt);

            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.KeySize = 256;
                rm.BlockSize = 256;
                rm.IV = salt;
                rm.GenerateKey();

                byte[] key = rm.Key;
                using (MemoryStream ms = new MemoryStream(key))
                {
                    ms.Write(key, 0, key.Length);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        // ===========================================================================  
    }
}