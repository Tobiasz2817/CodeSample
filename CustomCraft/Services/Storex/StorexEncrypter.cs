using System;
using System.IO;
using System.Security.Cryptography;

namespace Storex {
    public static class StorexEncrypter {
        // You can create with SaveKeyGenerator class
        const string Key = "qaKnKPGVdW72GMqgVSDolc3ONoLziKp6ydI3m34f05I="; // 24 bytes
        const string Iv = "p/uD2IBa4ULy0wfX5bGpRA=="; // 16 bytes
        
        public static string Encrypt(string plainText) {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(Key);
            aes.IV = Convert.FromBase64String(Iv);
            
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                using (var sw = new StreamWriter(cs)) {
                    sw.Write(plainText);
                }
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText) {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(Key);
            aes.IV = Convert.FromBase64String(Iv);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        
        public static (string, string) GenerateKeyAndIv() {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            return (Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
        }
    }
}
