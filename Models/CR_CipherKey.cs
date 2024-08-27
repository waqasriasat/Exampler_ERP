using System.Security.Cryptography;
using System.Text;

namespace Exampler_ERP.Models
{
  public class CR_CipherKey
  {
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwx"); // 16 bytes for AES-128, 24 bytes for AES-192, 32 bytes for AES-256
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes IV for AES

    public static string Encrypt(string plainText)
    {
      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = Key;
        aesAlg.IV = IV;

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
          return Convert.ToBase64String(msEncrypt.ToArray());
        }
      }
    }
    public static string Decrypt(string cipherText)
    {
      byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
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
  }
}
