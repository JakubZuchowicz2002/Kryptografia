namespace Kryptografia
{
    public static class SzyfrCezara
    {
        public static string Szyfruj(string tekst, int przesuniecie)
        {
            char[] zaszyfrowanyTekst = new char[tekst.Length];

            for (int i = 0; i < tekst.Length; i++)
            {
                char znak = tekst[i];

                if (char.IsLetter(znak))
                {
                    char baza = char.IsUpper(znak) ? 'A' : 'a';
                    znak = (char)((((znak + przesuniecie) - baza) % 26) + baza);
                }

                zaszyfrowanyTekst[i] = znak;
            }

            return new string(zaszyfrowanyTekst);
        }
    }
}