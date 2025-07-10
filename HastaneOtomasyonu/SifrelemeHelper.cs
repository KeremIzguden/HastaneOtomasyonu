using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SifrelemeHelper
{
    private static readonly string key = "0123456789ABCDEF0123456789ABCDEF"; // 32 karakter
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("1234567890ABCDEF"); // 16 byte sabit IV

    public static string Sifrele(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv; // Sabit IV

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public static string SifreCoz(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            return "";

        try
        {
            byte[] cipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream(cipher))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cryptoStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        catch
        {
            return "";
        }
    }

}
