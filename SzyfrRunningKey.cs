namespace Kryptografia
{
    public static class SzyfrRunningKey
    {
        public static string Szyfruj(string tekst, string klucz)
        {
            char[] wynik = new char[tekst.Length];
            int kluczIndex = 0;
            klucz = klucz.ToLower();

            for (int i = 0; i < tekst.Length; i++)
            {
                char znak = tekst[i];
                if (char.IsLetter(znak) && kluczIndex < klucz.Length)
                {
                    char baza = char.IsUpper(znak) ? 'A' : 'a';
                    int przesuniecie = klucz[kluczIndex] - 'a';
                    znak = (char)((((znak - baza) + przesuniecie) % 26) + baza);
                    kluczIndex++;
                }
                wynik[i] = znak;
            }
            return new string(wynik);
        }
    }
}