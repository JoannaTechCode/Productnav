using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TypicalTools.Services
{
    public class EncryptionService
    {
        string secretKey;

        public EncryptionService(IConfiguration config)
        {
            secretKey = config["SecretKey"];
        }

        public byte[] EncryptByteArray(byte[] fileData)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(secretKey);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor();


                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, 16);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(fileData, 0, fileData.Length);
                        csEncrypt.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }
            }
        }


        public byte[] DecryptByteArray(byte[] encryptedData)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(secretKey);
                byte[] IV = new byte[16];
                Array.Copy(encryptedData, IV, 16);
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedData, 16, encryptedData.Length - 16);
                        csDecrypt.FlushFinalBlock();
                        return msDecrypt.ToArray();

                    }

                }
            }
        }
    }
}
