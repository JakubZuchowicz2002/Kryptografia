using System;
using System.Security.Cryptography;
using System.Text;

namespace Kryptografia
{
    public static class SzyfrAES
    {
        public enum TrybAES { ECB, CBC, CTR, GCM }

        public static string Szyfruj(string tekst, byte[] klucz, byte[] iv, TrybAES tryb, out byte[]? tag, bool usePadding = true)
        {
            if (tekst is null) throw new ArgumentNullException(nameof(tekst));
            if (klucz is null) throw new ArgumentNullException(nameof(klucz));
            if (iv is null) throw new ArgumentNullException(nameof(iv));

            byte[] dane = Encoding.UTF8.GetBytes(tekst);
            byte[] wynik;
            tag = null;

            switch (tryb)
            {
                case TrybAES.ECB:
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = klucz;
                        aes.Mode = CipherMode.ECB;
                        aes.Padding = usePadding ? PaddingMode.PKCS7 : PaddingMode.None;

                        using var encryptor = aes.CreateEncryptor();
                        wynik = encryptor.TransformFinalBlock(dane, 0, dane.Length);
                    }
                    return Convert.ToBase64String(wynik);

                case TrybAES.CBC:
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = klucz;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = usePadding ? PaddingMode.PKCS7 : PaddingMode.None;

                        using var encryptor = aes.CreateEncryptor();
                        wynik = encryptor.TransformFinalBlock(dane, 0, dane.Length);
                    }
                    return $"{Convert.ToBase64String(wynik)}:{Convert.ToBase64String(iv)}";

                case TrybAES.CTR:
                    wynik = AES_CTR(dane, klucz, iv);
                    return $"{Convert.ToBase64String(wynik)}:{Convert.ToBase64String(iv)}";

                case TrybAES.GCM:
                    using (AesGcm aesGcm = new AesGcm(klucz))
                    {
                        wynik = new byte[dane.Length];
                        tag = new byte[16];
                        aesGcm.Encrypt(iv, dane, wynik, tag);
                    }
                    return $"{Convert.ToBase64String(wynik)}:{Convert.ToBase64String(tag)}:{Convert.ToBase64String(iv)}";

                default:
                    throw new ArgumentException("Nieobsługiwany tryb AES.", nameof(tryb));
            }
        }

        public static string Odszyfruj(string pakiet, byte[] klucz, TrybAES tryb, bool usePadding = true)
        {
            if (pakiet is null) throw new ArgumentNullException(nameof(pakiet));
            if (klucz is null) throw new ArgumentNullException(nameof(klucz));

            string[] parts = pakiet.Split(':', StringSplitOptions.RemoveEmptyEntries);
            byte[] cipherBytes;
            byte[] iv;
            byte[]? tag = null;

            switch (tryb)
            {
                case TrybAES.GCM:
                    if (parts.Length != 3) throw new ArgumentException("Dla GCM oczekiwany format: cipherBase64:tagBase64:ivBase64");
                    cipherBytes = Convert.FromBase64String(parts[0]);
                    tag = Convert.FromBase64String(parts[1]);
                    iv = Convert.FromBase64String(parts[2]);
                    break;

                case TrybAES.CBC:
                case TrybAES.CTR:
                    if (parts.Length != 2) throw new ArgumentException("Dla CBC/CTR oczekiwany format: cipherBase64:ivBase64");
                    cipherBytes = Convert.FromBase64String(parts[0]);
                    iv = Convert.FromBase64String(parts[1]);
                    break;

                case TrybAES.ECB:
                    cipherBytes = Convert.FromBase64String(parts[0]);
                    iv = new byte[16];  
                    break;

                default:
                    throw new ArgumentException("Nieobsługiwany tryb AES.", nameof(tryb));
            }

            byte[] wynikBytes;

            switch (tryb)
            {
                case TrybAES.ECB:
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = klucz;
                        aes.Mode = CipherMode.ECB;
                        aes.Padding = usePadding ? PaddingMode.PKCS7 : PaddingMode.None;

                        using var decryptor = aes.CreateDecryptor();
                        wynikBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    }
                    break;

                case TrybAES.CBC:
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = klucz;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = usePadding ? PaddingMode.PKCS7 : PaddingMode.None;

                        using var decryptor = aes.CreateDecryptor();
                        wynikBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    }
                    break;

                case TrybAES.CTR:
                    wynikBytes = AES_CTR(cipherBytes, klucz, iv);
                    break;

                case TrybAES.GCM:
                    if (tag is null) throw new ArgumentException("Brak tagu dla GCM.");
                    using (AesGcm aesGcm = new AesGcm(klucz))
                    {
                        wynikBytes = new byte[cipherBytes.Length];
                        aesGcm.Decrypt(iv, cipherBytes, tag, wynikBytes);
                    }
                    break;

                default:
                    throw new ArgumentException("Nieobsługiwany tryb AES.", nameof(tryb));
            }

            return Encoding.UTF8.GetString(wynikBytes);
        }

        private static byte[] AES_CTR(byte[] dane, byte[] klucz, byte[] nonce)
        {
            using Aes aes = Aes.Create();
            aes.Key = klucz;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            using var encryptor = aes.CreateEncryptor();
            byte[] wynik = new byte[dane.Length];
            byte[] counterBlock = new byte[16];
            Array.Copy(nonce, counterBlock, Math.Min(nonce.Length, counterBlock.Length));

            int blocks = (int)Math.Ceiling(dane.Length / 16.0);
            byte[] encryptedCounter = new byte[16];

            for (int i = 0; i < blocks; i++)
            {
                int produced = encryptor.TransformBlock(counterBlock, 0, 16, encryptedCounter, 0);
                if (produced != 16) throw new CryptographicException("Nie otrzymano 16 bajtów przy szyfrowaniu CTR.");

                int offset = i * 16;
                int length = Math.Min(16, dane.Length - offset);

                for (int j = 0; j < length; j++)
                    wynik[offset + j] = (byte)(dane[offset + j] ^ encryptedCounter[j]);

                for (int j = 15; j >= 0; j--)
                    if (++counterBlock[j] != 0) break;
            }

            return wynik;
        }
    }
}
