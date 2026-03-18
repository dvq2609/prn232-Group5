using System;
using System.IO;
using System.Security.Cryptography;

namespace backend.Helpers;

public static class EncryptionHelper
{
    // Encrypts a string using AES
    public static string EncryptString(string plainText, string base64Key)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        byte[] keyBytes = Convert.FromBase64String(base64Key);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyBytes;
            aesAlg.GenerateIV(); // Generate a new IV for each encryption

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                byte[] iv = aesAlg.IV;
                byte[] encryptedContent = msEncrypt.ToArray();

                // Combine IV and CipherText
                byte[] result = new byte[iv.Length + encryptedContent.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    // Decrypts a string using AES
    public static string DecryptString(string cipherText, string base64Key)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] keyBytes = Convert.FromBase64String(base64Key);

            using (Aes aesAlg = Aes.Create())
            {
                // The IV is always the first 16 bytes for AES block size of 128
                byte[] iv = new byte[16];
                byte[] cipher = new byte[fullCipher.Length - 16];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, 16, cipher, 0, cipher.Length);

                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipher))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch
        {
            // If decryption fails (e.g., legacy plain text message), return the original text
            return cipherText;
        }
    }
}
