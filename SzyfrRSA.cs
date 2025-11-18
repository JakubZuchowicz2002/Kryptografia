using System;
using System.Security.Cryptography;
using System.Text;

namespace Kryptografia
{
    public static class SzyfrRSA
    {
        public static (string publicKey, string privateKey) GenerujKlucze(int dlugoscKlucza = 2048)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(dlugoscKlucza))
            {
                string publicKey = rsa.ToXmlString(false);
                string privateKey = rsa.ToXmlString(true);
                return (publicKey, privateKey);
            }
        }

        public static string Szyfruj(string tekst, string kluczPubliczny)
        {
            if (string.IsNullOrEmpty(tekst))
                throw new ArgumentException("Tekst nie może być pusty");

            if (string.IsNullOrEmpty(kluczPubliczny))
                throw new ArgumentException("Klucz publiczny nie może być pusty");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(kluczPubliczny);

                    byte[] dane = Encoding.UTF8.GetBytes(tekst);
                    byte[] zaszyfrowane = rsa.Encrypt(dane, true); // true = OAEP padding dla bezpieczeństwa

                    return Convert.ToBase64String(zaszyfrowane);
                }
                catch (CryptographicException ex)
                {
                    throw new Exception($"Błąd szyfrowania RSA: {ex.Message}");
                }
            }
        }

        public static string Odszyfruj(string zaszyfrowanyTekst, string kluczPrywatny)
        {
            if (string.IsNullOrEmpty(zaszyfrowanyTekst))
                throw new ArgumentException("Zaszyfrowany tekst nie może być pusty");

            if (string.IsNullOrEmpty(kluczPrywatny))
                throw new ArgumentException("Klucz prywatny nie może być pusty");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(kluczPrywatny);

                    byte[] dane = Convert.FromBase64String(zaszyfrowanyTekst);
                    byte[] odszyfrowane = rsa.Decrypt(dane, true); // true = OAEP padding

                    return Encoding.UTF8.GetString(odszyfrowane);
                }
                catch (CryptographicException ex)
                {
                    throw new Exception($"Błąd odszyfrowania RSA: {ex.Message}");
                }
            }
        }

        public static string SzyfrujKompatybilny(string tekst, string kluczPublicznyXml)
        {
            return Szyfruj(tekst, kluczPublicznyXml);
        }

        public static string OdszyfrujKompatybilny(string zaszyfrowanyTekst, string kluczPrywatnyXml)
        {
            return Odszyfruj(zaszyfrowanyTekst, kluczPrywatnyXml);
        }
    }
}