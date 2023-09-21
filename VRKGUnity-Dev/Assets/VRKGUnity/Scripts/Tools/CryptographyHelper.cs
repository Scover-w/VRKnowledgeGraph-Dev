using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CryptographyHelper
{
    // TODO : Improve security (ex: BKDF2 for key derivation, salt)


    public static string Encrypt(string text, string key)
    {
        var hashKey = HashKey(key);
        using Aes aesAlg = Aes.Create();

        aesAlg.KeySize = 256;
        aesAlg.Key = hashKey;
        aesAlg.GenerateIV();
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        byte[] encryptedBytes;

        // Don't simplify the using statements, it creates an error
        using (MemoryStream msEncrypt = new())
        {
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(text);
                }
            }
            encryptedBytes = msEncrypt.ToArray();
        }

        byte[] result = new byte[aesAlg.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aesAlg.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string cipherText, string key)
    {
        var hashKey = HashKey(key);
        byte[] fullCipher = Convert.FromBase64String(cipherText);

        byte[] iv = new byte[16];
        byte[] cipher = new byte[fullCipher.Length - iv.Length];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        using Aes aesAlg = Aes.Create();
        aesAlg.KeySize = 256;
        aesAlg.Key = hashKey;
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        string decipheredText;

        // Don't simplify the using statements, it creates an error
        using (MemoryStream msDecrypt = new(cipher))
        {
            using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new(csDecrypt))
                {
                    decipheredText = srDecrypt.ReadToEnd();
                }
            }
        }

        return decipheredText;
    }


    private static byte[] HashKey(string preKey)
    {
        using SHA256 sha256Hash = SHA256.Create();

        return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(preKey));
    }
}
